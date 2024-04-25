using System.Linq;
using DungeonRPG.Scripts.Sensors;
using Godot;

namespace DungeonRPG.Scripts.Characters.Enemy;

public partial class PursuitBehavior : Node, INavigationBehavior
{
    [Signal] public delegate void TargetPositionChangedEventHandler(Vector3 newPosition);
    
    [ExportCategory("WIRING:")] 
    [Export] private VolumetricSensor _volumetricSensor;
    [Export] private Enemy _characterBody;

    [ExportCategory("CONFIGURATION:")] 
    // Degrees from forward vector inside which we consider an object is ahead.
    [Export(PropertyHint.Range, "0,90,1")] private float _aheadSemiConeDegrees = 29;
    // Degrees from forward vector inside which we consider an object is going toward us.
    [Export(PropertyHint.Range, "90,180,1")] private float _comingToUsSemiConeDegress = 160;
    
    public float ArrivingRadius { get; set; }
    
    private Vector3 _previousTargetPosition;
    private Vector3 _targetPosition;
    /// <summary>
    /// Position of the target we are pursuing.
    /// </summary>
    public Vector3 TargetPosition
    {
        get => _targetPosition;
        set
        {
            if (_previousTargetPosition != value)
            {
                _previousTargetPosition = value;
                _targetPosition = value;
                EmitSignal(SignalName.TargetPositionChanged, value);
            }
        }
    }

    private Player.Player _player;
    private float _cosAheadSemiConeRadians;
    private float _cosComingToUsSemiConeRadians;

    public override void _Ready()
    {
        _cosAheadSemiConeRadians = Mathf.Cos( Mathf.DegToRad(_aheadSemiConeDegrees));
        _cosComingToUsSemiConeRadians = Mathf.Cos(Mathf.DegToRad(_comingToUsSemiConeDegress));
    }
    
    public void UpdateTargetPosition()
    {
        if (_volumetricSensor.DetectedBodies.Count == 0) return;
        // There will be only one player in the game, so volumetric sensor will have only one
        // element when it detects anything in the player layer.
        _player = (Player.Player) _volumetricSensor.DetectedBodies.First();
        Vector3 playerPosition = _player.GlobalPosition;
        float playerSpeed = _player.Velocity.Length();
        
        if (TargetIsComingToUs() || Mathf.IsZeroApprox(playerSpeed))
        {
            // Target is coming to us so just go straight to it.
            TargetPosition = playerPosition;
        }
        else
        {
            // Target is not coming to us so we must predict where it will be.
            // The look-ahead time is proportional to the distance between the evader
            // and the pursuer and is inversely proportional to the sum of the
            // agents velocities.
            Vector3 currentPosition = _characterBody.GlobalPosition;
            float currentSpeed = _characterBody.Velocity.Length();
            Vector3 playerVelocity = _player.Velocity;
            float distanceToTarget = (playerPosition - currentPosition).Length();
            float lookAheadTime = distanceToTarget / (playerSpeed + currentSpeed);
            // When both agents are stopped lookAheadTime will return infinity, so we'd
            // better check that. 
            if (float.IsInfinity(lookAheadTime))
            {
                TargetPosition = playerPosition;
            }
            else
            {
                TargetPosition = playerPosition + playerVelocity * lookAheadTime;
            }
        }
    }

    private bool TargetIsComingToUs()
    {
        Vector3 currentPosition = _characterBody.GlobalPosition;
        Vector3 currentVelocityNormalized = _characterBody.Velocity.Normalized();
        Vector3 targetVelocityNormalized = _player.Velocity.Normalized();
        Vector3 toTargetNormalized = (TargetPosition - currentPosition).Normalized();

        bool targetInFrontOfUs = currentVelocityNormalized.Dot(toTargetNormalized) > _cosAheadSemiConeRadians;
        bool targetComingToUs = currentVelocityNormalized.Dot(targetVelocityNormalized) < _cosComingToUsSemiConeRadians;

        return targetInFrontOfUs && targetComingToUs;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_volumetricSensor.DetectedBodies.Count > 0)
        {
            UpdateTargetPosition();
        }
    }
}