using Godot;
using System;

public partial class Player : CharacterBody3D
{
    [ExportGroup("Required Nodes")]
    [Export] private AnimationPlayer _animPlayerNode;
    [Export] private Sprite3D _spriteNode;
    [Export] private StateMachine _stateMachine;
    
    /// <summary>
    /// Animation player for this Character.
    /// </summary>
    public AnimationPlayer AnimationPlayer => _animPlayerNode;
    
    /// <summary>
    /// This Character current state machine.
    /// </summary>
    public StateMachine StateMachine => _stateMachine;
    
    public override void _Ready()
    {
        _stateMachine.SwitchState<PlayerIdleState>();
    }
    
    private Vector2 _direction = Vector2.Zero;
    
    /// <summary>
    /// Current direction of this Character.
    /// </summary>
    public Vector2 Direction => _direction;

    public override void _PhysicsProcess(double delta)
    {
        Velocity = new Vector3(_direction.X, 0, _direction.Y);
        Velocity *= 5;
        MoveAndSlide();
        Flip();
    }

    public override void _Input(InputEvent @event)
    {
        _direction =Input.GetVector(GameConstants.INPUT_MOVE_LEFT,
            GameConstants.INPUT_MOVE_RIGHT,
            GameConstants.INPUT_MOVE_FORWARD,
            GameConstants.INPUT_MOVE_BACKWARD);
    }

    private void Flip()
    {
        bool isMovingHorizontally = Velocity.X != 0;
        if (isMovingHorizontally)
        {
            bool isMovingLeft = _direction.X < 0;
            _spriteNode.FlipH = isMovingLeft;
        }
    }
}
