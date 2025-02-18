using System;
using System.Collections.Generic;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using GameOffsets.Native;
using SharpDX;
using Color = System.Drawing.Color;
using Vector2 = System.Numerics.Vector2;

namespace MinimapIcons.IconsBuilder.Icons;

public class HudTexture
{
    public HudTexture()
    {
    }

    public HudTexture(string fileName)
    {
        FileName = fileName;
    }

    public string FileName { get; set; }
    public RectangleF UV { get; set; } = new RectangleF(0, 0, 1, 1);
    public float Size { get; set; } = 13;
    public System.Drawing.Color Color { get; set; } = Color.White;
}

public abstract class BaseIcon
{
    public int Version;

    protected static readonly Dictionary<string, Vector2i> strongboxesUV = new Dictionary<string, Vector2i>
    {
        { "Metadata/Chests/StrongBoxes/Large", new Vector2i(7, 7) },
        { "Metadata/Chests/StrongBoxes/Strongbox", new Vector2i(1, 2) },
        { "Metadata/Chests/StrongBoxes/Armory", new Vector2i(2, 1) },
        { "Metadata/Chests/StrongBoxes/Arsenal", new Vector2i(4, 1) },
        { "Metadata/Chests/StrongBoxes/Artisan", new Vector2i(3, 1) },
        { "Metadata/Chests/StrongBoxes/Jeweller", new Vector2i(1, 1) },
        { "Metadata/Chests/StrongBoxes/Cartographer", new Vector2i(5, 1) },
        { "Metadata/Chests/StrongBoxes/CartographerLowMaps", new Vector2i(5, 1) },
        { "Metadata/Chests/StrongBoxes/CartographerMidMaps", new Vector2i(5, 1) },
        { "Metadata/Chests/StrongBoxes/CartographerHighMaps", new Vector2i(5, 1) },
        { "Metadata/Chests/StrongBoxes/Ornate", new Vector2i(7, 7) },
        { "Metadata/Chests/StrongBoxes/Arcanist", new Vector2i(1, 8) },
        { "Metadata/Chests/StrongBoxes/Gemcutter", new Vector2i(6, 1) },
        { "Metadata/Chests/StrongBoxes/StrongboxDivination", new Vector2i(7, 1) },
        { "Metadata/Chests/AbyssChest", new Vector2i(7, 7) }
    };

    protected bool _HasIngameIcon;

    public BaseIcon(Entity entity)
    {
        Entity = entity;

        if (Entity == null)
        {
            return;
        }

        Rarity = Entity.Rarity;

        Priority = Rarity switch
        {
            MonsterRarity.White => IconPriority.Low,
            MonsterRarity.Magic => IconPriority.Medium,
            MonsterRarity.Rare => IconPriority.High,
            MonsterRarity.Unique => IconPriority.Critical,
            _ => IconPriority.Critical
        };

        Show = () => Entity.IsValid;
        Hidden = () => entity.IsHidden;
        GridPosition = () => Entity.GridPosNum;

        if (Entity.TryGetComponent<MinimapIcon>(out var minimapIconComponent))
        {
            var name = minimapIconComponent.Name;

            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            var iconIndexByName = ExileCore.Shared.Helpers.Extensions.IconIndexByName(name);

            if (iconIndexByName != MapIconsIndex.MyPlayer)
            {
                MainTexture = new HudTexture("Icons.png") { UV = SpriteHelper.GetUV(iconIndexByName), Size = 16 };
                _HasIngameIcon = true;
            }

            if (Entity.HasComponent<Portal>() && Entity.TryGetComponent<Transitionable>(out var transitionable))
            {
                Text = RenderName;
                Show = () => Entity.IsValid && transitionable.Flag1 == 2;
            }
            else
            {
                Show = () => Entity.GetComponent<MinimapIcon>() is { IsVisible: true, IsHide: false };
            }
        }
    }

    public bool HasIngameIcon => _HasIngameIcon;
    public Entity Entity { get; }

    public Func<Vector2> GridPosition { get; set; }
    public RectangleF DrawRect { get; set; }
    public Func<bool> Show { get; set; }
    public Func<bool> Hidden { get; protected set; } = () => false;
    public HudTexture MainTexture { get; protected set; }
    public IconPriority Priority { get; protected set; }
    public MonsterRarity Rarity { get; protected set; }
    public string Text { get; protected set; }
    public string RenderName => Entity.RenderName;
}