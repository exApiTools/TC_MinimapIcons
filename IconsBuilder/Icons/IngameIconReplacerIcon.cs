using System;
using System.Linq;
using ExileCore2.PoEMemory;
using ExileCore2.PoEMemory.Components;
using ExileCore2.PoEMemory.MemoryObjects;
using ExileCore2.Shared;
using ExileCore2.Shared.Enums;
using ExileCore2.Shared.Helpers;

namespace MinimapIcons.IconsBuilder.Icons;

public class IngameItemReplacerIcon : BaseIcon
{
    public IngameItemReplacerIcon(Entity entity, IconsBuilderSettings settings, MapIconsIndex mapIconsIndex)
        : base(entity)
    {
        Show = () => !entity.IsValid;

        var iconSizeMultiplier = RemoteMemoryObject.TheGame.Files.MinimapIcons.EntriesList.ElementAtOrDefault((int)mapIconsIndex)?.LargeMinimapSize ?? 1;
        MainTexture = new HudTexture("Icons.png")
        {
            UV = SpriteHelper.GetUV(mapIconsIndex),
            Size = RemoteMemoryObject.TheGame.IngameState.IngameUi.Map.LargeMapZoom * RemoteMemoryObject.TheGame.IngameState.Camera.Height * iconSizeMultiplier / 64,
        };
    }
}

public class IngameIconReplacerIcon : BaseIcon
{
    public IngameIconReplacerIcon(Entity entity, IconsBuilderSettings settings)
        : base(entity)
    {
        var isHidden = false;
        var transitionableFlag1 = 1;
        var shrineIsAvailable = true;
        var isOpened = false;

        T Update<T>(ref T store, Func<T> update)
        {
            return entity.IsValid ? store = update() : store;
        }

        Show = () => !Update(ref isHidden, () => entity.GetComponent<MinimapIcon>()?.IsHide ?? isHidden) &&
                     Update(ref transitionableFlag1, () => entity.GetComponent<Transitionable>()?.Flag1 ?? 1) == 1 &&
                     Update(ref shrineIsAvailable, () => entity.GetComponent<Shrine>()?.IsAvailable ?? shrineIsAvailable) &&
                     !Update(ref isOpened, () => entity.GetComponent<Chest>()?.IsOpened ?? isOpened) &&
                     !entity.IsValid;
        var name = entity.GetComponent<MinimapIcon>()?.Name ?? "";
        var iconIndexByName = ExileCore2.Shared.Helpers.Extensions.IconIndexByName(name);

        var iconSizeMultiplier = RemoteMemoryObject.TheGame.Files.MinimapIcons.EntriesList.ElementAtOrDefault((int)iconIndexByName)?.LargeMinimapSize ?? 1;
        MainTexture = new HudTexture("Icons.png")
        {
            UV = SpriteHelper.GetUV(iconIndexByName),
            Size = RemoteMemoryObject.TheGame.IngameState.IngameUi.Map.LargeMapZoom * RemoteMemoryObject.TheGame.IngameState.Camera.Height * iconSizeMultiplier / 64,
        };
    }
}