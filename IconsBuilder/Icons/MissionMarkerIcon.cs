using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared;
using ExileCore.Shared.Helpers;
using GameOffsets.Native;

namespace MinimapIcons.IconsBuilder.Icons;

public class MissionMarkerIcon : BaseIcon
{
    public MissionMarkerIcon(Entity entity, IconsBuilderSettings settings) : base(entity)
    {
        MainTexture = new HudTexture();
        MainTexture.FileName = "Icons.png";
        MainTexture.UV = SpriteHelper.GetUV(16, new Vector2i(14, 14));

        Show = () =>
        {
            var switchState = entity.GetComponent<Transitionable>() != null
                ? entity.GetComponent<Transitionable>().Flag1
                : (byte?) null;

            var isTargetable = entity.IsTargetable;
            return switchState == 1 || isTargetable;
        };

        MainTexture.Size = settings.SizeMiscIcon;
    }
}