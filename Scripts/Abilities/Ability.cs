using System;
using DungeonRPG.Scripts.General;
using Godot;
using Godot.Collections;

namespace DungeonRPG.Scripts.Abilities;

public partial class Ability : Node3D, IDamager
{
    [ExportCategory("WIRING:")]
    [Export] private AnimationPlayer _animationPlayerNode;

    [ExportCategory("CONFIGURATION:")]
    [Export] private Array<AbilityType> _usedAbilities;
    [Export] public float Damage { get; private set; }

    private int _currentAbilityIndex = 0;

    public override void _EnterTree()
    {
        base._EnterTree();
        _animationPlayerNode.AnimationFinished += OnAnimationFinished;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        _animationPlayerNode.AnimationFinished -= OnAnimationFinished;
    }

    private void OnAnimationFinished(StringName animname)
    {
        if (animname == GetAnimationName(_usedAbilities[_currentAbilityIndex]))
        {
            if (_currentAbilityIndex < _usedAbilities.Count - 1)
            {
                _currentAbilityIndex++;
                _animationPlayerNode.Play(GetAnimationName(_usedAbilities[_currentAbilityIndex]));
            }
            else
            {
                QueueFree();
            }
        }
    }

    public override void _Ready()
    {
        base._Ready();
        _animationPlayerNode.Play(GetAnimationName(_usedAbilities[_currentAbilityIndex]));
    }

    private string GetAnimationName(AbilityType abilityType)
    {
        switch (abilityType)
        {
            case AbilityType.Explode:
                return GameConstants.ANIM_EXPLOSION;
            case AbilityType.Expand:
                return GameConstants.ANIM_EXPAND;
            case AbilityType.Lightning:
                return GameConstants.ANIM_LIGHTNING;
            default:
                throw new Exception("Ability type not implemented");
        }
    }
}