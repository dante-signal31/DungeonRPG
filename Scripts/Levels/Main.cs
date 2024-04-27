using Godot;

namespace DungeonRPG.Scripts.Levels;

public partial class Main : Node3D
{
    public override void _Ready()
    {
        GetTree().Paused = true;
    }
}