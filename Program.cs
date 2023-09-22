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
    public Room(string name, string description)
    {
        Description = description;
        Name = name;
    }

    internal string HandleInput(string input)
    {
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
        Console.WriteLine($"To get away from this place, you have {String.Join(",", CurrentRoom.Exits.Keys)} as an option.");
        return;
    }
}

public static class Program
{
    public static void Main()
    {
        //setup
        var home = new Room("home", "You just woke up from bed. You can choose to stay and make coffee, go down the hatch to the basement, or take the door and go out.");
        var basement = new Room("basement", "You are now in the basement."); 
        var garden = new Room("garden", "You are now in your garden.");

        home.Exits.Add("basement", basement);
        home.Exits.Add("go out", garden);
        basement.Exits.Add("go back", home);

        // begin gameplay
        GameState.GameRunning = true;
        GameState.CurrentRoom = home;

        Console.WriteLine(GameState.CurrentRoom.Description);

        while (GameState.GameRunning)
        {
            string input = Console.ReadLine()!;

            var response = HandleInput(input);
            Console.Clear();
            Console.WriteLine(response);
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