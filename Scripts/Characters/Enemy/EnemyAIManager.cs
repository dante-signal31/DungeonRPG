using System;
using System.Linq;
using DungeonRPG.Scripts.Sensors;
using Godot;

namespace DungeonRPG.Scripts.Characters.Enemy;

public partial class EnemyAIManager : Node
{
    [ExportCategory("WIRING:")] 
    [Export] private EnemyStateMachine _stateMachine;
    [Export] private NavigationAI _navigationAi;
    [Export] private AgentMover _agentMover;
    [Export] private NavigationAgent3D _navigationAgent;
    [Export] private VolumetricSensor _volumetricSensor;
    
    [ExportCategory("CONFIGURATION:")]
    [Export] private float _arrivingRadius;

    public override void _EnterTree()
    {
        _volumetricSensor.BodyEntered += OnPlayerDetected;
        _volumetricSensor.BodyExited += OnPlayerExited;
    }

    public override void _ExitTree()
    {
        _volumetricSensor.BodyEntered -= OnPlayerDetected;
        _volumetricSensor.BodyExited -= OnPlayerExited;
    }

    public override void _Ready()
    {
        _agentMover.ArrivingRadius = _arrivingRadius;
        _navigationAi.ArrivingRadius = _arrivingRadius;
        _navigationAgent.TargetDesiredDistance = _arrivingRadius;
    }

    public override void _PhysicsProcess(double delta)
    {
        switch (_stateMachine.CurrentState)
        {
            case EnemyStateMachine.EnemyStates.Patrol:
                _agentMover.TargetPosition = _navigationAi.NextPositionToReachTarget;
                break;
            case EnemyStateMachine.EnemyStates.Idle:
                break;
            case EnemyStateMachine.EnemyStates.Pursuit:
                _agentMover.TargetPosition = _navigationAi.NextPositionToReachTarget;
                break;
            default:
                throw new NotImplementedException();
        }
        // if (_volumetricSensor.DetectedBodies.Count > 0)
        // {
        //         GD.Print($"[{_characterNode.Name}]Player position: {_volumetricSensor.DetectedBodies.First().GlobalPosition} " +
        //                  $"\n[{_characterNode.Name}]Target Position: {_agentMover.TargetPosition}\n\n" +
        //                  $"==================================================");
        // }
    }

    private void OnPlayerDetected(Node3D body)
    {
        _stateMachine.SwitchState(EnemyStateMachine.EnemyStates.Pursuit);
    }

    private void OnPlayerExited(Node3D body)
    {
        _stateMachine.SwitchState(_stateMachine.DefaultState);
    }
}