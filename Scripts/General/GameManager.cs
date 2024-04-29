using Godot;

namespace DungeonRPG.Scripts.General;

public partial class GameManager : Node
{
    public override void _EnterTree()
    {
        base._EnterTree();
        // Make it process mode "Always". Otherwise this node will freeze when you pause the game 
        // and it won't respond when you try to unpause the game.
        ProcessMode = ProcessModeEnum.Always;
        GameEvents.NumberOfEnemiesChangedEvent += OnNumberOfEnemiesChanged;
    }
    
    public override void _ExitTree()
    {
        base._ExitTree();
        GameEvents.NumberOfEnemiesChangedEvent -= OnNumberOfEnemiesChanged;
    }
    
    private void OnNumberOfEnemiesChanged(object sender, IntEventArgs e)
    {
        GD.Print($"[GameManager] Received notification that only {e.Value} enemies are left");
        if (e.Value == 0)
        {
            GetTree().Paused = true;
            GameEvents.RaiseGameEnded(true);
        }
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (Input.IsActionJustPressed(GameConstants.INPUT_PAUSE) && !GetTree().Paused)
        {
            PauseGame();
        } 
        else if (Input.IsActionJustPressed(GameConstants.INPUT_PAUSE))
        {
            ResumeGame();
        }
    }

    public void ResumeGame()
    {
        GetTree().Paused = false;
        GameEvents.RaiseResumedGame();
    }

    public void PauseGame()
    {
        GetTree().Paused = true;
        GameEvents.RaisePausedGame();
    }
}