using System;
using System.Net.Http.Headers;
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
            Random random = new Random();
            int damageDealt = random.Next(0, 35);

            if (damageDealt <= 7)
            {
                Console.WriteLine($"The monster {Name} attacks, but misses, so you don't take any damage.");
                Console.WriteLine($"You still have {Game.State.Player.Health} health left.");
            }
            else
            {
                Game.State.Player.Health = Game.State.Player.Health - damageDealt;
                Console.WriteLine($"The monster {Name} attacks, dealing {damageDealt} damage.");
                Console.WriteLine($"You now have {Game.State.Player.Health} health left.");
            }
        }

        public string DisplayHelp(Item health)
        {
            if (Game.State.Player.Inventory.Any(item => CorrectItem is Item correctItem))
            {
                Hint = $"Kill {Name} by using the {CorrectItem}. Say \"use {CorrectItem}\"";
                return Hint;
            }
            if (health != null && Game.State.Player.Health < 100)
            {
                Hint = $"Kill {Name} by using the {CorrectItem}. Say \"use {CorrectItem}\". If you need health, you can use your {health.Name} to regain health";
                return Hint;
            }
            else return Hint;
        }

        internal void TakeDamage()
        {
            Console.Clear();
            Random random = new Random();
            int damageDealt = random.Next(25, 45);
            Game.State.CurrentRoom.Monster.Health = Game.State.CurrentRoom.Monster.Health - damageDealt;

            if (Game.State.CurrentRoom.Monster.Health <= 0)
            {
                Console.WriteLine($"You attack {Name} with your {CorrectItem}, killing him completley.");
            }
            else
            {
                Console.WriteLine($"You attack {Name} with your {CorrectItem}, dealing {damageDealt} damage.");
                Console.WriteLine($"{Game.State.CurrentRoom.Monster.Name} has {Game.State.CurrentRoom.Monster.Health} health left.");
            }
        }
    }
}
