using Godot;
using System;

public partial class PlayerIdleState : Node
{
    private Player _characterNode;
    
    public override void _Ready()
    {
        _characterNode = GetOwner<Player>();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_characterNode.Direction != Vector2.Zero)
        {
            _characterNode.StateMachine.SwitchState<PlayerMoveState>();
        }
    }

    public override void _Notification(int what)
    {
        base._Notification(what);

        if (what == 5001)
        {
            _characterNode.AnimationPlayer.Play(GameConstants.ANIM_IDLE);
        }
    }
}
