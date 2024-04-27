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
    }

    public override void _ExitTree()
    {
        GameEvents.GameStartedEvent += OnGameStarted;
    }

    private void OnGameStarted(object sender, EventArgs _)
    {
        Reparent(_target);
        Position = positionFromTarget;
    }
}