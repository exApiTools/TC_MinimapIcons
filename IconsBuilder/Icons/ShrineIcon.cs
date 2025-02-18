using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;

namespace MinimapIcons.IconsBuilder.Icons;

public class ShrineIcon : BaseIcon
{
    public ShrineIcon(Entity entity, IconsBuilderSettings settings) : base(entity)
    {
        MainTexture = new HudTexture("Icons.png");
        MainTexture.UV = SpriteHelper.GetUV(MapIconsIndex.Shrine);
        Text = entity.GetComponent<Render>()?.Name;
        MainTexture.Size = settings.SizeShrineIcon;
        Show = () => entity.IsValid && entity.GetComponent<Shrine>().IsAvailable;
    }
}