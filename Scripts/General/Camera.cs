using System;
using Godot;

namespace DungeonRPG.Scripts.General;

public partial class Camera : Camera3D
{
    [Export] private Node3D _target;
    [Export] private Vector3 positionFromTarget;
    
    public override void _EnterTree()
    {
        GameEvents.GameStartedEvent += OnGameStarted;
        GameEvents.GameEndedEvent += OnGameEnded;
    }

    public override void _ExitTree()
    {
        GameEvents.GameStartedEvent += OnGameStarted;
        GameEvents.GameEndedEvent -= OnGameEnded;
    }

    private void OnGameStarted(object sender, EventArgs _)
    {
        Reparent(_target);
        Position = positionFromTarget;
    }
    
    private void OnGameEnded(object sender, EventArgs e)
    {
        Reparent(GetTree().Root);
    }
}