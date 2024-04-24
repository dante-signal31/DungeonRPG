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
    [Export] private NavigationAgent3D _navigationAgent;
    [Export] private PatrolBehavior _patrolBehavior;
    [Export] private PursuitBehavior _pursuitBehavior;

    private Vector3 _finalTargetPosition;
    /// <summary>
    /// Current target position in global space designated by the navigation AI.
    /// </summary>
    public Vector3 FinalTargetPosition
    {
        get => _finalTargetPosition;
        private set
        {
            _navigationAgent.TargetPosition = value;
            _finalTargetPosition = value;
        } 
    }

    public Vector3 _nextPositionToReachTarget;

    /// <summary>
    /// Next position in the path to get to the target.
    /// </summary>
    public Vector3 NextPositionToReachTarget
    {
        get => _nextPositionToReachTarget;
        private set
        {
            if (_nextPositionToReachTarget != value)
            {
                _nextPositionToReachTarget = value;
                EmitSignal(SignalName.TargetPositionChanged, _nextPositionToReachTarget);
            }            
        }
    }

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
    private bool _initialSynchronizationDone = false;
    
    public override void _EnterTree()
    { 
        _stateMachine.StateChanged += OnStateChanged;
    }

    public override void _ExitTree()
    {
        _stateMachine.StateChanged -= OnStateChanged;
    }

    public override void _Ready()
    {
        _currentState = _stateMachine.CurrentState;
        UpdateNavigationBehavior();
        
        // Make sure to not await during _Ready.
        Callable.From(ActorSetup).CallDeferred();
    }

    /// <summary>
    /// Reload target position from current navigation behavior and emit a signal if
    /// the target position has changed.
    /// </summary>
    private void UpdateTargetPosition()
    { 
        _currentNavigationBehavior.UpdateTargetPosition();
        FinalTargetPosition = _currentNavigationBehavior.TargetPosition;
    }

    private void OnStateChanged(EnemyStateMachine.EnemyStates newState)
    {
        _currentState = newState;
        UpdateNavigationBehavior();
        UpdateTargetPosition();
    }

    /// <summary>
    /// Select current navigation behavior based on current state.
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

    private async void ActorSetup()
    {
        // Wait for the first physics frame so the NavigationServer can sync.
        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
        
        // Now that the navigation map is no longer empty, set the movement target.
        UpdateTargetPosition();
    }

    public override void _PhysicsProcess(double _)
    {
        if (_navigationAgent.IsNavigationFinished())
        {
            UpdateTargetPosition();
        }
        else
        {
            NextPositionToReachTarget = _navigationAgent.GetNextPathPosition();
        }
    }
}