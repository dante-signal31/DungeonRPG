using Godot;

namespace DungeonRPG.Scripts.Characters.Enemy;

public partial class PatrolBehavior : Node, INavigationBehavior
{
    /// <summary>
    /// Signal a new navigation target position has been set.
    /// </summary>
    [Signal] public delegate void TargetPositionChangedEventHandler(Vector3 newTargetPosition);
    
    private Vector3 _currentTargetPosition;
    /// <summary>
    /// This behavior calculated target.
    /// </summary>
    public Vector3 TargetPosition => _currentTargetPosition;
    
    /// <summary>
    /// Distance to target point to consider it has been reached.
    /// </summary>
    public float ArrivingRadius { get; set; }
    
    public Path3D PatrolPath { get; set; }
    
    
    
    private int _currentPatrolPointIndex = 0;
    private int _totalPatrolPoints = 0;
    private Node3D _characterNode;
    

    public override void _Ready()
    {
        _totalPatrolPoints = PatrolPath.Curve.PointCount;
        _characterNode = GetOwner<Node3D>();
        // _currentTargetPosition = GetNextPatrolPoint();
        // UpdateTargetPosition();
    }

    /// <summary>
    /// Get next patrol point in the patrol path.
    ///
    /// After last point has been returned, it will return the first point.
    /// </summary>
    /// <returns>Next patrol point position in global space.</returns>
    private Vector3 GetNextPatrolPoint()
    {
        Vector3 nextPatrolPoint = PatrolPath.Curve.GetPointPosition(_currentPatrolPointIndex);
        Vector3 nextPatrolPointGlobalSpace = PatrolPath.ToGlobal(nextPatrolPoint);
        _currentPatrolPointIndex = (_currentPatrolPointIndex) >= _totalPatrolPoints-1 ? 0: _currentPatrolPointIndex+1;
        return nextPatrolPointGlobalSpace;
    }
    
    
    /// <summary>
    /// Get current patrol point in global space.
    /// </summary>
    /// <returns>Current patron point position in global space.</returns>
    private Vector3 GetCurrentPatrolPoint()
    {
        Vector3 currentPatrolPoint = PatrolPath.Curve.GetPointPosition(_currentPatrolPointIndex);
        Vector3 currentPatrolPointGlobalSpace = PatrolPath.ToGlobal(currentPatrolPoint);
        return currentPatrolPointGlobalSpace;
    }

    /// <summary>
    /// Update target position with the next point in patrol path.
    /// </summary>
    public void UpdateTargetPosition()
    {
        _currentTargetPosition = GetNextPatrolPoint();
        // EmitSignal(SignalName.TargetPositionChanged, _currentTargetPosition);
    }
    
    // public override void _PhysicsProcess(double _)
    // {
    //     if (_characterNode.GlobalPosition.DistanceTo(_currentTargetPosition)<=ArrivingRadius)
    //     {
    //         _currentTargetPosition = GetNextPatrolPoint();
    //         EmitSignal(SignalName.TargetPositionChanged, _currentTargetPosition);
    //     }
    // }
}