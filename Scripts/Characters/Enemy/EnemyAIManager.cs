using System;
using Godot;

namespace DungeonRPG.Scripts.Characters.Enemy;

public partial class EnemyAIManager : Node
{
    [ExportCategory("WIRING:")] 
    [Export] private CharacterBody3D _characterBody;
    [Export] private EnemyStateMachine _stateMachine;
    [Export] private NavigationAI _navigationAi;
    [Export] private AgentMover _agentMover;
    
    [ExportCategory("CONFIGURATION:")]
    [Export] private float _arrivingRadius;

    public override void _Ready()
    {
        _agentMover.ArrivingRadius = _arrivingRadius;
        _navigationAi.ArrivingRadius = _arrivingRadius;
    }

    public override void _PhysicsProcess(double delta)
    {
        switch (_stateMachine.CurrentState)
        {
            case EnemyStateMachine.EnemyStates.Patrol:
                _agentMover.TargetPosition = _navigationAi.TargetPosition;
                break;
            case EnemyStateMachine.EnemyStates.Idle:
                break;
            default:
                throw new NotImplementedException();
        }
    }
}