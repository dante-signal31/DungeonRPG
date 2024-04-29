using Godot;
using System;
using DungeonRPG.Scripts.General;

public partial class ResumeButton : Button
{
    private GameManager _gameManager;

    public override void _Ready()
    {
        base._Ready();
        _gameManager = GetTree().Root.GetNode<GameManager>("GameManager");
        Connect(SignalName.Pressed, Callable.From(_gameManager.ResumeGame));
    }
    
    public override void _ExitTree()
    {
        base._ExitTree();
        Disconnect(SignalName.Pressed, Callable.From(_gameManager.ResumeGame)); 
    }
}
