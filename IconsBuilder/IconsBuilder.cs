﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;
using GameOffsets.Native;
using MinimapIcons.IconsBuilder.Icons;

namespace MinimapIcons.IconsBuilder;

public class IconsBuilder
{
    private readonly MinimapIcons _plugin;

    public IconsBuilder(MinimapIcons plugin)
    {
        _plugin = plugin;
    }

    public IconsBuilderSettings Settings => _plugin.Settings.IconsBuilderSettings;

    private string DefaultAlertFile => Path.Combine(_plugin.DirectoryFullName, "config", "mod_alerts.txt");
    private string CustomAlertFile => Path.Combine(_plugin.ConfigDirectory, "mod_alerts.txt");
    private string DefaultIgnoreFile => Path.Combine(_plugin.DirectoryFullName, "config", "ignored_entities.txt");
    private string CustomIgnoreFile => Path.Combine(_plugin.ConfigDirectory, "ignored_entities.txt");

    private List<string> IgnoredEntities { get; set; }
    private Dictionary<string, Vector2i> AlertEntitiesWithIconSize { get; set; } = new Dictionary<string, Vector2i>();

    private static EntityType[] SkippedEntityTypes =>
    [
        EntityType.HideoutDecoration, 
        EntityType.Effect, 
        EntityType.Light, 
        EntityType.ServerObject, 
        EntityType.Daemon,
        EntityType.Error,
    ];

    private int RunCounter { get; set; }
    private int IconVersion;
        
    private void ReadAlertFile()
    {
        var customAlertFilePath = CustomAlertFile;
        var path = File.Exists(customAlertFilePath) ? customAlertFilePath : DefaultAlertFile;
        if (!File.Exists(path))
        {
            DebugWindow.LogError($"IconsBuilder -> Alert entities file does not exist. Path: {path}");
            return;
        }
        var readAllLines = File.ReadAllLines(path);

        foreach (var readAllLine in readAllLines)
        {
            if (readAllLine.StartsWith('#')) continue;
            var entityMetadata = readAllLine.Split(';');
            var iconSize = entityMetadata[2].Trim().Split(',');
            AlertEntitiesWithIconSize[entityMetadata[0]] = new Vector2i(int.Parse(iconSize[0]), int.Parse(iconSize[1]));
        }
    }

    private void ReadIgnoreFile()
    {
        var customIgnoreFilePath = CustomIgnoreFile;
        var path = File.Exists(customIgnoreFilePath) ? customIgnoreFilePath : DefaultIgnoreFile;
        if (!File.Exists(path))
        {
           _plugin.LogError($"IconsBuilder -> Ignored entities file does not exist. Path: {path}");
            return;
        }
        IgnoredEntities = File.ReadAllLines(path).Where(line => !string.IsNullOrWhiteSpace(line) && !line.StartsWith('#')).ToList();
    }

    public void AreaChange(AreaInstance area)
    {
        ReadAlertFile();
        ReadIgnoreFile();
    }

    public bool Initialise()
    {
        Settings.ResetIcons.OnPressed = () => { IconVersion++; };
        ReadAlertFile();           
        ReadIgnoreFile();
        return true;
    }

    public void Tick()
    {
        RunCounter++;
        if (RunCounter % Settings.RunEveryXTicks.Value != 0) return;

        AddIcons();
    }

    private void AddIcons()
    {
        foreach (var entity in _plugin.GameController.Entities)
        {
            try
            {
                if (entity == null) continue;
                if (entity.GetHudComponent<BaseIcon>() is { Version: var version, } && version >= IconVersion) continue;
                if (SkipIcon(entity)) continue;

                var icon = GenerateIcon(entity);
                if (icon == null) continue;
                icon.Version = IconVersion;
                entity.SetHudComponent(icon);
            }
            catch (Exception ex)
            {
                _plugin.LogError($"Failed to build an icon for {entity}: {ex}");
            }
        }
    }

    private bool SkipIcon(Entity entity)
    {
        if (entity is not { IsValid: true }) return true;
        if (SkippedEntityTypes.Any(x => x == entity.Type)) return true;
        if (IgnoredEntities.Any(x => entity.Path?.Contains(x) == true)) return true;

        return false;
    }

    private readonly ConditionalWeakTable<string, Regex> _regexes = [];

    private BaseIcon GenerateIcon(Entity entity)
    {
        var metadata = entity.Metadata;
        if (Settings.CustomIcons.Content
                .FirstOrDefault(x => _regexes.GetValue(x.MetadataRegex.Value, p => new Regex(p))!.IsMatch(metadata)) is { } customIconConfig)
        {
            return new CustomIcon(entity, Settings, customIconConfig);
        }

        if (entity.Type == EntityType.WorldItem)
        {
            if (Settings.UseReplacementsForItemIconsWhenOutOfRange &&
                entity.TryGetComponent<WorldItem>(out var worldItem) && 
                worldItem.Icon is var icon && 
                icon != MapIconsIndex.None)
            {
                return new IngameItemReplacerIcon(entity, Settings, icon);
            }
            else
            {
                return null;
            }
        }

        if (Settings.UseReplacementsForGameIconsWhenOutOfRange &&
            entity.TryGetComponent<MinimapIcon>(out var minimapIconComponent) && 
            !minimapIconComponent.IsHide)
        {
            var name = minimapIconComponent.Name;
            if (!string.IsNullOrEmpty(name))
            {
                return new IngameIconReplacerIcon(entity, Settings, _plugin.Settings);
            }
        }

        //Monsters
        if (entity.Type == EntityType.Monster)
        {
            if (!entity.IsAlive) return null;

            if (entity.League == LeagueType.Legion)
                return new LegionIcon(entity, Settings, AlertEntitiesWithIconSize);
            if (entity.League == LeagueType.Delirium)
                return new DeliriumIcon(entity, Settings, AlertEntitiesWithIconSize);

            return new MonsterIcon(entity, Settings, AlertEntitiesWithIconSize);
        }

        //NPC
        if (entity.Type == EntityType.Npc)
            return new NpcIcon(entity, Settings);

        //Player
        if (entity.Type == EntityType.Player)
        {
            if (!entity.TryGetComponent<Player>(out var player) ||
                player.PlayerName is not {} playerName ||
                _plugin.GameController.IngameState.Data.LocalPlayer.Address == entity.Address ||
                _plugin.GameController.IngameState.Data.LocalPlayer.GetComponent<Render>().Name == entity.RenderName) return null;

            if (!entity.IsValid) return null;
            return new PlayerIcon(entity, Settings, playerName);
        }

        //Chests
        if (entity.Type == EntityType.Chest && !entity.IsOpened)
            return new ChestIcon(entity, Settings);

        //Area transition
        if (entity.Type == EntityType.AreaTransition)
            return new MiscIcon(entity, Settings);

        //Shrine
        if (entity.HasComponent<Shrine>())
            return new ShrineIcon(entity, Settings);

        if (entity.HasComponent<Transitionable>() && entity.HasComponent<MinimapIcon>())
        {
            //Mission marker
            if (entity.Path.Equals("Metadata/MiscellaneousObjects/MissionMarker", StringComparison.Ordinal) ||
                entity.GetComponent<MinimapIcon>().Name.Equals("MissionTarget", StringComparison.Ordinal))
                return new MissionMarkerIcon(entity, Settings);

            return new MiscIcon(entity, Settings);
        }

        if (entity.HasComponent<MinimapIcon>() && entity.HasComponent<Targetable>() ||
            entity.Path.Contains("Metadata/Terrain/Leagues/Delve/Objects/EncounterControlObjects/AzuriteEncounterController") ||
            entity.Type == EntityType.LegionMonolith ||
            entity.Path is "Metadata/Terrain/Leagues/Sanctum/Objects/SanctumMote")
            return new MiscIcon(entity, Settings);

        return null;
    }
}