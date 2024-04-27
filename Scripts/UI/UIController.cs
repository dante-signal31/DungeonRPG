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

    public override void _ExitTree()
    {
        base._ExitTree();
        _containers[ContainerType.Start].ButtonNode.Pressed -= OnStartButtonPressed;
    }

    private void OnStartButtonPressed()
    {
        GetTree().Paused = false;
        _containers[ContainerType.Start].Visible = false;
        GameEvents.RaiseGameStarted();
    }
}
