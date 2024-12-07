using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using ExileCore2.PoEMemory.Components;
using ExileCore2.PoEMemory.MemoryObjects;
using ExileCore2.Shared;
using ExileCore2.Shared.Enums;
using ExileCore2.Shared.Helpers;
using GameOffsets2.Native;
using RectangleF = ExileCore2.Shared.RectangleF;

namespace MinimapIcons.IconsBuilder.Icons;

public abstract class BaseIcon
{
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

    protected static readonly Dictionary<string, Color> FossilRarity = new Dictionary<string, Color>
    {
        { "Fractured", Color.Aquamarine },
        { "Faceted", Color.Aquamarine },
        { "Glyphic", Color.Aquamarine },
        { "Hollow", Color.Aquamarine },
        { "Shuddering", Color.Aquamarine },
        { "Bloodstained", Color.Aquamarine },
        { "Tangled", Color.OrangeRed },
        { "Dense", Color.OrangeRed },
        { "Gilded", Color.OrangeRed },
        { "Sanctified", Color.Aquamarine },
        { "Encrusted", Color.Yellow },
        { "Aetheric", Color.Orange },
        { "Enchanted", Color.Orange },
        { "Pristine", Color.Orange },
        { "Prismatic", Color.Orange },
        { "Corroded", Color.Yellow },
        { "Perfect", Color.Orange },
        { "Jagged", Color.Yellow },
        { "Serrated", Color.Yellow },
        { "Bound", Color.Yellow },
        { "Lucent", Color.Yellow },
        { "Metallic", Color.Yellow },
        { "Scorched", Color.Yellow },
        { "Aberrant", Color.Yellow },
        { "Frigid", Color.Yellow }
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
        GridPosition = () => Entity.GridPos;

        if (Entity.TryGetComponent<MinimapIcon>(out var minimapIconComponent))
        {
            var name = minimapIconComponent.Name;

            if (!string.IsNullOrEmpty(name))
            {
                var iconIndexByName = ExileCore2.Shared.Helpers.Extensions.IconIndexByName(name);

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
                else if (Entity.Path.StartsWith("Metadata/Terrain/Labyrinth/Objects/Puzzle_Parts/Switch", StringComparison.Ordinal))
                {
                    Show = () =>
                    {
                        var transitionable = Entity.GetComponent<Transitionable>();
                        var minimapIcon = Entity.GetComponent<MinimapIcon>();
                        return Entity.IsValid && minimapIcon is { IsVisible: true, IsHide: false } && transitionable?.Flag1 != 2;
                    };
                }
                else if (Entity.Path.StartsWith("Metadata/MiscellaneousObjects/Abyss/Abyss"))
                {
                    Show = () =>
                    {
                        var minimapIcon = Entity.GetComponent<MinimapIcon>();

                        return Entity.IsValid && minimapIcon?.IsVisible == true &&
                               (minimapIcon.IsHide == false || Entity.GetComponent<Transitionable>().Flag1 == 1);
                    };
                }
                else if (entity.Path.Contains("Metadata/Terrain/Leagues/Delve/Objects/DelveMineral"))
                {
                    Priority = IconPriority.High;
                    MainTexture.UV = SpriteHelper.GetUV(MapIconsIndex.DelveMineralVein);
                    Text = "Sulphite";
                    Show = () => entity.IsValid && entity.IsTargetable;
                }
                else
                {
                    Show = () => Entity.GetComponent<MinimapIcon>() is { IsVisible: true, IsHide: false };
                }
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

    protected bool PathCheck(Entity path, params string[] check)
    {
        return check.Any(s => path.Path.Equals(s, StringComparison.Ordinal));
    }
}