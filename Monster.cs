using System;
using System.Text.Json.Serialization;

namespace Hampus_Epic_Adventure
{
    public class Monster
    {
        public int Health = 100;
        public string Name { get; set; }
        public Item CorrectItem { get; set; }
        public string Description { get; set; }
        public string Hint { get; set; }

        public Monster(int health, string name, string description, Item correctItem, string hint)
        {
            Health = health;
            Name = name;
            CorrectItem = correctItem;
            Description = $"There is a monster named {Name} here. You would need a {CorrectItem} to defend yourself.";
            Hint = $"Find the {CorrectItem} to defeat {Name}";
        }
        private void DealDamage(Player player)
        {
            Random random = new Random();
            player.Health -= random.Next(15, 30);
        }

        public string DisplayHelp()
        {
            if (Game.State.Player.Inventory.Any(item => CorrectItem is Item correctItem))
            {
                Hint = $"Kill {Name} by using the {CorrectItem}. Say \"use {CorrectItem}\"";
                return Hint;
            }
            else return Hint;
        }
    }
}
