using DungeonRPG.Scripts.General;
using Godot;

namespace DungeonRPG.Scripts.Characters.Player;

public partial class PlayerMoveState : PlayerState
{
    [Export(PropertyHint.Range, "0,20,0.1")] private float _speed = 5.0f;
    
    
    public override void _PhysicsProcess(double delta)
    {
        if (_characterNode.Direction == Vector2.Zero)
        {
            _characterNode.StateMachine.SwitchState<PlayerIdleState>();
            return;
        }
        
        _characterNode.Velocity = new Vector3(_characterNode.Direction.X, 
            0, _characterNode.Direction.Y);
        _characterNode.Velocity *= _speed;
        _characterNode.MoveAndSlide();
        _characterNode.Flip();
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed(GameConstants.INPUT_DASH))
        {
            _characterNode.StateMachine.SwitchState<PlayerDashState>();
        }
    }

    protected override void EnterState()
    {
        _characterNode.AnimationPlayer.Play(GameConstants.ANIM_MOVE);
    }
}