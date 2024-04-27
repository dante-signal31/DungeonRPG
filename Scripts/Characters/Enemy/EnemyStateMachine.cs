using System;
using System.Diagnostics;
using Godot;

namespace DungeonRPG.Scripts.Characters.Enemy;

public partial class EnemyStateMachine : AnimationTree
{
    /// <summary>
    /// Signal state machine changed its state. New state is emitted along the signal as an int
    /// that can be cast as an EnemyStates enum.
    /// </summary>
    [Signal] public delegate void StateChangedEventHandler(EnemyStates newState);
    
    /// <summary>
    /// Signal the death animation has finished.
    /// </summary>
    [Signal] public delegate void DeathAnimationFinishedEventHandler();
    
    public enum EnemyStates
    {
        Idle,
        Patrol,
        Pursuit,
        Attack,
        TakeHit,
        Death
    }
    
    public bool IsAttacking
    {
        get => (bool)_stateMachine.Get("parameters/conditions/isAttacking"); 
        set => _stateMachine.Set("parameters/conditions/isAttacking", value);
    }

    public bool IsTakingHit
    {
        get => (bool)_stateMachine.Get("parameters/conditions/isTakingHit"); 
        set => _stateMachine.Set("parameters/conditions/IsTakingHit", value);
    }

    public bool IsIdle
    {
        get => (bool)_stateMachine.Get("parameters/conditions/isIdle"); 
        set => _stateMachine.Set("parameters/conditions/isIdle", value);
    }

    public bool IsDead
    {
        get => (bool)_stateMachine.Get("parameters/conditions/isDead"); 
        set => _stateMachine.Set("parameters/conditions/isDead", value);
    }

    public bool IsMoving
    {
        get => (bool) _stateMachine.Get("parameters/conditions/isMoving"); 
        set => _stateMachine.Set("parameters/conditions/isMoving", value);
    }

    /// <summary>
    /// This state machine current state.
    /// </summary>
    /// <exception cref="Exception"></exception>
    public EnemyStates CurrentState
    {
        get
        {
            EnemyStates currentState = StringToEnemyStates(_stateMachine.GetCurrentNode());
            return currentState;
        }
    }
    
    public EnemyStates DefaultState { get; set; }
    
    private AnimationNodeStateMachinePlayback _stateMachine;

    /// <summary>
    /// Convert an state name into an EnemyState enum variant.
    /// </summary>
    /// <param name="state">State name string</param>
    /// <returns>EnemyStates enum variant.</returns>
    /// <exception cref="Exception">Exception if the state name is unknown.</exception>
    private EnemyStates StringToEnemyStates(string state)
    {
        EnemyStates enemyState = state switch
        {
            "Idle" => EnemyStates.Idle,
            "Patrol" => EnemyStates.Patrol,
            "Pursuit" => EnemyStates.Pursuit,
            "Attack" => EnemyStates.Attack,
            "TakeHit" => EnemyStates.TakeHit,
            "Death" => EnemyStates.Death,
            _ => DefaultState
        };
        return enemyState;
    }
    
    /// <summary>
    /// Make this state machine switch to the specified state.
    ///
    /// State machine only changes to new state if there is a link path to new state
    /// from the current one. To travel to the new state all links and states must be
    /// traversed.
    /// </summary>
    /// <param name="state">New state.</param>
    /// <exception cref="Exception"></exception>
    public void SwitchState(EnemyStates state)
    {
        string newStateName = state switch
        {
            EnemyStates.Idle => "Idle",
            EnemyStates.Patrol => "Patrol",
            EnemyStates.Pursuit => "Pursuit",
            EnemyStates.Attack => "Attack",
            EnemyStates.TakeHit => "TakeHit",
            EnemyStates.Death => "Death",
            _ => throw new Exception("Unknown state: " + state)
        };
        _stateMachine.Travel(newStateName);
        // _stateMachine.Next();
        // There's a bug in Godot that prevents AnimationTree to emit AnimationStarted signal
        // when that animation loops. So I cannot rely on OnAnimationStarted() be called to emit
        // StateChanged signal. That's why I do it here manually.
        // https://github.com/godotengine/godot/issues/76159
        EmitSignal(SignalName.StateChanged, (int)state);
    }
    
    public override void _Ready()
    {
        _stateMachine = (AnimationNodeStateMachinePlayback) Get("parameters/playback");
        SwitchState(DefaultState);
    }
    
    public override void _EnterTree()
    {
        this.AnimationStarted += OnAnimationStarted;
        AnimationFinished += OnAnimationFinished;
    }
    
    public override void _ExitTree()
    {
        this.AnimationStarted -= OnAnimationStarted;
        AnimationFinished -= OnAnimationFinished;
    }

    private void OnAnimationStarted(StringName name)
    {
        EnemyStates newState = StringToEnemyStates(name);
        EmitSignal(SignalName.StateChanged, (int)newState);
    }

    private void OnAnimationFinished(StringName animName)
    {
        if (animName == "Death")
            EmitSignal(SignalName.DeathAnimationFinished);
    }
}