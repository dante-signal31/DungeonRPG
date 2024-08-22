using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using DungeonRPG.Scripts.General;
using DungeonRPG.Scripts.UI;

public partial class UIController : Control
{
    private Dictionary<ContainerType, UIContainer> _containers;
    
    public override void _Ready()
    {
        _containers = GetChildren()
            .Where((element) => element is UIContainer)
            .Cast<UIContainer>()
            .ToDictionary((element) => element.container);
        
        _containers[ContainerType.Start].Visible = true;
        _containers[ContainerType.Start].ButtonNode.Pressed += OnStartButtonPressed;
    }
    
    public override void _EnterTree()
    {
        base._EnterTree();
        GameEvents.GameEndedEvent += OnGameEnded;
        GameEvents.PausedGameEvent += OnPausedGame;
        GameEvents.ResumedGameEvent += OnResumedGame;
    }
    
    public override void _ExitTree()
    {
        base._ExitTree();
        _containers[ContainerType.Start].ButtonNode.Pressed -= OnStartButtonPressed;
        GameEvents.GameEndedEvent -= OnGameEnded;
        GameEvents.PausedGameEvent -= OnPausedGame;
        GameEvents.ResumedGameEvent -= OnResumedGame;
    }

    private void OnGameEnded(object sender, GameEndedEventArgs e)
    {
        if (e.IsPlayerVictory)
        {
            _containers[ContainerType.Stats].Visible = false;
            _containers[ContainerType.Victory].Visible = true;
        }
        else
        {
            _containers[ContainerType.Stats].Visible = false;
            _containers[ContainerType.Defeat].Visible = true;
        }
    }

    private void OnStartButtonPressed()
    {
        GetTree().Paused = false;
        _containers[ContainerType.Start].Visible = false;
        _containers[ContainerType.Stats].Visible = true;
        GameEvents.RaiseGameStarted();
    }
    
    private void OnResumedGame(object sender, EventArgs e)
    {
        _containers[ContainerType.Pause].Visible = false;
        _containers[ContainerType.Reward].Visible = false;
    }

    private void OnPausedGame(object sender, EventArgs e)
    {
        _containers[ContainerType.Pause].Visible = true;
    }

    private void OnMapShown()
    {
        _containers[ContainerType.Stats].Visible = false;
    }

    private void OnMapHidden()
    {
        _containers[ContainerType.Stats].Visible = true;
    }
}
