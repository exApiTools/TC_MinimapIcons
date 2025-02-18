using System;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;

namespace MinimapIcons.IconsBuilder.Icons;

public class MiscIcon : BaseIcon
{
    public MiscIcon(Entity entity, IconsBuilderSettings settings) : base(entity)
    {
        Update(entity, settings);
    }

    public override string ToString()
    {
        return $"{Entity.Path} : ({Entity.Type}) :{Text}";
    }

    public void Update(Entity entity, IconsBuilderSettings settings)
    {
        if (!_HasIngameIcon)
        {
            MainTexture = new HudTexture();
            MainTexture.FileName = "Icons.png";
            MainTexture.Size = settings.SizeMiscIcon;
        }
        else
        {
            MainTexture.Size = settings.SizeDefaultIcon;
            Text = RenderName;
            Priority = IconPriority.VeryHigh;
            if (entity.GetComponent<MinimapIcon>()?.Name is "DelveRobot") Text = "Follow Me";

            return;
        }

        if (entity.HasComponent<Targetable>())
        {
            if (entity.Path is "Metadata/Terrain/Leagues/Synthesis/Objects/SynthesisPortal")
                Show = () => entity.IsValid;
            else
            {
                Show = () =>
                {
                    if (!entity.TryGetComponent<MinimapIcon>(out var minimapIcon)) return false;
                    var isVisible = minimapIcon.IsVisible && !minimapIcon.IsHide;
                    return entity.IsValid && isVisible && entity.IsTargetable;
                };
            }
        }
        else
            Show = () => entity.IsValid && entity.GetComponent<MinimapIcon>() is { IsVisible: true };

        if (entity.HasComponent<Transitionable>() && entity.HasComponent<MinimapIcon>())
        {
            bool TransitionableShow() =>
                entity.IsValid &&
                (entity.GetComponent<MinimapIcon>() is { IsHide: false } ||
                 entity.GetComponent<Transitionable>() is { Flag1: 1 });

            if (entity.Path.Equals("Metadata/MiscellaneousObjects/Abyss/AbyssCrackSpawners/AbyssCrackSkeletonSpawner"))
            {
                MainTexture.UV = SpriteHelper.GetUV(MapIconsIndex.AbyssCrack);
                Show = TransitionableShow;
            }
            else if (entity.Path.Equals("Metadata/MiscellaneousObjects/Abyss/AbyssStartNode"))
            {
                MainTexture.UV = SpriteHelper.GetUV(MapIconsIndex.Abyss);
                Show = TransitionableShow;
            }
            else if (entity.Path.Equals("Metadata/MiscellaneousObjects/Abyss/AbyssNodeSmall", StringComparison.Ordinal) ||
                     entity.Path.Equals("Metadata/MiscellaneousObjects/Abyss/AbyssNodeLarge", StringComparison.Ordinal) ||
                     entity.Path.StartsWith("Metadata/MiscellaneousObjects/Abyss/AbyssFinalNodeChest"))
            {
                Show = TransitionableShow;
            }
            else if (entity.Path.StartsWith("Metadata/Terrain/Leagues/Incursion/Objects/IncursionPortal", StringComparison.Ordinal))
                Show = () => entity.IsValid && entity.GetComponent<Transitionable>() is { Flag1: < 3 };
            else
            {
                Priority = IconPriority.Critical;
                Show = () => false;
            }
        }
        else if (entity.HasComponent<Targetable>())
        {
            if (entity.Path.Contains("Metadata/Terrain/Leagues/Delve/Objects/DelveMineral"))
            {
                Priority = IconPriority.High;
                MainTexture.UV = SpriteHelper.GetUV(MapIconsIndex.DelveMineralVein);
                Text = "Sulphite";
                Show = () => entity.IsValid && entity.IsTargetable;
            }
            else if (entity.Path.Contains("Metadata/Terrain/Leagues/Delve/Objects/EncounterControlObjects/AzuriteEncounterController"))
            {
                Priority = IconPriority.High;
                Text = "Start";
                Show = () => entity.IsValid && entity.GetComponent<Transitionable>() is { Flag1: < 3 };
                MainTexture.UV = SpriteHelper.GetUV(MapIconsIndex.PartyLeader);
            }
            else if (entity.Path is "Metadata/Terrain/Leagues/Sanctum/Objects/SanctumMote")
            {
                Priority = IconPriority.High;
                Text = "";
                Show = () => entity.IsValid;
                MainTexture.UV = SpriteHelper.GetUV(MapIconsIndex.BlightPath);
                MainTexture.Size = settings.SanctumGoldIconSize;
            }
        }
    }
}