using System;
using System.Linq;
using DungeonRPG.Scripts.Sensors;
using Godot;

namespace DungeonRPG.Scripts.Characters.Enemy;

public partial class EnemyAIManager : Node
{
    [ExportCategory("WIRING:")] 
    [Export] private Enemy _characterNode;
    [Export] private EnemyStateMachine _stateMachine;
    [Export] private NavigationAI _navigationAi;
    [Export] private AgentMover _agentMover;
    [Export] private Combat _combat;
    [Export] private NavigationAgent3D _navigationAgent;
    [Export] private VolumetricSensor _pursuitSensor;
    [Export] private VolumetricSensor _attackSensor;
    
    [ExportCategory("CONFIGURATION:")]
    [Export] private float _arrivingRadius;
    
    private Player.Player AttackedPlayer
    {
        get
        {
            if (PlayerAvailableToAttack)
                return (Player.Player)_attackSensor.DetectedBodies.First();
            return null;
        }
    }

    private bool PlayerAvailableToAttack => _attackSensor.DetectedBodies.Count > 0;

    public override void _EnterTree()
    {
        _pursuitSensor.BodyEntered += OnPlayerDetectedInPursuitRange;
        _pursuitSensor.BodyExited += OnPlayerExitedOfPursuitRange;
        _attackSensor.BodyEntered += OnPlayerDetectedInAttackRange;
        _attackSensor.BodyExited += OnPlayerExitedOfAttackRange;
    }

    public override void _ExitTree()
    {
        _pursuitSensor.BodyEntered -= OnPlayerDetectedInPursuitRange;
        _pursuitSensor.BodyExited -= OnPlayerExitedOfPursuitRange;
        _attackSensor.BodyEntered -= OnPlayerDetectedInAttackRange;
        _attackSensor.BodyExited -= OnPlayerExitedOfAttackRange;
    }

    public override void _Ready()
    {
        _agentMover.ArrivingRadius = _arrivingRadius;
        _navigationAi.ArrivingRadius = _arrivingRadius;
        _navigationAgent.TargetDesiredDistance = _arrivingRadius;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (PlayerAvailableToAttack &&
            !_combat.PerformingAttack)
        {
            _combat.HitPlayer();
            return;
        }
            
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
            case EnemyStateMachine.EnemyStates.Attack:
                break;
            case EnemyStateMachine.EnemyStates.TakeHit:
                break;
            case EnemyStateMachine.EnemyStates.Death:
                break;
            default:
                throw new NotImplementedException();
        }
    }

    private void OnPlayerDetectedInPursuitRange(Node3D body)
    {
        _stateMachine.SwitchState(EnemyStateMachine.EnemyStates.Pursuit);
    }

    private void OnPlayerExitedOfPursuitRange(Node3D body)
    {
        _stateMachine.SwitchState(_stateMachine.DefaultState);
    }
    
    private void OnPlayerDetectedInAttackRange(Node3D body)
    {
        // _stateMachine.SwitchState(EnemyStateMachine.EnemyStates.Attack);
        // AttackedPlayer = (Player.Player) body;
    }
    
    private void OnPlayerExitedOfAttackRange(Node3D body)
    {
        if (_pursuitSensor.DetectedBodies.Count > 0)
        {
            _stateMachine.SwitchState(EnemyStateMachine.EnemyStates.Pursuit);
        }
        else
        {
            _stateMachine.SwitchState(_stateMachine.DefaultState);
        }
    }
}