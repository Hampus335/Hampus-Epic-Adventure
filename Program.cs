using System.Runtime.InteropServices;

using Hampus_Epic_Adventure;

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
    public Item? Item { get; set; }
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

        //checks if player has visited room       
        if (GameState.CurrentRoom.Exits.TryGetValue(input, out Room room))
          {                
            if (GameState.CheckVisitedRoom(room.Slug))
            {
                GameState.CurrentRoom = room;
                GameState.RegisterRoom(GameState.CurrentRoom);
                return new CommandResult(GameState.CurrentRoom.Name, ClearScreen: false); 
            }
            
            else
            {
                GameState.CurrentRoom = room;
                GameState.RegisterRoom(GameState.CurrentRoom);
                return new CommandResult(GameState.CurrentRoom.Description, ClearScreen: false);
            }       
          } 

        if ("take " + GameState.CurrentRoom.Item?.Name == input.ToLower())
        {
            GameState.Player.Inventory.Add(GameState.CurrentRoom.Item);
            GameState.CurrentRoom.Item = null;
            return new CommandResult("You picked up a " + GameState.Player.Inventory.Last().Name.ToString(), ClearScreen: false);
        }
        else return new CommandResult("unknown", ClearScreen: true);
    }
}

public static class GameState
{
    public static Player Player { get; } = new Player();

    public static bool GameRunning;
    public static Room CurrentRoom { get; internal set; }

    static HashSet<string> VisitedRooms = new HashSet<string>();

    public static void RegisterRoom(Room room)
    {
        VisitedRooms.Add(room.Slug);
    }

    public static bool CheckVisitedRoom(string room)
    {
        //check if room has been visited
        if (VisitedRooms.Contains(room))
        {
            return true;
        }
        else return false;
    }


    internal static CommandResult HelpPlayer()
    {
        //return new CommandResult($"To get away from this place, you have {Utils.JoinAnd(", ", "and", CurrentRoom.Exits.Keys)} as an option.", ClearScreen: false);
        var distinctExits = CurrentRoom.Exits.DistinctBy(x => x.Value.Slug).Select(x => x.Key);
        string result = null;
        foreach (var v in distinctExits)
        {
            result += ", "+v;
        }
        return new CommandResult(result.ToString() + " will get you out of this place.", ClearScreen: false);
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

internal class Utils
{
    internal static object JoinAnd(string v1, string v2, Dictionary<string, Room>.KeyCollection keys)
    {
        throw new NotImplementedException();
    }
}

public static class Program
{
    public static void Main()
    {
        //setup
        var home = new Room("homeSpawn", "Home", "You just woke up from bed. You can choose to stay and make coffee, go down the hatch to the basement, or take the door and go out.",
            " When you look around, you can see a cozy, sunlit bedroom with artwork on the walls. There's a comfortable bed, a nightstand, a dresser, and a window with white curtains. A reading nook with an armchair and a bookshelf is nearby.");
        var
            basement = new Room("homeBasement", "Basement", "You are now in the basement.", " You look around and see a dimly lit space with cool air. It's filled with stored items, neatly arranged against the walls. There are shelves, boxes, and a workbench," +
            " hinting at various hobbies and pastimes." + " The air carries a faint scent of old books and wood. There is a key in the corner of the room."); 
        
        var garden = new Room("garden", "Garden", "You are now in your garden.", "You see a vibrant and lively outdoor space. It's adorned with an array of colorful flowers, blooming bushes, and lush greenery." +
            " The gentle rustle of leaves and the occasional chirping of birds fill the air. A well-tended path winds through the garden, inviting you to explore its beauty. There's a mix of fragrances from various flowers, adding to the pleasant atmosphere." +
            " There is a big rusty gate that is covered in vines and greenery.");

        home.Exits.Add("basement", basement);
        home.Exits.Add("go down", basement);
        home.Exits.Add("go out", garden);
        basement.Exits.Add("go back", home);
        basement.Exits.Add("back", home);
        basement.Exits.Add("go up", home); 
        garden.Exits.Add("go inside", home);
        basement.Item = new Item();
        basement.Item.Name = "key";

        GameState.CurrentRoom = home;
        GameState.RegisterRoom(home);

        // begin gameplay
        GameState.GameRunning = true;
        MainScreen();

        Console.WriteLine(GameState.CurrentRoom.Description);

        while (GameState.GameRunning)
        {
            string input = Console.ReadLine()!;
            //string response = HandleInput(input);
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