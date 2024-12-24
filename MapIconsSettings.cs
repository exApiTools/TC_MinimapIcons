using System.Collections.Generic;
using ExileCore2.Shared.Attributes;
using ExileCore2.Shared.Interfaces;
using ExileCore2.Shared.Nodes;
using MinimapIcons.IconsBuilder;

namespace MinimapIcons;

public class MapIconsSettings : ISettings
{
    public ToggleNode DrawMonsters { get; set; } = new ToggleNode(true);
    public RangeNode<float> ZForText { get; set; } = new RangeNode<float>(-10, -50, 50);
    public ToggleNode DrawOnlyOnLargeMap { get; set; } = new ToggleNode(true);
    public ToggleNode DrawNotValid { get; set; } = new ToggleNode(false);
    public ToggleNode DrawReplacementsForGameIconsWhenOutOfRange { get; set; } = new ToggleNode(true);
    public ToggleNode IgnoreFullscreenPanels { get; set; } = new ToggleNode(false);
    public ToggleNode IgnoreLargePanels { get; set; } = new ToggleNode(false);
    public ToggleNode Enable { get; set; } = new ToggleNode(true);
    public RangeNode<int> IconListRefreshPeriod { get; set; } = new RangeNode<int>(100, 0, 1000);

    [Menu(null, CollapsedByDefault = true)]
    public ContentNode<TextNode> AlwaysShownIngameIcons { get; set; } =
        new ContentNode<TextNode>()
        {
            Content =
            [
                new TextNode("Metadata/MiscellaneousObjects/Breach/BreachObject"),
                new TextNode("Metadata/Terrain/Leagues/Ritual/RitualRuneInteractable"),
            ],
            EnableControls = true, 
            ItemFactory = () => new TextNode(""),
            UseFlatItems = true,
        };

    public IconsBuilderSettings IconsBuilderSettings { get; set; } = new();
}