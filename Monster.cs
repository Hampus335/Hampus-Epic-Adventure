using System;
using System.Text.Json.Serialization;

namespace Hampus_Epic_Adventure
{
    public class Monster
    {
        public int Health { get; set; } = 100;
        public string Name { get; set; }
        public Item CorrectItem { get; set; }
        public string Description { get; set; }
        public string Hint { get; set; }

        public Monster(int health, string name, Item correctItem, string description, string hint)
        {
            Health = health;
            Name = name;
            CorrectItem = correctItem;
            Description = $"There is a monster named {Name} here. You would need a {CorrectItem} to defend yourself.";
            Hint = $"Find the {CorrectItem} to defeat {Name}";
        }
        public void DealDamage()
        {
            Random random = new Random();
            int damageDealt = random.Next(15, 30);
            Game.State.Player.Health =- damageDealt;
            Console.WriteLine($"The monster {Name} attacks, dealing {damageDealt} damage.");
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

        internal void TakeDamage()
        { 
            //if player is dead
            if (Game.State.Player.Health <= 0)
            {
                Game.State.Player.Dies();
            }
        }
    }
}
