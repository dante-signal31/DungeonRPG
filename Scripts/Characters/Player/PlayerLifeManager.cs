using DungeonRPG.Scripts.General;
using Godot;

namespace DungeonRPG.Scripts.Characters.Player;

public partial class PlayerLifeManager : CharacterLifeManager
{
    [ExportCategory("WIRING:")]
    [Export] StateMachine _stateMachine;
    
    protected override void CharacterHaveBeenHit()
    {
        GD.PushWarning("[Player] CharacterHaveBeenHit method called but not implemented!");
    }

    protected override void CharacterKilled()
    {
        _stateMachine.SwitchState<PlayerDeathState>();

        GameEvents.RaiseGameEnded();
    }
}