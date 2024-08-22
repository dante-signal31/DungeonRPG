using DungeonRPG.Scripts.Characters.Player;
using Godot;

namespace DungeonRPG.Scripts.General;

public partial class MiniMap : PanelContainer
{
    [ExportCategory("CONFIGURATION:")]
    [Export] private Player _player;
    
    public override void _Ready()
    {
        base._Ready();
        GetNode<MiniMapCamera>(
            "MiniMapMask/MiniMapViewportContainer/MiniMapViewport/Camera3D").Player = _player;
    }

    private void OnMapShown()
    {
        Visible = false;
    }
    
    private void OnMapHidden()
    {
        Visible = true;
    }
}