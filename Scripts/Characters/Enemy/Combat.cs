using System;
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
    [Export] Timer _attackCoolDownTimer;
    [Export] Timer _beenHitCoolDownTimer;
    
    /// <summary>
    /// Whether this modules is already performing an attack-
    /// </summary>
    public bool PerformingAttack { get; private set; } = false;
    private bool _isTakingHit = false;
    private bool _coolingDownAfterHit = false;
    private bool _coolingDownAfterAttack = false;


    public override void _EnterTree()
    {
        _stateMachine.AnimationFinished += OnAnimationEnded;
        _stateMachine.AnimationStarted += OnAnimationStarted;
        _characterNode.BeenHit += OnBeenHit;
        _attackCoolDownTimer.Timeout += OnAttackCoolDownTimerTimeout;
        _beenHitCoolDownTimer.Timeout += OnBeenHitCoolDownTimerTimeout;
    }

    public override void _ExitTree()
    {
        _stateMachine.AnimationFinished -= OnAnimationEnded;
        _stateMachine.AnimationStarted -= OnAnimationStarted;
        _characterNode.BeenHit -= OnBeenHit;
        _attackCoolDownTimer.Timeout -= OnAttackCoolDownTimerTimeout;
        _beenHitCoolDownTimer.Timeout -= OnBeenHitCoolDownTimerTimeout;
    }

    private void OnAnimationStarted(StringName animname)
    {
        if (animname == GameConstants.ANIM_ATTACK)
        {
            PerformingAttack = true;
        }
        else
        {
            PerformingAttack = false;
        }
    }

    private void OnAttackCoolDownTimerTimeout()
    {
        _coolingDownAfterAttack = false;
    }
    
    private void OnBeenHitCoolDownTimerTimeout()
    {
        _coolingDownAfterHit = false;
    }
    
    public override void _Ready()
    {
        if (_characterNode != null)
        {
            _attackCoolDownTimer.WaitTime = _characterNode.CooldownTimeAfterAttack;
            _beenHitCoolDownTimer.WaitTime = _characterNode.CooldownTimeAfterBeenHit;
        }
    }
    
    private void OnAnimationEnded(StringName animName)
    {
        switch (animName)
        {
            case GameConstants.ANIM_ATTACK:
                PerformingAttack = false;
                _coolingDownAfterAttack = true;
                _attackCoolDownTimer.Start();
                break;
            case GameConstants.ANIM_TAKE_HIT:
                _coolingDownAfterHit = true;
                _isTakingHit = false;
                _beenHitCoolDownTimer.Start();
                break;
        }
    }

    private void OnBeenHit(Area3D _)
    {
        _isTakingHit = true;
    }

    public void HitPlayer()
    {
        if (!PerformingAttack && !_isTakingHit && 
            !_coolingDownAfterHit && !_coolingDownAfterAttack)
        {
            // PerformingAttack = true; 
            _stateMachine.SwitchState(EnemyStateMachine.EnemyStates.Attack);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_attackSensor.DetectedBodies.Count > 0)
        {
            LookAtPlayer();
        }
    }
    
    private void LookAtPlayer()
    {
        if (_attackSensor.DetectedBodies.Count > 0)
        {
            Node3D player = _attackSensor.DetectedBodies.First();
            _characterNode.IsFacingLeft = (player.GlobalPosition.X - _characterNode.GlobalPosition.X) < 0;
        }
    }
}


