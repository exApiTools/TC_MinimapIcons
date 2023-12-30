using System;
using System.Collections.Generic;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared;
using ExileCore.Shared.Abstract;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using JM.LinqFaster;
using SharpDX;

namespace IconsBuilder.Icons
{
    public class LegionIcon : BaseIcon
    {
        public LegionIcon(Entity entity, IconsBuilderSettings settings, Dictionary<string, Size2> modIcons) :
            base(entity, settings)
        {
            Update(entity, settings, modIcons);
        }

        public override string ToString()
        {
            return $"{Entity.Metadata} : {Entity.Type} ({Entity.Address}) T: {Text}";
        }

        public void Update(Entity entity, IconsBuilderSettings settings, Dictionary<string, Size2> modIcons)
        {
            MainTexture = new HudTexture("Icons.png");

            (MainTexture.Size, Text) = Rarity switch
            {
                MonsterRarity.White => (MainTexture.Size = settings.SizeEntityWhiteIcon, null),
                MonsterRarity.Magic => (MainTexture.Size = settings.SizeEntityMagicIcon, null),
                MonsterRarity.Rare => (MainTexture.Size = settings.SizeEntityRareIcon, null),
                MonsterRarity.Unique => (MainTexture.Size = settings.SizeEntityUniqueIcon, entity.RenderName),
                _ => throw new ArgumentException("Legion icon rarity corrupted.")
            };

            if (entity.Path.StartsWith("Metadata/Monsters/LegionLeague/MonsterChest", StringComparison.Ordinal) || Rarity == MonsterRarity.Unique)
            {
                MainTexture.UV = SpriteHelper.GetUV(MapIconsIndex.LootFilterLargeGreenSquare);
                MainTexture.Color = Color.LimeGreen;
                Hidden = () => false;
                Text = entity.RenderName;

                Show = () =>
                {
                    if (Entity.IsValid)
                        return Entity.GetComponent<Life>().HPPercentage > 0.02;

                    return Entity.IsAlive;
                };
            }
            else
            {
                if (entity.TryGetComponent<ObjectMagicProperties>(out var objectMagicProperties) && objectMagicProperties.Mods is { } mods)
                {
                    if (mods.Contains("MonsterConvertsOnDeath_")) Show = () => entity.IsValid && entity.IsAlive && entity.IsHostile;

                    var modName = mods.FirstOrDefaultF(modIcons.ContainsKey);

                    if (modName != null)
                    {
                        MainTexture = new HudTexture("sprites.png")
                        {
                            UV = SpriteHelper.GetUV(modIcons[modName], new Size2F(7, 8))
                        };
                        Priority = IconPriority.VeryHigh;
                    }
                    else
                    {
                        (MainTexture.UV, MainTexture.Color) = Rarity switch
                        {
                            MonsterRarity.White => (SpriteHelper.GetUV(MapIconsIndex.LootFilterLargeRedCircle), Color.White),
                            MonsterRarity.Magic => (SpriteHelper.GetUV(MapIconsIndex.LootFilterLargeBlueCircle), Color.White),
                            MonsterRarity.Rare => (SpriteHelper.GetUV(MapIconsIndex.LootFilterLargeYellowCircle), Color.White),
                            MonsterRarity.Unique => (SpriteHelper.GetUV(MapIconsIndex.LootFilterLargeWhiteHexagon), Color.DarkOrange),
                            _ => throw new ArgumentOutOfRangeException(nameof(Rarity), Rarity, null)
                        };
                    }
                }

                var statDictionary = Entity.Stats;

                if (statDictionary == null)
                {
                    Show = () => Entity.GetComponent<Life>().HPPercentage > 0.02;
                    return;
                }

                // stats are broken for legion monsters (3.11) therefore the further code will most likely not be hit

                if (statDictionary.Count == 0)
                {
                    statDictionary = entity.GetComponentFromMemory<Stats>()?.ParseStats() ?? statDictionary;
                    if (statDictionary.Count == 0) Text = "Error";
                }

                if (statDictionary.TryGetValue(GameStat.MonsterMinimapIcon, out var indexMinimapIcon))
                {
                    var name = (MapIconsIndex) indexMinimapIcon;
                    Text = name.ToString().Replace("Legion", "");
                    Priority = IconPriority.Critical;

                    var frozenCheck = new TimeCache<bool>(() =>
                    {
                        var stats = Entity.Stats;
                        if (stats.Count == 0) return false;
                        stats.TryGetValue(GameStat.FrozenInTime, out var frozenInTime);
                        stats.TryGetValue(GameStat.MonsterHideMinimapIcon, out var monsterHideMinimapIcon);
                        return frozenInTime == 1 && monsterHideMinimapIcon == 1 || frozenInTime == 0 && monsterHideMinimapIcon == 0;
                    }, 75);

                    Show = () => Entity.IsAlive && frozenCheck.Value;
                }
                else
                    Show = () => !Hidden() && Entity.GetComponent<Life>()?.HPPercentage > 0.02;
            }
        }
    }
}
