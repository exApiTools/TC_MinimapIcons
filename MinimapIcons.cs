using System;
using System.Collections.Generic;
using System.Linq;
using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.Elements;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Abstract;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using SharpDX;
using Vector2 = System.Numerics.Vector2;

namespace MinimapIcons;

public class MinimapIcons : BaseSettingsPlugin<MapIconsSettings>
{
    private static readonly List<string> Ignored =
    [
        "Metadata/Monsters/LeagueAffliction/DoodadDaemons/DoodadDaemonEyes1",
        "Metadata/Monsters/LeagueAffliction/DoodadDaemons/DoodadDaemonEyes2",
        "Metadata/Monsters/LeagueAffliction/DoodadDaemons/DoodadDaemonEyes3",
        "Metadata/Monsters/LeagueAffliction/DoodadDaemons/DoodadDaemonSpikes",
        "Metadata/Monsters/LeagueAffliction/DoodadDaemons/DoodadDaemonSpikes2",
        "Metadata/Monsters/LeagueAffliction/DoodadDaemons/DoodadDaemonSpikes3",
        "Metadata/Monsters/LeagueAffliction/DoodadDaemons/DoodadDaemonPimple1",
        "Metadata/Monsters/LeagueAffliction/DoodadDaemons/DoodadDaemonPimple2",
        "Metadata/Monsters/LeagueAffliction/DoodadDaemons/DoodadDaemonPimple3",
        "Metadata/Monsters/LeagueAffliction/DoodadDaemons/DoodadDaemonGoatFillet1Vanish",
        "Metadata/Monsters/LeagueAffliction/DoodadDaemons/DoodadDaemonGoatFillet2Vanish",
        "Metadata/Monsters/LeagueAffliction/DoodadDaemons/DoodadDaemonGoatRhoa1Vanish",
        "Metadata/Monsters/LeagueAffliction/DoodadDaemons/DoodadDaemonGoatRhoa2Vanish",

        // Conquerors Ignores
        "Metadata/Monsters/AtlasExiles/AtlasExile1@",
        "Metadata/Monsters/AtlasExiles/CrusaderInfluenceMonsters/CrusaderArcaneRune",
        "Metadata/Monsters/AtlasExiles/AtlasExile2_",
        "Metadata/Monsters/AtlasExiles/EyrieInfluenceMonsters/EyrieFrostnadoDaemon",
        "Metadata/Monsters/AtlasExiles/AtlasExile3@",
        "Metadata/Monsters/AtlasExiles/AtlasExile3AcidPitDaemon",
        "Metadata/Monsters/AtlasExiles/AtlasExile3BurrowingViperMelee",
        "Metadata/Monsters/AtlasExiles/AtlasExile3BurrowingViperRanged",
        "Metadata/Monsters/AtlasExiles/AtlasExile4@",
        "Metadata/Monsters/AtlasExiles/AtlasExile4ApparitionCascade",
        "Metadata/Monsters/AtlasExiles/AtlasExile5Apparition",
        "Metadata/Monsters/AtlasExiles/AtlasExile5Throne",

        // Incursion Ignores
        "Metadata/Monsters/LeagueIncursion/VaalSaucerRoomTurret",
        "Metadata/Monsters/LeagueIncursion/VaalSaucerTurret",
        "Metadata/Monsters/LeagueIncursion/VaalSaucerTurret",

        // Betrayal Ignores
        "Metadata/Monsters/LeagueBetrayal/BetrayalTaserNet",
        "Metadata/Monsters/LeagueBetrayal/FortTurret/FortTurret1Safehouse",
        "Metadata/Monsters/LeagueBetrayal/FortTurret/FortTurret1",
        "Metadata/Monsters/LeagueBetrayal/MasterNinjaCop",
        "Metadata/Monsters/LeagueBetrayal/BetrayalRikerMortarDaemon",
        "Metadata/Monsters/LeagueBetrayal/BetrayalBoneNovaDaemon",
        "Metadata/Monsters/LeagueBetrayal/BetrayalCatarinaPillarDaemon_",
        "Metadata/Monsters/LeagueBetrayal/BetrayalUpgrades/BetrayalDaemonCorpseConsume",

        // Legion Ignores
        "Metadata/Monsters/LegionLeague/LegionVaalGeneralProjectileDaemon",
        "Metadata/Monsters/LegionLeague/LegionSergeantStampedeDaemon",
        "Metadata/Monsters/LegionLeague/LegionSandTornadoDaemon",
        "Metadata/Monsters/LegionLeague/LegionVaalGeneralMoveDaemonQuad", // Viper Napuatzi, Legion Boss

        // Random Ignores
        "Metadata/Monsters/InvisibleFire/InvisibleSandstorm_",
        "Metadata/Monsters/InvisibleFire/InvisibleFrostnado",
        "Metadata/Monsters/InvisibleFire/InvisibleFireAfflictionDemonColdDegen",
        "Metadata/Monsters/InvisibleFire/InvisibleFireAfflictionDemonColdDegenUnique",
        "Metadata/Monsters/InvisibleFire/InvisibleFireAfflictionCorpseDegen",
        "Metadata/Monsters/InvisibleFire/InvisibleFireEyrieHurricane",
        "Metadata/Monsters/InvisibleFire/InvisibleIonCannonFrost",
        "Metadata/Monsters/InvisibleFire/AfflictionBossFinalDeathZone",
        "Metadata/Monsters/InvisibleFire/InvisibleFireDoedreSewers",
        "Metadata/Monsters/InvisibleFire/InvisibleFireDelveFlameTornadoSpiked",
        "Metadata/Monsters/InvisibleFire/InvisibleHolyCannon",
        "Metadata/Monsters/InvisibleFire/DelveVaalBossInvisibleLight",
        "Metadata/Monsters/InvisibleFire/InvisibleChaosstorm", // Avatar of Undoing, Geode Boss
        "Metadata/Monsters/InvisibleFire/InvisibleKitavaCannon",

        "Metadata/Monsters/InvisibleCurse/InvisibleFrostbiteStationary",
        "Metadata/Monsters/InvisibleCurse/InvisibleConductivityStationary",
        "Metadata/Monsters/InvisibleCurse/InvisibleEnfeeble",

        "Metadata/Monsters/InvisibleAura/InvisibleWrathStationary",

        // "Metadata/Monsters/Labyrinth/GoddessOfJustice",
        // "Metadata/Monsters/Labyrinth/GoddessOfJusticeMapBoss",
        "Metadata/Monsters/Frog/FrogGod/SilverOrb",
        "Metadata/Monsters/Frog/FrogGod/SilverPool",
        "Metadata/Monsters/LunarisSolaris/SolarisCelestialFormAmbushUniqueMap",
        "Metadata/Monsters/Invisible/MaligaroSoulInvisibleBladeVortex",
        "Metadata/Monsters/Daemon",
        "Metadata/Monsters/Daemon/MaligaroBladeVortexDaemon",
        "Metadata/Monsters/Daemon/SilverPoolChillDaemon",
        "Metadata/Monsters/AvariusCasticus/AvariusCasticusStatue",
        "Metadata/Monsters/Maligaro/MaligaroDesecrate",

        "Metadata/Monsters/Avatar/AvatarMagmaOrbDaemon",
        "Metadata/Monsters/Monkeys/FlameBearerTalismanT2Ghost",
        "Metadata/Monsters/Totems/TalismanTotem/TalismanTotemDeathscape",
        "Metadata/Monsters/BeehiveBehemoth/BeehiveBehemothSwampDaemon",
        "Metadata/Monsters/VaalWraith/VaalWraithChampionMinion",

        // Synthesis
        "Metadata/Monsters/LeagueSynthesis/SynthesisDroneBossTurret1",
        "Metadata/Monsters/LeagueSynthesis/SynthesisDroneBossTurret2",
        "Metadata/Monsters/LeagueSynthesis/SynthesisDroneBossTurret3",
        "Metadata/Monsters/LeagueSynthesis/SynthesisDroneBossTurret4",
        "Metadata/Monsters/LeagueSynthesis/SynthesisWalkerSpawned_",

        //Ritual
        "Metadata/Monsters/LeagueRitual/FireMeteorDaemon",
        "Metadata/Monsters/LeagueRitual/GenericSpeedDaemon",
        "Metadata/Monsters/LeagueRitual/ColdRotatingBeamDaemon",
        "Metadata/Monsters/LeagueRitual/ColdRotatingBeamDaemonUber",
        "Metadata/Monsters/LeagueRitual/GenericEnergyShieldDaemon",
        "Metadata/Monsters/LeagueRitual/GenericMassiveDaemon",
        "Metadata/Monsters/LeagueRitual/ChaosGreenVinesDaemon_",
        "Metadata/Monsters/LeagueRitual/ChaosSoulrendPortalDaemon",
        "Metadata/Monsters/LeagueRitual/VaalAtziriDaemon",
        "Metadata/Monsters/LeagueRitual/LightningPylonDaemon",

        // Bestiary
        "Metadata/Monsters/LeagueBestiary/RootSpiderBestiaryAmbush",
        "Metadata/Monsters/LeagueBestiary/BlackScorpionBestiaryBurrowTornado",
        "Metadata/Monsters/LeagueBestiary/ModDaemonCorpseEruption",
        "Metadata/Monsters/LeagueBestiary/ModDaemonSandLeaperExplode1",
        "Metadata/Monsters/LeagueBestiary/ModDaemonStampede1",
        "Metadata/Monsters/LeagueBestiary/ModDaemonGraspingPincers1",
        "Metadata/Monsters/LeagueBestiary/ModDaemonPouncingShade1",
        "Metadata/Monsters/LeagueBestiary/ModDaemonPouncingShadeQuickHit",
        "Metadata/Monsters/LeagueBestiary/ModDaemonFire1",
        "Metadata/Monsters/LeagueBestiary/ModDaemonVultureBomb1",
        "Metadata/Monsters/LeagueBestiary/ModDaemonVultureBombCast1",
        "Metadata/Monsters/LeagueBestiary/ModDaemonParasiticSquid1",
        "Metadata/Monsters/LeagueBestiary/ModDaemonBloodRaven1",
        "Metadata/Monsters/LeagueBestiary/SandLeaperBestiaryClone",
        "Metadata/Monsters/LeagueBestiary/SpiderPlagueBestiaryExplode",
        "Metadata/Monsters/LeagueBestiary/ParasiticSquidBestiaryClone",
        "Metadata/Monsters/LeagueBestiary/HellionBestiaryClone",
        "Metadata/Monsters/LeagueBestiary/BestiarySpiderCocoon",
        "Metadata/Monsters/LeagueBestiary/GemFrogBestiaryClone",

        // Ritual
        "Metadata/Monsters/LeagueRitual/GoldenCoinDaemon",
        "Metadata/Monsters/LeagueRitual/GenericLifeDaemon",
        "Metadata/Monsters/LeagueRitual/GenericChargesDaemon"
    ];

    private readonly Dictionary<string, bool> IgnoreCache = new Dictionary<string, bool>();

    private IngameUIElements _ingameUi;
    private bool _largeMap;
    private float _mapScale;
    private Vector2 _mapCenter;
    private SubMap LargeMapWindow => GameController.Game.IngameState.IngameUi.Map.LargeMap;
    private CachedValue<List<BaseIcon>> _iconListCache;

    public override bool Initialise()
    {
        Graphics.InitImage("sprites.png");
        Graphics.InitImage("Icons.png");
        CanUseMultiThreading = true;
        _iconListCache = CreateIconListCache();
        Settings.IconListRefreshPeriod.OnValueChanged += (_, _) => CreateIconListCache();
        return true;
    }

    private TimeCache<List<BaseIcon>> CreateIconListCache()
    {
        return new TimeCache<List<BaseIcon>>(() =>
        {
            var entitySource = Settings.DrawNotValid
                ? GameController?.EntityListWrapper.Entities
                : GameController?.EntityListWrapper?.OnlyValidEntities;
            var baseIcons = entitySource?.Select(x => x.GetHudComponent<BaseIcon>())
                .Where(icon => icon != null)
                .OrderBy(x => x.Priority)
                .ToList();
            return baseIcons ?? [];
        }, Settings.IconListRefreshPeriod);
    }

    public override Job Tick()
    {
        _ingameUi = GameController.Game.IngameState.IngameUi;

        var smallMiniMap = _ingameUi.Map.SmallMiniMap;
        if (smallMiniMap.IsVisibleLocal)
        {
            var mapRect = smallMiniMap.GetClientRectCache;
            _mapCenter = mapRect.Center.ToVector2Num();
            _largeMap = false;
            _mapScale = smallMiniMap.MapScale;
        }
        else if (_ingameUi.Map.LargeMap.IsVisibleLocal)
        {
            var largeMapWindow = LargeMapWindow;
            _mapCenter = largeMapWindow.MapCenter;
            _largeMap = true;
            _mapScale = largeMapWindow.MapScale;
        }

        return null;
    }

    public override void Render()
    {
        if (!Settings.Enable.Value || !GameController.InGame || Settings.DrawOnlyOnLargeMap && !_largeMap) return;

        if (!Settings.IgnoreFullscreenPanels &&
            _ingameUi.FullscreenPanels.Any(x => x.IsVisible) ||
            !Settings.IgnoreLargePanels &&
            _ingameUi.LargePanels.Any(x => x.IsVisible))
            return;

        var playerRender = GameController?.Player?.GetComponent<Render>();
        if (playerRender == null) return;
        var playerPos = playerRender.PosNum.WorldToGrid();
        var playerHeight = -playerRender.UnclampedHeight;

        if (LargeMapWindow == null) return;

        var baseIcons = _iconListCache.Value;
        if (baseIcons == null) return;

        foreach (var icon in baseIcons)
        {
            if (icon?.Entity == null) continue;

            if (icon.Entity.Type == EntityType.WorldItem)
                continue;

            if (!Settings.DrawMonsters && icon.Entity.Type == EntityType.Monster)
                continue;

            if (IgnoreCache.GetOrAdd(icon.Entity.Path, () => Ignored.Any(x => icon.Entity.Path.StartsWith(x))))
                continue;

            if (icon.Entity.Path.StartsWith(
                    "Metadata/Monsters/AtlasExiles/BasiliskInfluenceMonsters/BasiliskBurrowingViper")
                && icon.Entity.Rarity != MonsterRarity.Unique)
                continue;

            if (!icon.Show())
                continue;

            if (icon.HasIngameIcon &&
                (!Settings.DrawReplacementsForGameIconsWhenOutOfRange || icon.Entity.IsValid) &&
                !icon.Entity.Path.Contains("Metadata/Terrain/Leagues/Delve/Objects/DelveWall"))
                continue;

            var iconGridPos = icon.GridPositionNum();
            var position = _mapCenter +
                           DeltaInWorldToMinimapDelta(iconGridPos - playerPos,
                               (playerHeight + GameController.IngameState.Data.GetTerrainHeightAt(iconGridPos)) * PoeMapExtension.WorldToGridConversion);

            var iconValueMainTexture = icon.MainTexture;
            var size = iconValueMainTexture.Size;
            var halfSize = size / 2f;
            icon.DrawRect = new RectangleF(position.X - halfSize, position.Y - halfSize, size, size);
            var drawRect = icon.DrawRect;
            if (!_largeMap)
            {
                _ingameUi.Map.SmallMiniMap.GetClientRectCache.Contains(ref drawRect, out var contains);
                if (!contains)
                    continue;
            }

            Graphics.DrawImage(iconValueMainTexture.FileName, drawRect, iconValueMainTexture.UV, iconValueMainTexture.Color);
            if (icon.Hidden())
            {
                var s = drawRect.Width * 0.5f;
                drawRect.Inflate(-s, -s);

                Graphics.DrawImage(icon.MainTexture.FileName, drawRect,
                    SpriteHelper.GetUV(MapIconsIndex.LootFilterSmallCyanCircle), Color.White);

                drawRect.Inflate(s, s);
            }

            if (!string.IsNullOrEmpty(icon.Text))
                Graphics.DrawText(icon.Text, position.Translate(0, Settings.ZForText), FontAlign.Center);
        }
    }

    private const float CameraAngle = 38.7f * MathF.PI / 180;
    private static readonly float CameraAngleCos = MathF.Cos(CameraAngle);
    private static readonly float CameraAngleSin = MathF.Sin(CameraAngle);

    private Vector2 DeltaInWorldToMinimapDelta(Vector2 delta, float deltaZ)
    {
        return _mapScale * Vector2.Multiply(new Vector2(delta.X - delta.Y, deltaZ - (delta.X + delta.Y)), new Vector2(CameraAngleCos, CameraAngleSin));
    }
}

public static class Extensions
{
    public static T GetOrAdd<TKey, T>(this Dictionary<TKey, T> dictionary, TKey key, Func<T> valueFunc)
    {
        if (dictionary.TryGetValue(key, out var result))
        {
            return result;
        }

        result = valueFunc();
        dictionary[key] = result;
        return result;
    }
}