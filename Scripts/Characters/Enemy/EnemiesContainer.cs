using System;
using System.Linq;
using DungeonRPG.Scripts.General;
using Godot;

namespace DungeonRPG.Scripts.Characters.Enemy;

public partial class EnemiesContainer : Node
{
    private int _numberOfEnemies;

    /// <summary>
    /// Number of enemies in current scene.
    /// </summary>
    public int NumberOfEnemies
    {
        get => _numberOfEnemies;
        private set
        {
            _numberOfEnemies = value;
            GameEvents.RaiseNumberOfEnemiesChanged(_numberOfEnemies);
        }
    }

    public override void _Ready()
    {
        NumberOfEnemies = UpdateNumberOfEnemies();
    }

    public override void _EnterTree()
    {
        GameEvents.EnemyDiedEvent += OnEnemyDied;
    }

    public override void _ExitTree()
    {
        GameEvents.EnemyDiedEvent += OnEnemyDied;
    }

    private void OnEnemyDied(object sender, EventArgs e)
    {
        // We substract one because the died enemy is still playing its dying animation
        // so he still exists in the scene when this method is called.
        NumberOfEnemies = UpdateNumberOfEnemies() - 1;
    }

    private int UpdateNumberOfEnemies()
    {
        return GetChildren().Count(child => child is Enemy);
    }
}