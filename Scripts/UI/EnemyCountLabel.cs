using DungeonRPG.Scripts.General;
using Godot;

namespace DungeonRPG.Scripts.UI;

public partial class EnemyCountLabel : Label
{
        public override void _EnterTree()
        {
                base._EnterTree();
                GameEvents.NumberOfEnemiesChangedEvent += OnNumberOfEnemiesChanged;
        }
        
        public override void _ExitTree()
        {
                base._ExitTree();
                GameEvents.NumberOfEnemiesChangedEvent -= OnNumberOfEnemiesChanged;
        }

        private void OnNumberOfEnemiesChanged(object sender, IntEventArgs e)
        {
                Text = e.Value.ToString();
        }
}