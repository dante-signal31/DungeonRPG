using Godot;

namespace DungeonRPG.Scripts.Characters;

public partial class Hurtbox : Area3D
{
    
    public override void _EnterTree()
    {
        AreaEntered += OnAreaEntered;
        AreaExited += OnAreaExited;
    }

    public override void _ExitTree()
    {
        AreaEntered -= OnAreaEntered;
        AreaExited -= OnAreaExited;
    }

    private void OnAreaEntered(Area3D area)
    {
        // GD.Print($"[{GetOwner<Node>().Name}] Under attack from {area.GetOwner<Node>().Name}");
    }

    private void OnAreaExited(Area3D area)
    {
        // GD.Print($"[{GetOwner<Node>().Name}] {area.GetOwner<Node>().Name} ceases his attack.");
    }
}