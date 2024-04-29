using Godot;
using System;
using DungeonRPG.Scripts.General;
using DungeonRPG.Scripts.Reward;
using DungeonRPG.Scripts.UI;

public partial class RewardUIUpdater : VBoxContainer
{
    [ExportCategory("WIRING:")] 
    [Export] private UIContainer _parentPanel;
    [Export] private Label _rewardLabel;
    [Export] private TextureRect _rewardIcon;

    private GameManager _gameManager;
    
    public override void _EnterTree()
    {
        base._EnterTree();
        GameEvents.RewardGotEvent += OnRewardGot;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        GameEvents.RewardGotEvent -= OnRewardGot;
    }

    public override void _Ready()
    {
        base._Ready();
        _gameManager = GetTree().Root.GetNode<GameManager>("GameManager");
    }

    public void OnRewardGot(object sender, RewardGotEventArgs rewardGotEventArgs)
    {
        _rewardIcon.Texture = rewardGotEventArgs.Reward.SpriteTexture;
        _rewardLabel.Text = rewardGotEventArgs.Reward.Description;
        _parentPanel.Visible = true;
        _gameManager.PauseGame();
    }
}
