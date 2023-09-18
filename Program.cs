using System.Runtime.InteropServices;

public static class Player
{
    public static int health = 10;
}



public class Room
{
    internal string HandleInput(string input)
    {
        throw new NotImplementedException();
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
        Console.WriteLine("You are in a room. You can choose to stay and make coffee, go down the hatch to the basement, or take the door and go out.");

        while (GameState.GameRunning)
        {
            string input = Console.ReadLine()!;

            var response = HandleInput(input);
        }

        string HandleInput(string input)
        {
            // try handle global command

            // if we can't, pass it to the room
    
            return GameState.CurrentRoom.HandleInput(input);
        }
    }

}