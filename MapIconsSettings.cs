using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;

namespace MinimapIcons
{
    public class MapIconsSettings : ISettings
    {
        [Menu("Draw Monster")]
        public ToggleNode DrawMonsters { get; set; } = new ToggleNode(true);

        [Menu("Z for text")]
        public RangeNode<float> ZForText { get; set; } = new RangeNode<float>(-10, -50, 50);

        public ToggleNode DrawOnlyOnLargeMap { get; set; } = new ToggleNode(true);
        public ToggleNode MultiThreading { get; set; } = new ToggleNode(false);
        public ToggleNode DrawNotValid { get; set; } = new ToggleNode(false);
        public ToggleNode DrawReplacementsForGameIconsWhenOutOfRange { get; set; } = new ToggleNode(true);
        public ToggleNode IgnoreFullscreenPanels { get; set; } = new ToggleNode(false);
        public ToggleNode IgnoreLargePanels { get; set; } = new ToggleNode(false);
        public ToggleNode Enable { get; set; } = new ToggleNode(true);
    }
}