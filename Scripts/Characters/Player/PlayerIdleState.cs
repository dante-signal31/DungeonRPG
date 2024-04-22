using DungeonRPG.Scripts.General;
using Godot;

namespace DungeonRPG.Scripts.Characters.Player;

public partial class PlayerIdleState : PlayerState
{
    public override void _PhysicsProcess(double delta)
    {
        if (_characterNode.Direction != Vector2.Zero)
        {
            _characterNode.StateMachine.SwitchState<PlayerMoveState>();
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed((GameConstants.INPUT_DASH)))
        {
            _characterNode.StateMachine.SwitchState<PlayerDashState>();
        }
    }

    protected override void EnterState()
    {
        _characterNode.AnimationPlayer.Play(GameConstants.ANIM_IDLE);
    }
}