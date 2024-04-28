using System;

namespace DungeonRPG.Scripts.General;

public class GameEvents
{
    public static event EventHandler GameStartedEvent;
    public static event EventHandler GameEndedEvent;
    public static event EventHandler EnemyDiedEvent;
    public static event EventHandler<IntEventArgs> NumberOfEnemiesChangedEvent;
    
    public static void RaiseNumberOfEnemiesChanged(int value) => 
        NumberOfEnemiesChangedEvent?.Invoke(null, new IntEventArgs(value));
    
    public static void RaiseGameStarted() => GameStartedEvent?.Invoke(null, EventArgs.Empty);
    public static void RaiseGameEnded() => GameEndedEvent?.Invoke(null, EventArgs.Empty);
    public static void RaiseEnemyDied() => EnemyDiedEvent?.Invoke(null, EventArgs.Empty);
    
    public class IntEventArgs : EventArgs
    {
        public int Value { get; set; }
        
        public IntEventArgs(int value)
        {
            Value = value;
        }
    }
}