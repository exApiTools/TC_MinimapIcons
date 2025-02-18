using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;

namespace MinimapIcons.IconsBuilder.Icons;

public class PlayerIcon : BaseIcon
{
    public PlayerIcon(Entity entity, IconsBuilderSettings settings, string playerName) :
        base(entity)
    {
        Show = () => entity.IsValid && !settings.HidePlayers;
        if (_HasIngameIcon) return;
        MainTexture = new HudTexture("Icons.png")
        {
            UV = SpriteHelper.GetUV(MapIconsIndex.OtherPlayer),
        };
        Text = playerName;
    }
}