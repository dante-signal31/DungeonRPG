using Godot;

namespace DungeonRPG.Scripts.Characters.Enemy;

public abstract partial class SteeringBehavior : Node
{
    /// <summary>
    /// Distance to target to get it as reached.
    /// </summary>
    public abstract float ArrivingRadius { get; set; }
    
    /// <summary>
    /// Get new steering as a tuple of linear acceleration and angular one.
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public abstract SteeringOutput GetSteering(SteeringBehaviorArgs args);
}

