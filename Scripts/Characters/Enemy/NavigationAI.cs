using Godot;

namespace DungeonRPG.Scripts.Characters.Enemy;

public partial class NavigationAI : Node
{
    /// <summary>
    /// Signal a new navigation target position has been set.
    /// </summary>
    [Signal] public delegate void TargetPositionChangedEventHandler(Vector3 newTargetPosition);
    
    [ExportCategory("WIRING:")] 
    [Export] private EnemyStateMachine _stateMachine;
    [Export] private PatrolBehavior _patrolBehavior;
    [Export] private PursuitBehavior _pursuitBehavior;

    /// <summary>
    /// Current target position in global space designated by the navigation AI.
    /// </summary>
    public Vector3 TargetPosition { get; private set; }

    private float _arrivingRadius;

    /// <summary>
    /// Distance to target to get it as reached.
    /// </summary>
    public float ArrivingRadius
    {
        get=> _arrivingRadius;
        set
        {
            _patrolBehavior.ArrivingRadius = value;
            _pursuitBehavior.ArrivingRadius = value;
            _arrivingRadius = value;
        }
    }

    private EnemyStateMachine.EnemyStates _currentState;
    private INavigationBehavior _currentNavigationBehavior;
    
    public override void _EnterTree()
    { 
        _stateMachine.StateChanged += OnStateChanged;
        _patrolBehavior.TargetPositionChanged += OnTargetPositionChanged;
    }

    public override void _ExitTree()
    {
        _stateMachine.StateChanged -= OnStateChanged;
        _patrolBehavior.TargetPositionChanged -= OnTargetPositionChanged;
    }

    public override void _Ready()
    {
        _currentState = _stateMachine.CurrentState;
        UpdateNavigationBehavior();
        UpdateTargetPosition();
    }

    /// <summary>
    /// Reload target position from current navigation behavior and emit a signal if
    /// the target position has changed.
    /// </summary>
    private void UpdateTargetPosition()
    {
        if (TargetPosition != _currentNavigationBehavior.TargetPosition)
        {
            TargetPosition = _currentNavigationBehavior.TargetPosition;
            EmitSignal(SignalName.TargetPositionChanged, TargetPosition);
        }
    }

    private void OnStateChanged(EnemyStateMachine.EnemyStates newState)
    {
        _currentState = newState;
        UpdateNavigationBehavior();
        UpdateTargetPosition();
    }

    /// <summary>
    /// Update current navigation behavior based on current state.
    /// </summary>
    private void UpdateNavigationBehavior()
    {
        _currentNavigationBehavior = _currentState switch
        {
            EnemyStateMachine.EnemyStates.Patrol => _patrolBehavior,
            EnemyStateMachine.EnemyStates.Pursuit => _pursuitBehavior,
            _ => null
        };
    }

    private void OnTargetPositionChanged(Vector3 _)
    {
        UpdateTargetPosition();
    }
    
    
}