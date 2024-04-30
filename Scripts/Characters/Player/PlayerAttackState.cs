using DungeonRPG.Scripts.General;
using Godot;

namespace DungeonRPG.Scripts.Characters.Player;

public partial class PlayerAttackState : PlayerState
{
    [ExportCategory("WIRING:")]
    [Export] private Timer _cooldownTimer;
    [Export] private PackedScene _lightningScene;

    [ExportCategory("CONFIGURATION:")] 
    [Export] private float _cooldownTime = 3.0f;
    [Export] private float _hitBoxDistance = 1.0f;
        
    private int comboCounter = 1;
    private int maxComboCount = 2;

    public override void _EnterTree()
    {
        _cooldownTimer.Timeout += OnCooldownTimerTimeout;
    }
    
    public override void _Ready()
    {
        base._Ready();
        _cooldownTimer.OneShot = true;
        _cooldownTimer.WaitTime = _cooldownTime;
        _characterNode.HitBox.BodyEntered += OnBoyEnteredTheHitbox;
    }
    

    public override void _ExitTree()
    {
        _cooldownTimer.Timeout -= OnCooldownTimerTimeout;
        _characterNode.HitBox.BodyEntered += OnBoyEnteredTheHitbox;
    }

    private void OnBoyEnteredTheHitbox(Node3D enemyBody)
    {
        if (comboCounter != maxComboCount) return;
        LaunchLightningAttack(enemyBody);
    }

    private void LaunchLightningAttack(Node3D enemyBody)
    {
        Node3D _lightning = _lightningScene.Instantiate<Node3D>();
        _lightning.Position = enemyBody.Position;
        GetTree().CurrentScene.AddChild(_lightning);
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

    private void PerformHit()
    {
        PlaceHitBox();
    }

    private void PlaceHitBox()
    {
        Vector3 newPosition = _characterNode.IsFacingLeft? Vector3.Left : Vector3.Right;
        _characterNode.HitBox.Position = newPosition * _hitBoxDistance;
    }
}