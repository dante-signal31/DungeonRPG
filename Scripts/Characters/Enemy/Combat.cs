using System.Linq;
using DungeonRPG.Scripts.General;
using DungeonRPG.Scripts.Sensors;
using Godot;

namespace DungeonRPG.Scripts.Characters.Enemy;

public partial class Combat : Node
{
    [ExportCategory("WIRING:")] 
    [Export] private Enemy _characterNode;
    [Export] private VolumetricSensor _attackSensor;
    [Export] private EnemyStateMachine _stateMachine;
    
    private bool IsAttacking => _stateMachine.CurrentState != EnemyStateMachine.EnemyStates.Attack;
    
    private void HitPlayer()
    {
        if (!IsAttacking) _stateMachine.SwitchState(EnemyStateMachine.EnemyStates.Attack);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_attackSensor.DetectedBodies.Count > 0)
        {
            LookAtPlayer();
            HitPlayer();
        }
    }
    
    private void LookAtPlayer()
    {
        if (_attackSensor.DetectedBodies.Count > 0)
        {
            Node3D player = _attackSensor.DetectedBodies.First();
            bool isPlayerAtLeft = player.GlobalPosition.X < _characterNode.GlobalPosition.X;
            _characterNode.Flip(isPlayerAtLeft);
        }
    }
}


