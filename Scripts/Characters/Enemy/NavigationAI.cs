using Godot;
using Godot.Collections;

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
    [Export] private Enemy _characterNode;
    [Export] private Timer _timer;

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
                // EmitSignal(SignalName.TargetPositionChanged, _nextPositionToReachTarget);
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
    private bool _newWaypointNeeded = true;
    private bool _newPursuitTargetPosition = true;
    private RandomNumberGenerator _rng;
    private bool _navigationReady = false;
    
    public override void _EnterTree()
    { 
        _stateMachine.StateChanged += OnStateChanged;
        _timer.Timeout += GetNextPatrolPoint;
        _navigationAgent.WaypointReached += OnWaypointReached;
        _pursuitBehavior.TargetPositionChanged += OnPursuitTargetPositionChanged;
        _rng = new RandomNumberGenerator();
    }

    public override void _ExitTree()
    {
        _stateMachine.StateChanged -= OnStateChanged;
        _timer.Timeout -= GetNextPatrolPoint;
        _navigationAgent.WaypointReached -= OnWaypointReached;
        _pursuitBehavior.TargetPositionChanged -= OnPursuitTargetPositionChanged;
    }

    public override void _Ready()
    {
        _timer.WaitTime = _rng.RandfRange(0, _characterNode.PatrolWaitingTime);
        _currentState = _stateMachine.CurrentState;
        UpdateNavigationBehavior();
        
        // Make sure to not await during _Ready.
        Callable.From(NavigationSetup).CallDeferred();
        
    }

    private async void NavigationSetup()
    {
        // Some code flow call this method in the Ready step. Problem is that any call
        // to the navigation agent should be made after the first frame, to let navigation
        // server to prepare itself. So we wait for the first physics frame.
        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
        _navigationReady = true;
        UpdateTargetPosition();
    }

    /// <summary>
    /// Reload target position from current navigation behavior and emit a signal if
    /// the target position has changed.
    /// </summary>
    private void UpdateTargetPosition()
    { 
        // // Some code flow call this method in the Ready step. Problem is that any call
        // // to the navigation agent should be made after the first frame, to let navigation
        // // server to prepare itself. So we wait for the first physics frame.
        // await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
        
        // Now that the navigation map is no longer empty, set the movement target.
        if (_currentNavigationBehavior != null)
        {
            _currentNavigationBehavior.UpdateTargetPosition();
            FinalTargetPosition = _currentNavigationBehavior.TargetPosition;
        }
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

    /// <summary>
    /// This method is called by the timer after waiting in a patrol path waypoint. It returns
    /// next waypoint and resumes patrolling. This way the enemy waits a moment in every waypoint.
    /// </summary>
    public void GetNextPatrolPoint()
    {
        _stateMachine.SwitchState(EnemyStateMachine.EnemyStates.Patrol);
        UpdateTargetPosition();
        _timer.Stop();
        _timer.WaitTime = _rng.RandfRange(0, _characterNode.PatrolWaitingTime);
    }

    public void OnWaypointReached(Dictionary _)
    {
        // Don't call _navigationAgent.GetNextPathPosition() from here or you will
        // end in an endless loop.
        _newWaypointNeeded = true;
    }

    public void OnPursuitTargetPositionChanged(Vector3 position)
    {
        _newPursuitTargetPosition = true;
    }

    public override void _PhysicsProcess(double _)
    {
        if (!_navigationReady) return;
        switch (_stateMachine.CurrentState)
        {
            case EnemyStateMachine.EnemyStates.Patrol:
                if (_navigationAgent.IsNavigationFinished())
                {

                    _stateMachine.SwitchState(EnemyStateMachine.EnemyStates.Idle);
                    _timer.Start();
                }
                else if (_newWaypointNeeded)
                {
                    NextPositionToReachTarget = _navigationAgent.GetNextPathPosition();
                    _newWaypointNeeded = false;
                }
                break;
            case EnemyStateMachine.EnemyStates.Pursuit:
                if (_newPursuitTargetPosition)
                {
                    UpdateTargetPosition();
                    _newPursuitTargetPosition = false;
                }
                else
                {
                    if (_navigationAgent.IsNavigationFinished())
                    {
                        // TODO: Here, enemy should fight.
                    }
                    else if (_newWaypointNeeded)
                    {
                        NextPositionToReachTarget = _navigationAgent.GetNextPathPosition();
                        _newWaypointNeeded = false;
                    }
                }
                break;
        } 
    }
}