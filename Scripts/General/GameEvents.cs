using System;

namespace DungeonRPG.Scripts.General;

public class GameEvents
{
    public static event EventHandler GameStartedEvent;
    
    public static void RaiseGameStarted() => GameStartedEvent?.Invoke(null, EventArgs.Empty);
}