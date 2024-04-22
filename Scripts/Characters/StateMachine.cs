using DungeonRPG.Scripts.General;
using Godot;

namespace DungeonRPG.Scripts.Characters;

public partial class StateMachine : Node
{
    [Export] private Player.PlayerState _currentState;
    [Export] private Player.PlayerState[] _states;
    
    public override void _Ready()
    {
        _currentState.Notify(GameConstants.NOTIFICATION_STATE_ENTER);
    }
    
    public void SwitchState<T>()
    {
        Player.PlayerState newState = null;

        foreach (Player.PlayerState state in _states)
        {
            if (state is T)
            {
                newState = state;
                break;
            }
        }

        if (newState == null) return;

        // Disable current state because we are going to switch to another one.
        _currentState.Notify(GameConstants.NOTIFICATION_STATE_EXIT);
        // Switch to another state.
        _currentState = newState;
        // Enable new current state.
        _currentState.Notify(GameConstants.NOTIFICATION_STATE_ENTER);
    }
}