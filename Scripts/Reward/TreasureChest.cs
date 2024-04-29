using Godot;
using System;
using DungeonRPG.Scripts.Characters.Player;
using DungeonRPG.Scripts.General;
using DungeonRPG.Scripts.Reward;
using DungeonRPG.Scripts.Sensors;

public partial class TreasureChest : StaticBody3D
{
    [ExportCategory("WIRING:")] 
    [Export] private VolumetricSensor _playerSensor;
    [Export] private Sprite3D _openChestSprite;
    [Export] private Sprite3D _closedChestSprite;
    [Export] private Sprite3D _hintSprite;
    
    [ExportCategory("CONFIGURATION:")]
    [Export] private RewardResource _reward;
    
    public bool IsOpened { get; private set; } = false;

    public override void _EnterTree()
    {
        base._EnterTree();
        _playerSensor.BodyEntered += OnPlayerDetected;
        _playerSensor.BodyExited += OnPlayerExited;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        _playerSensor.BodyEntered -= OnPlayerDetected;
        _playerSensor.BodyExited -= OnPlayerExited;
    }

    public override void _Ready()
    {
        base._Ready();
        _closedChestSprite.Visible = true;
        _openChestSprite.Visible = false;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (Input.IsActionJustPressed(GameConstants.INPUT_INTERACT) &&
            (_playerSensor.DetectedBodies.Count > 0) &&
            !IsOpened)
        {
            OpenChest();
        }
        
    }

    private void OnPlayerDetected(Node3D _)
    {
        if (!IsOpened) _hintSprite.Visible = true;
    }
    
    private void OnPlayerExited(Node3D _)
    {
        if (!IsOpened) _hintSprite.Visible = false;
    }

    private void OpenChest()
    {
        IsOpened = true;
        _closedChestSprite.Visible = false;
        _openChestSprite.Visible = true;
        _hintSprite.Visible = false;
        GameEvents.RaiseRewardGot(_reward);
    }
}
