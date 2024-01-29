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
            Description = $"There is a monster named {Name} here.";
            Hint = $"Find the {CorrectItem} to defeat {Name}";
        }
        public void DealDamage()
        {
            //if player is dead
            if (Game.State.Player.Health <= 0)
            {
                Game.State.Player.Dies();
            }
            else
            {
                Random random = new Random();
                int damageDealt = random.Next(25, 35);
                Game.State.Player.Health = Game.State.Player.Health - damageDealt;
                Console.WriteLine($"The monster {Name} attacks, dealing {damageDealt} damage.");
                Console.WriteLine($"You now have {Game.State.Player.Health} health left.");
            }
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
            Random random = new Random();
            int damageDealt = random.Next(25, 45);
            Game.State.CurrentRoom.Monster.Health = -damageDealt;
            Console.WriteLine($"You attack {Name} with your {CorrectItem}, dealing {damageDealt} damage.");
        }
    }
}
