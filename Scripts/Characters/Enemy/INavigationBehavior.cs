using Godot;

namespace DungeonRPG.Scripts.Characters.Enemy;

public interface INavigationBehavior
{
    public float ArrivingRadius { get; set; }
    public Vector3 TargetPosition { get; }
}