using Godot;

namespace DungeonRPG.Scripts.Characters.Enemy;

public partial class Enemy : CharacterBody3D
{
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

    public override void _Ready()
    {
        
    }
}