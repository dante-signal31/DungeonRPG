using Godot;

namespace DungeonRPG.Scripts.Characters.Enemy;

public partial class Attack : Node
{
    [ExportCategory("WIRING:")]
    [Export] private Enemy _characterNode;
    
    [ExportCategory("CONFIGURATION:")] 
    [Export] private float _hitBoxDistance = 1.0f;
    
    private void PerformHit()
    {
        PlaceHitBox();
    }

    private void PlaceHitBox()
    {
        Vector3 newPosition = _characterNode.IsFacingLeft? Vector3.Left : Vector3.Right;
        _characterNode.HitBox.Position = newPosition * _hitBoxDistance;
    }
}