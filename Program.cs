using System.Runtime.InteropServices;

public static class Player
{
    public static int health = 10;
}



public class Room
{
    public string Description { get; set; }
    public Dictionary<string, Room> Exits { get; set; } = new();
    public string Name { get; set; }
    public string DetailedDescription { get; set; }
    public int ID { get; set; }
    public Room(string name, string description, string detailedDescription, int id)
    {
        Description = description;
        Name = name;
        DetailedDescription = detailedDescription;
        ID = id;
    }

    internal string HandleInput(string input)
    {
        GameState.DetailedDescription(input);
        if (Exits.TryGetValue(input, out var room))
        {
            GameState.CurrentRoom = room;
            GameState.VisitedRoom(room.Name);
            
            return room.Description;
        }
        return null;
    }
}

public static class GameState
{
    public static bool GameRunning;

    public static Room CurrentRoom { get; internal set; }

    static HashSet<string> visitedRooms = new HashSet<string>();

    public static void VisitedRoom(string roomName)
    {
        HashSet<string> visitedRooms = new HashSet<string>();
        visitedRooms.Add(roomName);
    }

    internal static void HelpPlayer()
    {
        Console.WriteLine($"To get away from this place, you have {String.Join(", and", CurrentRoom.Exits.Keys)} as an option.");
        return;
    }

    internal static void DetailedDescription(string input)
    {
        if (input.ToLower() == "look")
        {
            Console.WriteLine(CurrentRoom.DetailedDescription);
        }
    }
}

public static class Program
{
    public static void Main()
    {
        //setup
        var home = new Room("home", "You just woke up from bed. You can choose to stay and make coffee, go down the hatch to the basement, or take the door and go out.", "A cozy, sunlit bedroom with artwork on the walls. There's a comfortable bed, a nightstand, a dresser, and a window with white curtains. A reading nook with an armchair and a bookshelf is nearby.", 1);
        var basement = new Room("basement", "You are now in the basement.", "A dimly lit space with cool air. It's filled with stored items, neatly arranged against the walls. There are shelves, boxes, and a workbench, hinting at various hobbies and pastimes. The air carries a faint scent of old books and wood.", 2); 
        var garden = new Room("garden", "You are now in your garden.", "A vibrant and lively outdoor space. It's adorned with an array of colorful flowers, blooming bushes, and lush greenery. The gentle rustle of leaves and the occasional chirping of birds fill the air. A well-tended path winds through the garden, inviting you to explore its beauty. There's a mix of fragrances from various flowers, adding to the pleasant atmosphere.", 3);

        home.Exits.Add("basement", basement);
        home.Exits.Add("go out", garden);
        basement.Exits.Add("go back", home);
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
                Console.WriteLine("Unrecognized command");
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
            Console.WriteLine("your surroundings. Typing \"inventory\" tells you what you're carrying.");
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
                GameState.HelpPlayer();
            }
            return GameState.CurrentRoom.HandleInput(input);
        }
    }

}