using System.Collections.Generic;
using Godot;

namespace DungeonRPG.Scripts.Sensors;

public partial class VolumetricSensor : Area3D
{
    [ExportCategory("CONFIGURATION:")]
    [Export(PropertyHint.Layers3DPhysics)]
    private uint SensorLayers
    {
        get => CollisionLayer;
        set => CollisionLayer = value;
    }

    [Export(PropertyHint.Layers3DPhysics)]
    private uint DetectedLayers
    {
        get => CollisionMask;
        set => CollisionMask = value;
    }
    
    /// <summary>
    /// Set of the bodies detected by this volumetric sensor.
    /// </summary>
    public HashSet<Node3D> DetectedBodies { get; private set; } = new();
    
    public override void _EnterTree()
    {
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }
    
    public override void _ExitTree()
    {
        BodyEntered -= OnBodyEntered;
        BodyExited -= OnBodyExited;
    }

    private void OnBodyEntered(Node3D body)
    {
        DetectedBodies.Add(body);
    }

    private void OnBodyExited(Node3D body)
    {
        DetectedBodies.Remove(body);
    }
}