using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hampus_Epic_Adventure;

public abstract class InteractiveItem
{
    public abstract CommandResult HandleInput(string input);
}

public class Door : InteractiveItem
{
    public int DoorID { get; set; }
    public Key Key { get; set; }
    public bool DoorOpen { get; set; }
    public string DoorLeadsToSlug { get; set; }
    public string DoorName { get; set; }
    public string KeyName { get; set; }
    public Door(int doorID, string doorLeadsToSlug, Key key, string doorName, string keyName)
    {
        DoorID = doorID;
        DoorLeadsToSlug = doorLeadsToSlug;
        Key = key;
        DoorName = doorName;
        KeyName = keyName;
    }

    public override CommandResult HandleInput(string input)
    {
        if (input.ToLower() == $"go through the {DoorName.ToLower()}" || input.ToLower() == $"go through {DoorName.ToLower()}" || input.ToLower() == $"use {DoorName.ToLower()}" || input.ToLower() == $"enter {DoorName.ToLower()}" || input.ToLower() == $"enter the {DoorName.ToLower()}")
        {
            if (DoorOpen)
            {
                Game.State.MoveToRoom(DoorLeadsToSlug);
                return new CommandResult(Text: $"Going through {DoorName}.", ClearScreen: false, RecognizedCommand: true);
            }
        }

        if (input.ToLower() == $"use {KeyName.ToLower()}" || input.ToLower() == $"unlock {DoorName.ToLower()}" || input.ToLower() == $"open {DoorName.ToLower()}" || input.ToLower() == $"open the {DoorName.ToLower()}" || input.ToLower() == $"unlock the {DoorName.ToLower()}" || input.ToLower() == $"use the {KeyName.ToLower()}")
        {
            if (Game.State.Player.Inventory.Any(item => item is Key key) && Key.ID == DoorID)
            {
                DoorOpen = true;
                Game.State.Player.Inventory.Remove(Key);
                return new CommandResult(Text: $"The {DoorName} is now open.", ClearScreen: false, RecognizedCommand: true);
            }
            else if (Game.State.Player.Inventory.Contains(Key))
            {
                return new CommandResult(Text: $"This is not the right {KeyName.ToLower()} for this {DoorName}.", ClearScreen: false, RecognizedCommand: true);
            }
            else
            {
                return new CommandResult(Text: $"You need a {KeyName.ToLower()} here.", ClearScreen: false, RecognizedCommand: true);
            }
        }
        else return new CommandResult(ClearScreen: false, RecognizedCommand: false  );
    }
}


