using System.Linq;
using DungeonRPG.Scripts.Resources;
using Godot;

namespace DungeonRPG.Scripts.Characters.Enemy;

public partial class EnemyLifeManager : CharacterLifeManager
{
    [ExportCategory("WIRING")] 
    [Export] private EnemyStateMachine _stateMachine;
    
    protected override void CharacterHaveBeenHit()
    {
        _stateMachine.SwitchState(EnemyStateMachine.EnemyStates.TakeHit);
    }

    protected override void CharacterKilled()
    {
        _stateMachine.SwitchState(EnemyStateMachine.EnemyStates.Death);
        EmitSignal(SignalName.WeHaveBeenKilled);
    }
}