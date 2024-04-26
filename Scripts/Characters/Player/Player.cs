using DungeonRPG.Scripts.Characters.Enemy;
using DungeonRPG.Scripts.General;
using Godot;

namespace DungeonRPG.Scripts.Characters.Player;

public partial class Player : Character
{
    [ExportGroup("Required Nodes")]
    [Export] public AnimationPlayer AnimationPlayer { get; private set; }
    [Export] private Sprite3D _spriteNode;
    [Export] public StateMachine StateMachine { get; private set; }
    
    /// <summary>
    /// Is this Character facing left?
    /// </summary>
    public bool IsFacingLeft { get; private set; }
    
    private Vector2 _direction = Vector2.Zero;
    
    /// <summary>
    /// Current direction of this Character.
    /// </summary>
    public Vector2 Direction => _direction;

    public override void _Input(InputEvent @event)
    {
        _direction =Input.GetVector(GameConstants.INPUT_MOVE_LEFT,
            GameConstants.INPUT_MOVE_RIGHT,
            GameConstants.INPUT_MOVE_FORWARD,
            GameConstants.INPUT_MOVE_BACKWARD);
    }

    public void Flip()
    {
        bool isMovingHorizontally = Velocity.X != 0;
        if (isMovingHorizontally)
        {
            IsFacingLeft = _direction.X < 0;
            _spriteNode.FlipH = IsFacingLeft;
        }
    }
}