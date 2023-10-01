using System.Runtime.InteropServices;

public class Player
{
    public static int Health = 100;
    public List<Item> Inventory = new List<Item>();
}

public class Room
{
    public string Description { get; set; }
    public Dictionary<string, Room> Exits { get; set; } = new();
    public string Slug { get; set; }
    public string Name { get; set; }
    public string DetailedDescription { get; set; }
    public Room(string slug, string name, string description, string detailedDescription, int id)
    {
        Slug = slug;
        Name = name;
        Description = description;
        DetailedDescription = detailedDescription;
    }

    internal string HandleInput(string input)
    {
        //command handler for specific room

        if (GameState.DetailedDescription(input))
        {
            return GameState.CurrentRoom.Description + GameState.CurrentRoom.DetailedDescription;
        }
        else if (Exits.TryGetValue(input, out var room))
        {
            GameState.CurrentRoom = room;
            GameState.VisitedRoom(room.Name);
            
            return room.Description;
        }
        else return null;
    }
}

public static class GameState
{
    public static Player Player { get; } = new Player();

    public static bool GameRunning;
    public static Room CurrentRoom { get; internal set; }

    static HashSet<string> visitedRooms = new HashSet<string>();

    public static void VisitedRoom(string roomName)
    {
        HashSet<string> visitedRooms = new HashSet<string>();
        visitedRooms.Add(roomName);
    }

    internal static string HelpPlayer()
    {
        return($"To get away from this place, you have {String.Join(", and ", CurrentRoom.Exits.Keys)} as an option.");
    }

    internal static bool DetailedDescription(string input)
    {
        if (input.ToLower() == "look")
        {
            return true;
        }
        else return false;
    }
}

public static class Program
{
    public static void Main()
    {
        //setup
        var home = new Room("homeSpawn", "home", "You just woke up from bed. You can choose to stay and make coffee, go down the hatch to the basement, or take the door and go out.",
            " When you look around, you can see a cozy, sunlit bedroom with artwork on the walls. There's a comfortable bed, a nightstand, a dresser, and a window with white curtains. A reading nook with an armchair and a bookshelf is nearby.", 1);
        var 
            basement = new Room("homeBasement", "basement", "You are now in the basement.", " You look around and see a dimly lit space with cool air. It's filled with stored items, neatly arranged against the walls. There are shelves, boxes, and a workbench," +
            " hinting at various hobbies and pastimes."+" The air carries a faint scent of old books and wood. There is a key in the corner of the room.", 2); 
        
        var garden = new Room("garden", "garden", "You are now in your garden.", "You see a vibrant and lively outdoor space. It's adorned with an array of colorful flowers, blooming bushes, and lush greenery." +
            " The gentle rustle of leaves and the occasional chirping of birds fill the air. A well-tended path winds through the garden, inviting you to explore its beauty. There's a mix of fragrances from various flowers, adding to the pleasant atmosphere." +
            "There is a big rusty gate that is covered in vines and greenery."
            , 3);

        home.Exits.Add("basement", basement);
        home.Exits.Add("go down", basement);
        home.Exits.Add("go out", garden);
        basement.Exits.Add("go back", home);
        basement.Exits.Add("back", home);
        basement.Exits.Add("go up", home); basement.Exits.Add("go back", home);
        garden.Exits.Add("go inside", home);


        // begin gameplay
        GameState.GameRunning = true;

        MainScreen();

        GameState.CurrentRoom = home;

        Console.WriteLine(GameState.CurrentRoom.Description);

        while (GameState.GameRunning)
        {
            string input = Console.ReadLine()!;
            string response = HandleInput(input);
            if (response == null)
            {   
                Console.WriteLine("Unrecognized command.");
            }
            else
            {
                Console.Clear();
            }
            Console.WriteLine(response);
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
        string HandleInput(string input)
        {
            // try handle global command

            // if we can't, pass it to the room
            if (input.ToLower() == "quit" || input.ToLower() == "exit")
            {
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
                    Console.WriteLine($"{item}");
                }
            }
            return GameState.CurrentRoom.HandleInput(input);
        }
    }
}