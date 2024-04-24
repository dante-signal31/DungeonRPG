using Godot;

namespace DungeonRPG.Scripts.Characters.Enemy;

public partial class ArriveSteeringBehavior : SteeringBehavior
{
    [ExportCategory("CONFIGURATION:")] 
    [Export] private float _brakingRadius;
    [Export] private Curve _decelerationCurve;
    [Export] private float _accelerationRadius;
    [Export] private Curve _accelerationCurve;
    
    /// <summary>
    /// Distance to target point to consider it has been reached.
    /// </summary>
    public override float ArrivingRadius { get; set; }
    
    private Vector3 _startPosition;
    private float _distanceFromStart;
    private bool _idle = true;

    public override SteeringOutput GetSteering(SteeringBehaviorArgs args)
    {
        Vector3 targetPosition = args.TargetPosition;
        Vector3 currentPosition = args.Position;
        Vector3 currentVelocity = args.CurrentVelocity;
        float stopSpeed = args.StopSpeed;
        float maximumSpeed = args.MaximumSpeed;
        
        
        Vector3 toTarget = targetPosition - currentPosition;
        float distanceToTarget = toTarget.Length();

        float newSpeed = 0.0f;
        
        if (_idle && _distanceFromStart > 0) _distanceFromStart = 0;
        
        if (distanceToTarget >= ArrivingRadius && _distanceFromStart < _accelerationRadius)
        { // Acceleration phase.
            if (_idle)
            {
                _startPosition = currentPosition;
                _idle = false;
            }
            _distanceFromStart = (currentPosition - _startPosition).Length();
            newSpeed = maximumSpeed * 
                       _accelerationCurve.Sample(_distanceFromStart / _accelerationRadius);
        }
        else if (distanceToTarget < _brakingRadius && distanceToTarget >= ArrivingRadius)
        { // Deceleration phase.
            newSpeed = currentVelocity.Length() > stopSpeed?
                maximumSpeed * _decelerationCurve.Sample(1-(distanceToTarget / _brakingRadius)):
                0;
        }
        else if (distanceToTarget < ArrivingRadius)
        { // Stop phase.
            newSpeed = 0;
            _idle = true;
        }
        else
        { // Cruise speed phase.
            newSpeed = maximumSpeed;
        }
        
        Vector3 newVelocity = toTarget.Normalized() * newSpeed;
        
        return new SteeringOutput(newVelocity, 0);
    }
}