﻿using System;
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
    public Room DoorLeadsTo { get; set; }
    public string DoorName { get; set; }
    public string KeyName { get; set; }
    public Door(int doorID, Room doorLeadsTo, Key key, string doorName, string keyName)
    {
        DoorID = doorID;
        DoorLeadsTo = doorLeadsTo;
        Key = key;
        DoorName = doorName;
        KeyName = keyName;
    }

    public override CommandResult HandleInput(string input)
    {
        if (input.ToLower() == $"go through {DoorName.ToLower()}" || input.ToLower() == $"use {DoorName.ToLower()}")
        {
            if (DoorOpen)
            {
                Game.State.MoveToRoom(DoorLeadsTo);
                return new CommandResult(Text: $"Going through {DoorName}.", ClearScreen: false, RecognizedCommand: true);
            }
        }

        if (input.ToLower() == "use key" || input.ToLower() == $"unlock {DoorName.ToLower()}")
        {
            if (Game.State.Player.Inventory.Contains(Key) && Key.ID == DoorID)
            {
                DoorOpen = true;
                Game.State.Player.Inventory.Remove(Key);
                return new CommandResult(Text: $"The {DoorName} is now open.", ClearScreen: false, RecognizedCommand: true);
            }
            else if (Game.State.Player.Inventory.Contains(Key))
            {
                return new CommandResult(Text: $"This is not the right key for this {DoorName}.", ClearScreen: false, RecognizedCommand: true);
            }
            else
            {
                return new CommandResult(Text: "You need a key here.", ClearScreen: false, RecognizedCommand: true);
            }
        }
        else return new CommandResult(null, ClearScreen: false, RecognizedCommand: false);
    }
}


