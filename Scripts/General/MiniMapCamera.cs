using DungeonRPG.Scripts.Characters.Player;
using Godot;

namespace DungeonRPG.Scripts.General;

public partial class MiniMapCamera : Camera3D
{
    [ExportCategory("WIRING:")] 
    [Export] public Player Player { get; set; }

    public override void _Process(double delta)
    {
        GlobalPosition = Player.GlobalPosition with {Y = GlobalPosition.Y};
    }
}