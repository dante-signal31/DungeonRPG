using Godot;

namespace DungeonRPG.Scripts.Characters.Enemy;

/// <summary>
/// This script moves its scene based on its steering behaviours nodes.
/// </summary>
public partial class AgentMover : Node
{
    [ExportCategory("WIRING:")] 
    [Export] private SteeringBehavior _steeringBehavior;
    [Export] private CharacterBody3D _characterBody;
    [Export] private EnemyStateMachine _stateMachine;
    
    public float MaximumSpeed { get; set; } = 2.0f;
    public float StopSpeed { get; set; }= 0.1f;
    public float MaximumRotationalSpeed { get; set; } = 10.0f;
    public float MaximumAcceleration { get; set; } = 10.0f;
    public float MaximumDeceleration { get; set; } = 10.0f;
    
    /// <summary>
    /// This agent target position.
    /// </summary>
    public Vector3 TargetPosition { get; set; }

    private float _arrivingRadius;

    public float ArrivingRadius
    {
        get => _arrivingRadius;
        set
        {
            _arrivingRadius = value;
            _steeringBehavior.ArrivingRadius = _arrivingRadius;
        }
    }
    
    /// <summary>
    /// This agent current speed
    /// </summary>
    public float CurrentSpeed { get; private set; }
    
    
    private SteeringBehaviorArgs _behaviorArgs;
    
    /// <summary>
    /// Create an initial set of SteeringBehaviorArgs for this agent.
    /// </summary>
    /// <returns>A set of initial SteeringBehaviorArgs.</returns>
    private SteeringBehaviorArgs GetSteeringBehaviorArgs()
    {
        return new SteeringBehaviorArgs(
            GetOwner<Node3D>(), 
            TargetPosition,
            _characterBody.Velocity, 
            MaximumSpeed, 
            StopSpeed,
            MaximumRotationalSpeed,
            MaximumAcceleration,
            MaximumDeceleration,
            0);
    }

    public override void _EnterTree()
    {
        _behaviorArgs = GetSteeringBehaviorArgs();
    }

    public override void _PhysicsProcess(double delta)
    {
        _behaviorArgs.TargetPosition = TargetPosition;
        _behaviorArgs.CurrentVelocity = _characterBody.Velocity;
        _behaviorArgs.DeltaTime = delta;
        SteeringOutput steeringOutput = _steeringBehavior.GetSteering(_behaviorArgs);
        _characterBody.Velocity = steeringOutput.Linear;
        CurrentSpeed = _characterBody.Velocity.Length();
        _characterBody.MoveAndSlide();
    }
}