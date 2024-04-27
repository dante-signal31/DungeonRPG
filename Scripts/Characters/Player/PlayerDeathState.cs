using DungeonRPG.Scripts.General;
using Godot;

namespace DungeonRPG.Scripts.Characters.Player;

public partial class PlayerDeathState : PlayerState
{
    protected override void EnterState()
    {
        _characterNode.AnimationPlayer.Play(GameConstants.ANIM_DEATH);
        
        _characterNode.AnimationPlayer.AnimationFinished += OnAnimationFinished;
    }
    
    public override void _ExitTree()
    {
        base._ExitTree();
        _characterNode.AnimationPlayer.AnimationFinished -= OnAnimationFinished;
    }

    private void OnAnimationFinished(StringName animname)
    {
        _characterNode.QueueFree();
    }

    protected override void ExitState()
    {
        throw new System.NotImplementedException();
    }
}