using System.Linq;
using System.Runtime.CompilerServices;
using DungeonRPG.Scripts.Resources;
using Godot;

namespace DungeonRPG.Scripts.Characters.Enemy;

public partial class Character : CharacterBody3D
{
    [Export] public Hurtbox HurtBox { get; private set; }
    [Export] public Area3D HitBox { get; private set; }
    [Export] private StatResource[] _statResources;

    public float Strength
    {
        get
        {
            return GetStatResource(Stat.Strength).StatValue;
        }
    }


    public override void _EnterTree()
    {
        base._EnterTree();
        HurtBox.AreaEntered += OnHurtBoxEntered;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        HurtBox.AreaEntered -= OnHurtBoxEntered;
    }

    protected void OnHurtBoxEntered(Area3D area)
    {
        float attackerStrength = GetAttackerStrength(area);
        ApplyDamage(attackerStrength);
    }

    private float GetAttackerStrength(Area3D area)
    {
        Character attacker = area.GetOwner<Character>();
        return attacker.Strength;
    }

    private void ApplyDamage(float damage)
    {
        StatResource health = GetStatResource(Stat.Health);
        health.StatValue -= damage;
        GD.Print($"[{Name}] Health: {health.StatValue}");
    }

    private StatResource GetStatResource(Stat stat)
    {
        StatResource resource = _statResources.First(value => value.StatType == stat);
        return resource;
    }
}