using System.Linq;
using DungeonRPG.Scripts.Characters.Player;
using DungeonRPG.Scripts.General;
using Godot;

namespace DungeonRPG.Scripts.Characters;

public partial class StateMachine : Node
{
    /// <summary>
    /// Signal state machine changed its state. New state is emitted along the signal as an int
    /// that can be cast as an EnemyStates enum.
    /// </summary>
    [Signal] public delegate void StateChangedEventHandler(PlayerState newState);
    
    [ExportCategory("WIRING:")]
    [Export] private Player.PlayerState _currentState;
    [Export] private Player.PlayerState[] _states;
    
    public override void _Ready()
    {
        _currentState.Notify(GameConstants.NOTIFICATION_STATE_ENTER);
    }
    
    public void SwitchState<T>()
    {
        if (_currentState is T) return;
        
        PlayerState newState = _states.First(state => state is T);

        // Disable current state because we are going to switch to another one.
        _currentState.Notify(GameConstants.NOTIFICATION_STATE_EXIT);
        // Switch to another state.
        if (newState != _currentState) 
            EmitSignal(SignalName.StateChanged, newState);
        _currentState = newState;
        // Enable new current state.
        _currentState.Notify(GameConstants.NOTIFICATION_STATE_ENTER);
    }
}