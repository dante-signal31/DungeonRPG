using System;
using DungeonRPG.Scripts.Resources;
using Godot;

namespace DungeonRPG.Scripts.UI;

public partial class StatLabel : Label
{
    [ExportCategory("CONFIGURATION:")]
    [Export] private StatResource _playerStatResource;

    public override void _Ready()
    {
        _playerStatResource.StatValueChanged += OnStatValueChanged;
        
        Text = _playerStatResource.StatValue.ToString();
    }

    public override void _ExitTree()
    {
        _playerStatResource.StatValueChanged += OnStatValueChanged;
    }

    private void OnStatValueChanged(object sender, EventArgs e)
    {
        Text = _playerStatResource.StatValue.ToString();
    }
}