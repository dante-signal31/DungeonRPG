using System;
using DungeonRPG.Scripts.Reward;

namespace DungeonRPG.Scripts.General;

public class GameEvents
{
    public static event EventHandler GameStartedEvent;
    public static event EventHandler<GameEndedEventArgs> GameEndedEvent;
    public static event EventHandler EnemyDiedEvent;
    public static event EventHandler<IntEventArgs> NumberOfEnemiesChangedEvent;
    public static event EventHandler PausedGameEvent;
    public static event EventHandler ResumedGameEvent; 
    
    public static event EventHandler<RewardGotEventArgs> RewardGotEvent;
    
    public static void RaiseNumberOfEnemiesChanged(int value) => 
        NumberOfEnemiesChangedEvent?.Invoke(null, new IntEventArgs(value));
    
    public static void RaiseGameStarted() => GameStartedEvent?.Invoke(null, EventArgs.Empty);
    public static void RaiseGameEnded(bool isPlayerVictory) => 
        GameEndedEvent?.Invoke(null, new GameEndedEventArgs(isPlayerVictory));
    public static void RaiseEnemyDied() => EnemyDiedEvent?.Invoke(null, EventArgs.Empty);

    public static void RaisePausedGame() => PausedGameEvent?.Invoke(null, EventArgs.Empty);

    public static void RaiseResumedGame() => ResumedGameEvent?.Invoke(null, EventArgs.Empty);
    
    public static void RaiseRewardGot(RewardResource reward) => RewardGotEvent?.Invoke(null, new RewardGotEventArgs(reward));
}

public class RewardGotEventArgs: EventArgs
{
    public RewardResource Reward { get; private set; }
    
    public RewardGotEventArgs(RewardResource reward)
    {
        Reward = reward;
    }
}

public class IntEventArgs : EventArgs
{
    public int Value { get; private set; }
        
    public IntEventArgs(int value)
    {
        Value = value;
    }
}

public class GameEndedEventArgs: EventArgs
{
    public bool IsPlayerVictory { get; private set; }
    
    public GameEndedEventArgs(bool isPlayerVictory)
    {
        IsPlayerVictory = isPlayerVictory;
    }
}