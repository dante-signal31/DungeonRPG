using Godot;

namespace DungeonRPG.Scripts.Characters.Enemy;

public partial class PursuitBehavior : Node, INavigationBehavior
{
    public float ArrivingRadius { get; set; }
    public Vector3 TargetPosition { get; set; }
    public void UpdateTargetPosition()
    {
        throw new System.NotImplementedException();
    }
}