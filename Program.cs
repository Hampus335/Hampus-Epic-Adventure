using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

using Hampus_Epic_Adventure;

public class Player
{
    public int Health { get; set; } = 100;
    public List<Item> Inventory { get; set; } = new List<Item>();
}   

public class Room
{
    public string Description { get; set; }
    public Dictionary<string, Room> Exits { get; set; } = new();
    public string Slug { get; set; }
    public string Name { get; set; }
    public string DetailedDescription { get; set; }
    public Item? Item { get; set; }
    public List<InteractiveItem> Interactives = new List<InteractiveItem>();
    public Room(string slug, string name, string description, string detailedDescription, Item? item = null)
    {   
        Slug = slug;
        Name = name;
        Description = description;
        DetailedDescription = detailedDescription;
        Item = item;
    }
    internal CommandResult HandleInput(string input)
    {
        //command handler for specific room
        if (GameState.DetailedDescription(input))
        {
            return new CommandResult(GameState.CurrentRoom.Description + " " + GameState.CurrentRoom.DetailedDescription, ClearScreen: false);
        }
        if (GameState.CurrentRoom.Exits.TryGetValue(input, out Room room))
        {
            CommandResult result = GameState.MoveToRoom(room);
            return result;
        }

        //check if player is trying to use interactive in the specific room
        foreach (InteractiveItem item in Interactives)
        {
            var result = item.HandleInput(input);
            return result;
        }

        if ("take " + GameState.CurrentRoom.Item?.Name.ToLower() == input.ToLower())
        {
            GameState.Player.Inventory.Add(GameState.CurrentRoom.Item);
            GameState.CurrentRoom.Item = null;
            return new CommandResult("You picked up a " + GameState.Player.Inventory.Last().Name.ToString(), ClearScreen: false);
        }
        else return new CommandResult("unknown", ClearScreen: true);
    }
}

public class GameState
{
    public Player Player { get; } = new Player();

    public bool GameRunning;
    public Room CurrentRoom { get; internal set; }

    static HashSet<string> VisitedRooms = new HashSet<string>();

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
        return new CommandResult("To get out of this place, you can use " + String.Join(", and ", CurrentRoom.Exits.DistinctBy(x => x.Value.Slug).Select(x => x.Key)), ClearScreen: false);
    }

    internal bool DetailedDescription(string input)
    {
        if (input.ToLower() == "look")
        {
            return true;
        }
        else return false;
    }

    internal CommandResult MoveToRoom(Room room)
    {
        //return appropriate information if the player has visited the room. Returns a different text if the player hasn't visited the room previously.
        if (GameState.CheckVisitedRoom(room.Slug))
        {
            GameState.CurrentRoom = room;
            GameState.VisitRoom(GameState.CurrentRoom);
            return new CommandResult(GameState.CurrentRoom.Name, ClearScreen: false);
        }

        else
        {
            GameState.CurrentRoom = room;
            GameState.VisitRoom(GameState.CurrentRoom);
            return new CommandResult(GameState.CurrentRoom.Description, ClearScreen: false);
        }
    }

    internal void SaveGame(Player player)
    {
        string jsonString = JsonSerializer.Serialize(player);
        string fileName = "PlayerData.json";
        File.WriteAllText(fileName, jsonString);
    }
}


public static class Program
{
    public static void Main()
    {
        //setup
        var home = new Room("homeSpawn", "Bedroom", "You are now in your bedroom. You can either go down the hatch to the basement, or take the door and go out.",
            " When you look around, you can see a cozy, sunlit bedroom with artwork on the walls. There's a comfortable bed, a nightstand, a dresser, and a window with white curtains. A reading nook with an armchair and a bookshelf is nearby.");
        var basement = new Room("homeBasement", "Basement", "You are now in the basement.", " You look around and see a dimly lit space with cool air. It's filled with stored items, neatly arranged against the walls. There are shelves, boxes, and a workbench," +
            " hinting at various hobbies and pastimes." + " The air carries a faint scent of old books and wood. There is a key in the corner of the room."); 
        
        var garden = new Room("garden", "Garden", "You are now in your garden. There is a gate at the end of your garden.", "You see a vibrant and lively outdoor space. It's adorned with an array of colorful flowers, blooming bushes, and lush greenery." +
            " The gentle rustle of leaves and the occasional chirping of birds fill the air. A well-tended path winds through the garden, inviting you to explore its beauty. There's a mix of fragrances from various flowers, adding to the pleasant atmosphere." +
            " There is a big rusty gate that is covered in vines and greenery.");

        var forest = new Room("first part of forest outside the gate", "Forest", "You have ventured beyond the gate into a dense forest.",
                 " The towering trees create a natural canopy, casting dappled sunlight on the forest floor. A soft carpet of fallen leaves crunches beneath your feet as you explore." +
                 " The air is filled with the soothing sounds of rustling leaves, distant bird calls, and the occasional scampering of unseen creatures. A narrow trail leads deeper into the heart of the woods.");

        home.Exits.Add("basement", basement);
        home.Exits.Add("go down", basement);
        home.Exits.Add("go out", garden);
        basement.Exits.Add("go back", home);
        basement.Exits.Add("back", home);
        basement.Exits.Add("go up", home); 
        garden.Exits.Add("go inside", home);
        forest.Exits.Add("go to garden", garden);

        Key key = new Key(1, "Key");
        Door gate = new Door(1, forest, key, "gate");
        gate.DoorLeadsTo = forest;
        garden.Interactives.Add(gate);

        basement.Item = key;

        GameState.CurrentRoom = home;
        GameState.VisitRoom(home);

        // begin gameplay
        GameState.GameRunning = true;
        MainScreen();
        Console.WriteLine(GameState.CurrentRoom.Description);
        Console.Write(" > ");

        while (GameState.GameRunning)
        {
            string input = Console.ReadLine()!;
            Console.WriteLine();
            CommandResult response = HandleInput(input);
            if (response.Text == "unknown")
            {
                Console.WriteLine("Unrecognized command.");
            }
            else if (response.ClearScreen)
            {
                Console.Clear();
            }
            else
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
                GameState.SaveGame(GameState.Player);
                Environment.Exit(0);
            }
            if (input.ToLower() == "help")
            {
                return GameState.HelpPlayer();
            }
             
            if (input.ToLower() == "show inventory" || input.ToLower() == "inventory")
            {
                foreach (var item in GameState.Player.Inventory)           
                {
                    Console.WriteLine($" -  {item.Name}");         
                }
                return new CommandResult(Text: null, ClearScreen: false);
            }
            return GameState.CurrentRoom.HandleInput(input);
        }
    }
}