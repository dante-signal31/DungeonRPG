using System.Linq;
using DungeonRPG.Scripts.Sensors;
using Godot;

namespace DungeonRPG.Scripts.Characters.Enemy;

public partial class Attack : Node
{
    [ExportCategory("WIRING:")]
    [Export] private Enemy _characterNode;
    [Export] private VolumetricSensor _attackSensor;
    
    private void PerformHit()
    {
        PlaceHitBox();
    }

    private void PlaceHitBox()
    {
        if (_attackSensor.DetectedBodies.Count == 0) return;
        Node3D player = _attackSensor.DetectedBodies.First();
        _characterNode.HitBox.GlobalPosition = player.GlobalPosition;
    }
}