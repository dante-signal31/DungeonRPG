using System.Linq;
using DungeonRPG.Scripts.General;
using DungeonRPG.Scripts.Resources;
using Godot;

namespace DungeonRPG.Scripts.Characters;

public abstract partial class Character : CharacterBody3D, IDamager
{
    
    /// <summary>
    /// Signal emitted when this character has been hit while fighting.
    ///
    /// Includes as a parameter the hitBox area of the character that hurt this one.
    /// </summary>
    [Signal] public delegate void BeenHitEventHandler(Area3D area);

    [ExportCategory("WIRING:")] 
    [Export] private CharacterLifeManager _lifeManager;
    [Export] public Hurtbox HurtBox { get; private set; }
    [Export] public Area3D HitBox { get; private set; }
    [Export] public StatResource[] StatResources { get; private set; }
    
    [ExportGroup("Combat behavior")]
    [Export] public float CooldownTimeAfterAttack { get; private set; }
    [Export] public float CooldownTimeAfterBeenHit { get; private set; }

    /// <summary>
    /// Strength of the character
    /// </summary>
    public float Strength => GetStatResource(Stat.Strength).StatValue;

    /// <summary>
    /// Current health of the character
    /// </summary>
    public float Health
    {
        get => GetStatResource(Stat.Health).StatValue;
        set => GetStatResource(Stat.Health).StatValue = Mathf.Clamp(value, 0, int.MaxValue);
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        HurtBox.AreaEntered += OnHurtBoxEntered;
        _lifeManager.WeHaveBeenKilled += OnCharacterKilled;
        
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        HurtBox.AreaEntered -= OnHurtBoxEntered;
        _lifeManager.WeHaveBeenKilled -= OnCharacterKilled;
    }

    protected void OnHurtBoxEntered(Area3D area)
    {
        EmitSignal(SignalName.BeenHit, area);
    }

    protected abstract void OnCharacterKilled();

    protected StatResource GetStatResource(Stat stat)
    {
        StatResource resource = StatResources.First(value => value.StatType == stat);
        return resource;
    }

    public float Damage => Strength;
}