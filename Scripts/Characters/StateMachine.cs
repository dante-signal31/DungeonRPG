using Godot;
using System;

public partial class StateMachine : Node
{
    [Export] private PlayerState _currentState;
    [Export] private PlayerState[] _states;
    
    public override void _Ready()
    {
        _currentState.Notify(5001);
    }
    
    public void SwitchState<T>()
    {
        PlayerState newState = null;

        foreach (PlayerState state in _states)
        {
            if (state is T)
            {
                newState = state;
                break;
            }
        }

        if (newState == null) return;

        // Disable current state because we are going to switch to another one.
        _currentState.Notify(5002);
        // Switch to another state.
        _currentState = newState;
        // Enable new current state.
        _currentState.Notify(5001);
    }
}
