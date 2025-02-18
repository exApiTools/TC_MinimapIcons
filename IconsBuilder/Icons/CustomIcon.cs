using ExileCore.PoEMemory;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared;
using ExileCore.Shared.Helpers;

namespace MinimapIcons.IconsBuilder.Icons;

public class CustomIcon : BaseIcon
{
    public CustomIcon(Entity entity, IconsBuilderSettings settings, CustomIconSettings customIconSettings)
        : base(entity)
    {
        Show = () => true;
        MainTexture = new HudTexture("Icons.png")
        {
            UV = SpriteHelper.GetUV(customIconSettings.Icon),
            Size = RemoteMemoryObject.pTheGame.IngameState.IngameUi.Map.LargeMapZoom * RemoteMemoryObject.pTheGame.IngameState.Camera.Height / 64 * customIconSettings.Size.Value,
            Color = customIconSettings.Tint.Value.ToSystem(),
        };
    }
}