using Godot;

namespace DungeonRPG.Scripts.Characters.Enemy;

public partial class Enemy : CharacterBody3D
{
    [ExportCategory("WIRING:")]
    [Export] private Sprite3D _spriteNode;
    
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
    [Export] private Path3D _patrolPath;
    
    /// <summary>
    /// Is this Character facing left?
    /// </summary>
    public bool IsFacingLeft { get; private set; }
    
    private EnemyStateMachine _stateMachine;
    private PatrolBehavior _patrolBehavior;
    
    public Vector3 Forward { get; private set; } = Vector3.Right;
    
    public override void _EnterTree()
    {
        _stateMachine = GetNode<EnemyStateMachine>("StateMachine");
        _patrolBehavior = GetNode<PatrolBehavior>("AI/Navigation/Patrol");
        _patrolBehavior.PatrolPath = _patrolPath;
        _stateMachine.DefaultState = DefaultState;
    }

    public override void _Process(double delta)
    {
        Flip();
    }

    public void Flip()
    {
        bool isMovingHorizontally = Velocity.X != 0;
        if (isMovingHorizontally)
        {
            IsFacingLeft = Velocity.X < 0;
            _spriteNode.FlipH = IsFacingLeft;
        }
    }
}