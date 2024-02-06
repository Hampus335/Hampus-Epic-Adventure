using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Serialization;

using Hampus_Epic_Adventure;

public class Player
{
    public int Health { get; set; } = 100;
    public List<Item> Inventory { get; set; } = new List<Item>();

    internal void Dies()
    {
        Console.WriteLine("You have now died");
        Console.WriteLine("Game Over");
        Game.State.GameRunning = false;
    }
}

    public class Room
    {
        public string Description { get; set; }
        public Dictionary<string, string> Exits { get; set; } = new();
        public string Slug { get; set; }
        public string Name { get; set; }
        public string DetailedDescription { get; set; }
        public Item? Item { get; set; }
        public List<InteractiveItem> Interactives { get; set; } = new();
        public Monster? Monster { get; set; }
    public Room(string slug, string name, string description, string detailedDescription, Item? item = null, Monster? monster = null)
        {   
            Slug = slug;
            Name = name;
            Description = description;
            DetailedDescription = detailedDescription;
            Item = item;
            Monster = monster;
        }
    internal CommandResult HandleInput(string input)
    {
        //command handler for specific room
        if (Game.State.DetailedDescription(input))
        {
            return new CommandResult(Game.State.CurrentRoom.Description + " " + Game.State.CurrentRoom.DetailedDescription, ClearScreen: false, RecognizedCommand: true);
        }

        if (Game.State.CurrentRoom.Exits.TryGetValue(input, out string destination))
        {
            CommandResult result = Game.State.MoveToRoom(destination);
            return result;
        }

        //check if player is trying to use interactive in the specific room
        foreach (InteractiveItem item in Interactives)
        {
            var result = item.HandleInput(input);
            return result;
        }

        if ("take " + Game.State.CurrentRoom.Item?.Name.ToLower() == input.ToLower() || "grab " + Game.State.CurrentRoom.Item?.Name.ToLower() == input.ToLower() || "take the " + Game.State.CurrentRoom.Item?.Name.ToLower() == input.ToLower() || "grab the " + Game.State.CurrentRoom.Item?.Name.ToLower() == input.ToLower() || "pick up the " + Game.State.CurrentRoom.Item?.Name.ToLower() == input.ToLower() || "pickup the " + Game.State.CurrentRoom.Item?.Name.ToLower() == input.ToLower() || "pick up " + Game.State.CurrentRoom.Item?.Name.ToLower() == input.ToLower() || "pickup " + Game.State.CurrentRoom.Item?.Name.ToLower() == input.ToLower())
        {
            Game.State.Player.Inventory.Add(Game.State.CurrentRoom.Item);
            Game.State.CurrentRoom.Item = null;
            return new CommandResult(Text: "You picked up a " + Game.State.Player.Inventory.Last().Name.ToString().ToLower(), ClearScreen: false, RecognizedCommand: true);
        }
        else return new CommandResult(ClearScreen: false, RecognizedCommand: false);
    }

    internal void OnEnter()
    {
        if (Game.State.CurrentRoom.Monster != null)
        {   
            HandleMonster();         
        }
    }

    private void HandleMonster()
    {
        Random random = new Random();

        if (Game.State.Player.Inventory.Any(item => item is Sword sword))//behöver göra så att det inte är endast svärd, kan vara andra items än svärd med
        {
            if (random.Next(0, 101) > 55)
            {
                Console.WriteLine(Monster.Description);

                while (Monster.Health >= 0)
                {
                    Monster.DealDamage();

                    //see if player has item to regain health
                    Item? health = Game.State.Player.Inventory.FirstOrDefault(x => x is Heal);
                    if (health != null)
                    {
                        Console.WriteLine($"You can use your {Monster.CorrectItem} to defend yourself from {Monster.Name}, or eat your {health.Name} to increase health. (Say \"Eat {health.Name}\"");
                    }
                    else
                    {
                        Console.WriteLine($"You can use your {Monster.CorrectItem} to defend yourself from {Monster.Name}, or flee to search for something to increase your health with if needed, by saying \"go back\"");
                    }

                    string? input = Console.ReadLine().ToLower();

                    if (input == $"use {Monster.CorrectItem}".ToLower() || input == $"use the {Monster.CorrectItem}".ToLower())
                    {
                        Monster.TakeDamage();
                    }
                    else if (input == "help")
                    {
                        Monster.DisplayHelp(health);
                    }

                    else if (input == "go back")
                    {
                        //Game.State.MoveToRoom("forest1");
                        Game.State.MoveToRoom(Game.State.LastRoom.Slug);
                        break;
                    }
                }

                if (Game.State.CurrentRoom.Monster.Health <= 0)
                {
                    Monster = null;
                    Console.WriteLine("Press any key to continue");
                    Console.ReadKey();
                }
            }
         //else no monster spawned, so we can enter the room without issues 
         else return;
        }
        
        else
        //the player has no sword, so the chance of not having a monster is reduced
        {
            if (random.Next(0, 101) > 20)
            {
                while (Monster.Health >= 0)
                {
                    //chance of fleeing the monster
                    if (random.Next(0, 10) < 3)
                    {
                        break;
                    }

                    Monster.DealDamage();

                    //see if player has item to regain health
                    Item? health = Game.State.Player.Inventory.FirstOrDefault(x => x is Heal);
                    if (health != null)
                    {
                        Console.WriteLine($"If you want to escape from {Monster.Name}, either go back to try and find the sword by saying \"go back\" or continue trying to fight {Monster.Name} without a {Monster.CorrectItem} by pressing enter.");
                        Console.WriteLine($"You can also eat your {health.Name} to increase health. (Say \"Eat {health.Name}\"");
                    }
                    else
                    {
                        Console.WriteLine($"If you want to escape from {Monster.Name}, either go back to try and find the sword by saying \"go back\" or continue trying to fight {Monster.Name} without a {Monster.CorrectItem} by pressing enter.");
                    }


                    if (Console.ReadLine().ToLower() == "go back")
                    {
                        Game.State.MoveToRoom(Game.State.LastRoom.Slug);
                        break;
                    }
                }
            }
        }
    }
}

public class GameState
{
    public List<Room> Rooms { get; set; } = new List<Room> ();
    public Player Player { get; } = new Player();

    public bool GameRunning;
    public Room CurrentRoom { get; internal set; }

    static HashSet<string> VisitedRooms = new HashSet<string>();
    public Room? LastRoom;

    public void VisitRoom(Room room)
    {
        VisitedRooms.Add(room.Slug);
    }

    public bool CheckVisitedRoom(string room)
    {
        //check if room has been visited
        if (VisitedRooms.Contains(room))
        {
            return true;
        }
        else return false;
    }

    internal CommandResult HelpPlayer()
    {
        List<string> interactiveHelp = new List<string>();
        foreach (InteractiveItem g in CurrentRoom.Interactives)
        {
            var help = g.DisplayHelp();
            if (help is not null)
            interactiveHelp.Add(help);
        }
        //the room has interactives but no item
        if (interactiveHelp.Count > 0 && CurrentRoom.Item == null)
        {
            return new CommandResult("To get out of this place, you can say " + String.Join(", and ", CurrentRoom.Exits.DistinctBy(x => x.Value).Select(x => x.Key)) + $" or {String.Join(", or", interactiveHelp.Distinct()).ToLower()}", ClearScreen: false, RecognizedCommand: true);
        }

        //the room has an item but no interactives
        if (CurrentRoom.Item != null && interactiveHelp.Count == 0)
        {
            return new CommandResult("To get out of this place, you can say " + String.Join(", and ", CurrentRoom.Exits.DistinctBy(x => x.Value).Select(x => x.Key)) + $", or {CurrentRoom.Item.DisplayHelp().ToLower()}", ClearScreen: false, RecognizedCommand: true);
        }

        //the room has both an item and interactives
        if (CurrentRoom.Item != null && interactiveHelp.Count > 0)
        {
            return new CommandResult("To get out of this place, you can say " + String.Join(", and ", CurrentRoom.Exits.DistinctBy(x => x.Value).Select(x => x.Key)) + $" or{String.Join(", or", interactiveHelp.Distinct()).ToLower()}" + $" or {CurrentRoom.Item.DisplayHelp().ToLower()}", ClearScreen: false, RecognizedCommand: true);
        }

        //the room has neither an item nor an interactive
        return new CommandResult("To get out of this place, you can say " + String.Join(", and ", CurrentRoom.Exits.DistinctBy(x => x.Value).Select(x => x.Key)), ClearScreen: false, RecognizedCommand: true);
    }

    internal bool DetailedDescription(string input)
    {
        if (input.ToLower() == "look")
        {
            return true;
        }
        else return false;
    }

    internal CommandResult MoveToRoom(string destination)
    {
        //changes room with the slug as identifyer
        Room? room = Game.State.Rooms.FirstOrDefault(room => room.Slug == destination);
        LastRoom = CurrentRoom;
        CurrentRoom = room;

        CurrentRoom.OnEnter();

        var text = CheckVisitedRoom(destination)
            ? CurrentRoom.Name
            : CurrentRoom.Description;

        VisitRoom(CurrentRoom);
        return new CommandResult(text, ClearScreen: true);
    }

    internal CommandResult MoveToPreviousRoom(string destination)
    {
        if (LastRoom != null)
        {
            return MoveToRoom(LastRoom.Slug);
        }
        else return new CommandResult(Text: "There is no previous room available", ClearScreen: false, RecognizedCommand: true);
    }

    internal void SaveGame()
    {
        PlayerData playerData = new PlayerData()
        {
            RoomSlug = CurrentRoom.Slug,
            Player = Game.State.Player,
        };

        WorldData worldData = new WorldData()
        {
            Rooms = Rooms
        };
        string jsonDataPlayer = JsonSerializer.Serialize(playerData);
        string fileNamePlayer = "PlayerData.json";
        File.WriteAllText(fileNamePlayer, jsonDataPlayer);

        string jsonDataWorld = JsonSerializer.Serialize(worldData);
        string fileNameWorld = "WorldData.json";
        File.WriteAllText(fileNameWorld, jsonDataWorld);
    }

    public JsonData LoadGame()
    {
        if (File.Exists(@"PlayerData.json"))
        {
            var savedPlayerData = File.ReadAllText(@"PlayerData.json");
            var jsonPlayerOutput = JsonSerializer.Deserialize<PlayerData>(savedPlayerData);

            var savedWorldData = File.ReadAllText(@"WorldData.json");
            var jsonWorldOutput = JsonSerializer.Deserialize<WorldData>(savedWorldData);

            JsonData jsonData = new JsonData
            {
                PlayerData = jsonPlayerOutput,
                WorldData = jsonWorldOutput
            };
            return jsonData;
        }
        else return null;
    }
}

public static class Program
{
    public static void Main()
    {
        var savedGameData = Game.State.LoadGame();
        if (savedGameData != null)
        {
            //player has played before, so we load the world and player data
            ConfigureGameStateFromJson(savedGameData);
        }
        else
        {
            //the player has not played before, so we load the spawnpoint
            LoadStartpoint();
        }
        Game.State.GameRunning = true;
        MainScreen();
        Console.WriteLine(Game.State.CurrentRoom.Description);
        Console.Write(" > ");
        
        while (Game.State.GameRunning)
        {
            string input = Console.ReadLine()!;
            Console.WriteLine();
            CommandResult response = HandleInput(input);

            if (response.ClearScreen)
            {
                Console.Clear();
            }
            else if (!response.RecognizedCommand)
            {
                Console.WriteLine("Unrecognized command.");
            }
            if (response.Text != null)
            {
                Console.WriteLine(response.Text);
            }
            Console.Write(" > ");
        }

        static void MainScreen()
        {
            Console.WriteLine("Welcome to Hampus Epic Adventure");

            Console.WriteLine("To play the game, type short phrases into the command line.");
            Console.WriteLine("If you type \"look\", the game gives you a description of");
            Console.WriteLine("your surroundings which may be neccessary for you to solve some puzzles. Typing \"show inventory\" tells you what you're carrying.");
            Console.WriteLine("If you want to go back, you can say \"go back\", and you will go back to the previous room if there is one");
            Console.WriteLine("Type \"help\" if you are not able to move on from where you are.");
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
            Console.Clear();
        }
        CommandResult HandleInput(string input)
        {
            // try handle global command

            // if we can't, pass it to the room
            if (input.ToLower() == "quit" || input.ToLower() == "exit")
            {
                Game.State.SaveGame();
                Environment.Exit(0);
            }
            if (input.ToLower() == "help")
            {
                return Game.State.HelpPlayer();
            }

            if (input.ToLower() == "go back" || input.ToLower() == "back")
            {
                return Game.State.MoveToPreviousRoom(input);
            }

            if (input.ToLower() == "show inventory" || input.ToLower() == "inventory")
            {
                foreach (var item in Game.State.Player.Inventory)
                {
                    Console.WriteLine($" -  {item.Name}");
                }
                if (Game.State.Player.Inventory.Count < 1)
                {
                    Console.WriteLine($" -  Your inventory is empty.");
                }
                return new CommandResult(Text: null, ClearScreen: false, RecognizedCommand: true);
            }
            return Game.State.CurrentRoom.HandleInput(input);
        }
    }

    private static void LoadStartpoint()
    {
        //first time that the player is playing, so we load the standard room
        var savedWorldData = File.ReadAllText(@"WorldData.json");
        var worldData = JsonSerializer.Deserialize<WorldData>(savedWorldData);

        Game.State.Rooms = worldData.Rooms;
        Game.State.CurrentRoom = worldData.Rooms.First();
        Game.State.VisitRoom(Game.State.CurrentRoom);
    }

    private static void ConfigureGameStateFromJson(JsonData jsonData)
    {
        //find the correct room with the slug as identifyer and make it into CurrentRoom
        Game.State.Rooms = jsonData.WorldData.Rooms;
        Game.State.CurrentRoom = Game.State.Rooms.FirstOrDefault(room => room.Slug == jsonData.PlayerData.RoomSlug);
        Game.State.Player.Inventory = jsonData.PlayerData.Player.Inventory;
        Game.State.Player.Health = jsonData.PlayerData.Player.Health;
    }
}