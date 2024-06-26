﻿using System.Linq;
using DungeonRPG.Scripts.Abilities;
using DungeonRPG.Scripts.Resources;
using Godot;

namespace DungeonRPG.Scripts.Characters;

public abstract partial class CharacterLifeManager : Node
{
    /// <summary>
    /// Signal that we have just jumped into Death state.
    /// </summary>
    [Signal] public delegate void WeHaveBeenKilledEventHandler();

    /// <summary>
    /// Signal that we have received some damage.
    ///
    /// Damage is a negative value.
    /// </summary>
    [Signal] public delegate void WeHaveBeenDamagedEventHandler(float damage);
    
    [ExportCategory("WIRING:")]
    [Export] private Character _characterNode;
    
    public override void _EnterTree()
    {
        base._EnterTree();
        _characterNode.BeenHit += OnBeenHit;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        _characterNode.BeenHit -= OnBeenHit;
    }
    
    protected float GetAttackerStrength(Area3D area)
    {
        IDamager attacker = area.GetOwner<IDamager>();
        return attacker.Damage;
    }

    protected virtual void ApplyDamage(float damage)
    {
        _characterNode.Health -= damage;
        EmitSignal(SignalName.WeHaveBeenDamaged, -damage);
        GD.Print($"[{_characterNode.Name}] Health = " +
                 $"{_characterNode.Health}");
        if (_characterNode.Health == 0)
        {
            CharacterKilled();
        }
        else
        {
            CharacterHaveBeenHit();
        }
    }
    
    protected void OnBeenHit(Area3D area)
    {
        float attackerStrength = GetAttackerStrength(area);
        ApplyDamage(attackerStrength);
    }

    /// <summary>
    /// Make character state machine jump to TakeHit state.
    /// </summary>
    protected abstract void CharacterHaveBeenHit();
    
    /// <summary>
    /// Make character state machine jump to Death state.
    /// </summary>
    protected abstract void CharacterKilled();
}