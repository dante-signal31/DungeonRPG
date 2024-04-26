using DungeonRPG.Scripts.Sensors;
using Godot;

namespace DungeonRPG.Scripts.Characters.Enemy;

public partial class Combat : Node
{
    [ExportCategory("WIRING:")] [Export] private Enemy _characterNode;
    [Export] private VolumetricSensor _attackSensor;
    [Export] private StateMachine _stateMachine;

    /// <summary>
    /// Whether the attack sensor is pointing to left or not.
    /// </summary>
    private bool AttackSensorFacingLeft => _attackSensor.RotationDegrees.Y == 180;

    private void HitPlayer()
    {
        _stateMachine?.SwitchState<Attack>();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_attackSensor.DetectedBodies.Count > 0)
        {
            HitPlayer();
        }

        FlipAttackSensorIfNeeded();
    }

    private void FlipAttackSensorIfNeeded()
    {
        if (_characterNode.IsFacingLeft != AttackSensorFacingLeft)
        {
            _attackSensor.RotationDegrees = _characterNode.IsFacingLeft ?
                _attackSensor.RotationDegrees with { Y = 180} :
                _attackSensor.RotationDegrees with { Y = 0};
        }
    }
}


