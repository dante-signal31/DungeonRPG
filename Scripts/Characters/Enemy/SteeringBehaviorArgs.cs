﻿using Godot;

namespace DungeonRPG.Scripts.Characters.Enemy;

/// <summary>
/// Base class passed as argument to Steering Behaviors Process() methods.
/// </summary>
public class SteeringBehaviorArgs
{
    /// <summary>
    /// Owner of this steering.
    /// </summary>
    public Node3D CurrentAgent { get; private set; }
    
    /// <summary>
    /// Target position for this steering.
    /// </summary>
    public Vector3 TargetPosition { get; set; }
    
    /// <summary>
    /// Maximum linear speed for this steering.
    /// </summary>
    public float MaximumSpeed { get; private set; }
    
    /// <summary>
    /// Minimum linear speed under which agent is considered stopped.
    /// </summary>
    public float StopSpeed { get; private set; }
    
    /// <summary>
    /// Current owner velocity vector.
    /// </summary>
    public Vector3 CurrentVelocity { get; set; }
    
    /// <summary>
    /// Maximum rotational speed for this steering.
    /// </summary>
    public float MaximumRotationalSpeed { get; private set; }
    
    /// <summary>
    /// Maximum acceleration for this steering.
    /// </summary>
    public float MaximumAcceleration { get; private set; }
    
    /// <summary>
    /// Maximum deceleration for this steering.
    /// </summary>
    public float MaximumDeceleration { get; private set; }
    
    /// <summary>
    /// Delta time since last steering behavior update.
    /// </summary>
    public double DeltaTime { get; set; }

    /// <summary>
    /// This GameObject position.
    /// </summary>
    public Vector3 Position => CurrentAgent.GlobalPosition;

    /// <summary>
    /// This GameObject rotation.
    /// </summary>
    public float Orientation => CurrentAgent.Rotation.Z;

    public SteeringBehaviorArgs(Node3D currentAgent, Vector3 targetPosition, Vector3 currentVelocity,
        float maximumSpeed, float stopSpeed, float maximumRotationalSpeed, float maximumAcceleration,
        float maximumDeceleration, float deltaTime)
    {
        CurrentVelocity = currentVelocity;
        TargetPosition = targetPosition;
        MaximumSpeed = maximumSpeed;
        StopSpeed = stopSpeed;
        MaximumRotationalSpeed = maximumRotationalSpeed;
        CurrentAgent = currentAgent;
        MaximumAcceleration = maximumAcceleration;
        MaximumDeceleration = maximumDeceleration;
        DeltaTime = deltaTime;
    }
}
