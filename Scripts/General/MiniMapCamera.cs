using DungeonRPG.Scripts.Characters.Player;
using Godot;

namespace DungeonRPG.Scripts.General;

public partial class MiniMapCamera : Camera3D
{
    [ExportCategory("WIRING:")] 
    [Export] private Player _player;

    public override void _Process(double delta)
    {
        GlobalPosition = _player.GlobalPosition with {Y = GlobalPosition.Y};
    }
}