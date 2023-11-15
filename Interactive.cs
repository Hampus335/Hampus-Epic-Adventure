using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hampus_Epic_Adventure;

public abstract class InteractiveItem
{
    public string Name { get; set; }

    public abstract CommandResult HandleInput(string input);
}

public class Door : InteractiveItem
{
    public int DoorID { get; set; }
    public Key Key { get; set; }
    public bool DoorOpen { get; set; }
    public Room DoorLeadsTo { get; set; }

    public Door(int doorID, Room doorLeadsTo, Key key)
    {
        DoorID = doorID;
        DoorLeadsTo = doorLeadsTo;
        Key = key;
    }

    public override CommandResult HandleInput(string input)
    {
        if (input.ToLower() == "go through door")
        {
            if (DoorOpen)
            {
                Console.WriteLine("Going through door.");
            }
        }

        if (input.ToLower() == "use key")
        {
            if (GameState.Player.Inventory.Contains(Key) && Key.ID == DoorID)
            {
                DoorOpen = true;
                Console.WriteLine("Door is now open.");
            }
            else if (GameState.Player.Inventory.Contains(Key))
            {
                Console.WriteLine("This is not the right key for this door.");
            }
            else if (!GameState.Player.Inventory.Contains(Key))
            {
                Console.WriteLine("You need a key here.");
            }

            return new CommandResult(Text: null, ClearScreen: false);
        }
        else return new CommandResult("unknown", ClearScreen: false);
    }
}


