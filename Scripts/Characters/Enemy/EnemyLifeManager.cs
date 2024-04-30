using System.Linq;
using DungeonRPG.Scripts.General;
using DungeonRPG.Scripts.Resources;
using Godot;

namespace DungeonRPG.Scripts.Characters.Enemy;

public partial class EnemyLifeManager : CharacterLifeManager
{
    [ExportCategory("WIRING")] 
    [Export] private EnemyStateMachine _stateMachine;
    [Export] private Sprite3D _spriteNode;
    [Export] private Timer _hitTimer;
    
    [ExportCategory("CONFIGURATION:")]
    [Export] private float _hitTime = 0.5f;
    
    private ShaderMaterial _shader;

    public override void _EnterTree()
    {
        base._EnterTree();
        _hitTimer.Timeout += OnHitTimerTimeout;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        _hitTimer.Timeout -= OnHitTimerTimeout;
    }

    public override void _Ready()
    {
        base._Ready();
        _hitTimer.WaitTime = _hitTime;
        _shader = (ShaderMaterial) _spriteNode.MaterialOverlay;
    }
    
    private void OnHitTimerTimeout()
    {
        _shader.SetShaderParameter("active", false);
    }
    
    protected override void CharacterHaveBeenHit()
    {
        _stateMachine.SwitchState(EnemyStateMachine.EnemyStates.TakeHit);
        _shader.SetShaderParameter("active", true);
        _hitTimer.Start();
    }

    protected override void CharacterKilled()
    {
        _stateMachine.SwitchState(EnemyStateMachine.EnemyStates.Death);
        EmitSignal(SignalName.WeHaveBeenKilled);
        GameEvents.RaiseEnemyDied();
    }
}