using DungeonRPG.Scripts.General;
using Godot;

namespace DungeonRPG.Scripts.Characters.Player;

public partial class PlayerDashState: PlayerState
{
    [Export] private Timer _dashTimerNode;
    [Export(PropertyHint.Range, "0, 20, 0.1")] private float _speed = 10.0f;
    
    
    public override void _Ready()
    {
        base._Ready();
        _dashTimerNode.Timeout += _OnDashTimerTimeout;
    }

    protected override void EnterState()
    {
        _characterNode.AnimationPlayer.Play(GameConstants.ANIM_DASH);
    }

    public override void _ExitTree()
    {
        _dashTimerNode.Timeout -= _OnDashTimerTimeout;
    }

    public override void _PhysicsProcess(double delta)
    {
        // If PhysicsProcess is called, that means this state is the current one,
        // so if the dash timer has not been started, start it.
        if (_dashTimerNode.IsStopped())
        {
            _dashTimerNode.Start();
            if (_characterNode.Velocity == Vector3.Zero)
            {
                _characterNode.Velocity = _characterNode.IsFacingLeft ?
                    new Vector3(-_speed, 0, 0) : new Vector3(_speed, 0, 0);
            }
            else
            {
                _characterNode.Velocity = _characterNode.Velocity.Normalized() * _speed;
            }
        }
        _characterNode.MoveAndSlide();
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustReleased(GameConstants.INPUT_DASH))
        {
            FinishDash();
        }
    }

    private void FinishDash()
    {
        _characterNode.Velocity = Vector3.Zero;
        _characterNode.StateMachine.SwitchState<DungeonRPG.Scripts.Characters.Player.PlayerIdleState>();
    }
    
    private void _OnDashTimerTimeout()
    {
        FinishDash();
    }
}