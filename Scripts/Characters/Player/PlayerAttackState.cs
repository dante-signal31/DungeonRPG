using DungeonRPG.Scripts.General;
using Godot;

namespace DungeonRPG.Scripts.Characters.Player;

public partial class PlayerAttackState : PlayerState
{
    [ExportCategory("WIRING:")]
    [Export] private Timer _cooldownTimer;

    [ExportCategory("CONFIGURATION:")] 
    [Export] private float _cooldownTime = 3.0f;
        
    private int comboCounter = 1;
    private int maxComboCount = 2;

    public override void _EnterTree()
    {
        _cooldownTimer.Timeout += OnCooldownTimerTimeout;
    }

    public override void _ExitTree()
    {
        _cooldownTimer.Timeout -= OnCooldownTimerTimeout;
    }

    public override void _Ready()
    {
        base._Ready();
        _cooldownTimer.OneShot = true;
        _cooldownTimer.WaitTime = _cooldownTime;
    }


    protected override void EnterState()
    {
        _characterNode.AnimationPlayer.Play($"{GameConstants.ANIM_ATTACK}{comboCounter}", 
            customBlend: -1,customSpeed: 1.5F);
        _characterNode.AnimationPlayer.AnimationFinished += OnAnimationFinished;
    }
    
    protected override void ExitState()
    {
        _characterNode.AnimationPlayer.AnimationFinished -= OnAnimationFinished;
    }

    private void OnAnimationFinished(StringName animName)
    {
        if (animName == $"{GameConstants.ANIM_ATTACK}{comboCounter}")
        {
            comboCounter = Mathf.Wrap(++comboCounter, 1, maxComboCount + 1);
            _cooldownTimer.Start();
            _characterNode.StateMachine.SwitchState<PlayerIdleState>();
        }
    }

    private void OnCooldownTimerTimeout()
    {
        comboCounter = 1;
    }
}