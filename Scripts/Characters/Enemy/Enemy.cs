using DungeonRPG.Scripts.Sensors;
using Godot;

namespace DungeonRPG.Scripts.Characters.Enemy;

public partial class Enemy : Characters.Character
{
    [ExportCategory("WIRING:")]
    [Export] private Sprite3D _spriteNode;
    [Export] private AgentMover _agentMover;
    
    [ExportCategory("CONFIGURATION:")] 
    private EnemyStateMachine.EnemyStates _defaultState = EnemyStateMachine.EnemyStates.Idle;
    [Export] private EnemyStateMachine.EnemyStates DefaultState
    {
        get => _defaultState;
        set
        {
            if (_defaultState == value) return;
            _defaultState = value;
            if (_stateMachine != null) _stateMachine.DefaultState = value;
        }
    }

    [ExportGroup("Movement")] 
    private float _maximumSpeed;
    [Export] private float MaximumSpeed
    {
        get => _agentMover.MaximumSpeed;
        set
        {
            if (_agentMover != null) _agentMover.MaximumSpeed = value;
            _maximumSpeed = value;
        }
    }

    private float _stopSpeed;
    [Export] private float StopSpeed
    {
        get => _agentMover.StopSpeed;
        set
        {
            if (_agentMover != null)_agentMover.StopSpeed = value;
            _stopSpeed = value;
        }
    }

    private float _maximumRotationalSpeed;
    [Export] private float MaximumRotationalSpeed
    {
        get => _agentMover.MaximumRotationalSpeed;
        set
        {
            if (_agentMover != null) _agentMover.MaximumRotationalSpeed = value;
            _maximumRotationalSpeed = value;
        }
    }

    private float _maximumAcceleration;
    [Export] private float MaximumAcceleration
    {
        get => _agentMover.MaximumAcceleration;
        set
        {
            if (_agentMover != null) _agentMover.MaximumAcceleration = value;
            _maximumAcceleration = value;
        }
    }

    private float _maximumDeceleration;
    [Export] private float MaximumDeceleration
    {
        get => _agentMover.MaximumDeceleration;
        set
        {
            if (_agentMover != null)_agentMover.MaximumDeceleration = value;
            _maximumDeceleration = value;
        }
    }

    [ExportGroup("Patrol behavior")]
    [Export] private Path3D _patrolPath;
    [Export] private float _patrolWaitingTime = 2.0f;


    /// <summary>
    /// Is this Character facing left?
    /// </summary>
    public bool IsFacingLeft
    {
        get => _spriteNode.FlipH;
        set => Flip(value);
    }
    
    private EnemyStateMachine _stateMachine;
    private PatrolBehavior _patrolBehavior;
    
    public Vector3 Forward { get; private set; } = Vector3.Right;
    
    /// <summary>
    /// Time to wait idle before moving again after reaching a patrol checkpoint.
    /// </summary>
    public float PatrolWaitingTime => _patrolWaitingTime;
    
    public override void _EnterTree()
    {
        base._EnterTree();
        _stateMachine = GetNode<EnemyStateMachine>("StateMachine");
        _patrolBehavior = GetNode<PatrolBehavior>("AI/Navigation/Patrol");
        _patrolBehavior.PatrolPath = _patrolPath;
        _stateMachine.DefaultState = DefaultState;
    }

    public override void _Ready()
    {
        _agentMover.MaximumSpeed = _maximumSpeed;
        _agentMover.StopSpeed = _stopSpeed;
        _agentMover.MaximumRotationalSpeed = _maximumRotationalSpeed;
        _agentMover.MaximumAcceleration = _maximumAcceleration;
        _agentMover.MaximumDeceleration = _maximumDeceleration;
    }

    public override void _Process(double delta)
    {
        if (Velocity.X != 0)
        {
            IsFacingLeft = (Velocity.X < 0);
        }
    }

    public void Flip(bool isFacingLeft)
    {
        _spriteNode.FlipH = isFacingLeft;
    }
}