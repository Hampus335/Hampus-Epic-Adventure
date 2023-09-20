using System.Runtime.InteropServices;

public static class Player
{
    public static int health = 10;
}



public class Room
{
    public string Description { get; set; }
    public string Exits { get; set; }
    public Room(string description, string exits)
    {
        Description = description;
        Exits = exits;
    }

    internal string HandleInput(string input)
    {
        
    }
}

public static class GameState
{
    public static bool GameRunning;

    public static Room CurrentRoom { get; internal set; }
}

public static class Program
{
    public static void Main()
    {
        //setup
        var home = new Room("You just woke up from bed. You can choose to stay and make coffee, go down the hatch to the basement, or take the door and go out.", "north");
        var basement = new Room("You are now in the basement."); 
        var garden = new Room("You are now in your garden.");

        // begin gameplay
        GameState.GameRunning = true;
        GameState.CurrentRoom = home;

        Console.WriteLine(GameState.CurrentRoom.Description);

        while (GameState.GameRunning)
        {
            string input = Console.ReadLine()!;

            var response = HandleInput(input);
        }

        string HandleInput(string input)
        {
            // try handle global command

            // if we can't, pass it to the room
            if (input.ToLower() == "quit" || input.ToLower() == "exit")
            {
                Environment.Exit(0);
            }
            return GameState.CurrentRoom.HandleInput(input);
        }
    }

}