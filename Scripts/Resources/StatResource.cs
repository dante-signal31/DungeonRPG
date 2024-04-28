using System;
using Godot;

namespace DungeonRPG.Scripts.Resources;

[GlobalClass]
public partial class StatResource: Resource
{
    /// <summary>
    /// Emitted when the stat value is updated.
    /// </summary>
    public event EventHandler StatValueChanged;
    
    [Export] public Stat StatType { get; private set; }

    private float _statValue;
    [Export] public float StatValue
    {
        get => _statValue;
        set
        {
            _statValue = Mathf.Clamp(value, 0, int.MaxValue);
            StatValueChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}