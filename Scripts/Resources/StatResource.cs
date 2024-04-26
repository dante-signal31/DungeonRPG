using Godot;

namespace DungeonRPG.Scripts.Resources;

[GlobalClass]
public partial class StatResource: Resource
{
    [Export] public Stat StatType { get; private set; }

    private float _statValue;
    [Export] public float StatValue
    {
        get => _statValue;
        set => _statValue = Mathf.Clamp(value, 0, int.MaxValue);
    }
}