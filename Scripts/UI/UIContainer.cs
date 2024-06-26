using Godot;

namespace DungeonRPG.Scripts.UI;

public partial class UIContainer : Container
{
    [Export] public ContainerType container { get; private set; }
    [Export] public Button ButtonNode { get; private set; }
}