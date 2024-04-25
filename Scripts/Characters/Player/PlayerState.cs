using DungeonRPG.Scripts.General;
using Godot;

namespace DungeonRPG.Scripts.Characters.Player;

public abstract partial class PlayerState : Node
{
    protected Player _characterNode;

    public override void _Ready()
    {
        _characterNode = GetOwner<Player>();
        SetPhysicsProcess(false);
        SetProcessInput(false);
    }
    
    public void Notify(int what, NotificationArgs args=null)
    {
        base._Notification(what);

        switch (what)
        {
            // Code sent to make the state as the current one. This state processing
            // is activated because it is who decides when to transition to another state.
            case GameConstants.NOTIFICATION_STATE_ENTER:
                // _characterNode.AnimationPlayer.Play(_stateAnimation);
                EnterState();
                SetPhysicsProcess(true);
                SetProcessInput(true);
                break;
            // Code sent to disable the state processing. This happens when the state 
            // no longer is going to be the current one. So, its processing is disabled.
            case GameConstants.NOTIFICATION_STATE_EXIT:
                SetPhysicsProcess(false);
                SetProcessInput(false);
                break;
        }
    }

    /// <summary>
    /// Actions taken when this state is activated as the current one.
    /// </summary>
    protected abstract void EnterState();
    
    /// <summary>
    /// Actions taken when this state ceases to be the current one.
    /// </summary>
    protected abstract void ExitState();
    
    protected void CheckForAttackInput()
    {
        if (Input.IsActionJustPressed(GameConstants.INPUT_ATTACK))
        {
            _characterNode.StateMachine.SwitchState<PlayerAttackState>();
        }
    }
}