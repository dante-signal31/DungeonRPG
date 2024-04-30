using DungeonRPG.Scripts.General;
using Godot;

namespace DungeonRPG.Scripts.Abilities;

public partial class Bomb : Node3D, IDamager
{
    [ExportCategory("WIRING:")]
    [Export] private AnimationPlayer _animationPlayerNode;

    [ExportCategory("CONFIGURATION:")] 
    [Export] public float Damage { get; private set; }

    public override void _Ready()
    {
        base._Ready();
        _animationPlayerNode.AnimationFinished += OnAnimationFinished;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        _animationPlayerNode.AnimationFinished -= OnAnimationFinished;
    }

    private void OnAnimationFinished(StringName animname)
    {
        if (animname == GameConstants.ANIM_EXPAND)
        {
            _animationPlayerNode.Play(GameConstants.ANIM_EXPLOSION);
        }
        else
        {
            QueueFree();
        }
    }
}