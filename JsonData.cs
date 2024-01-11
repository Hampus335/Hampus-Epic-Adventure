using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hampus_Epic_Adventure;

public class JsonData
{
    public PlayerData PlayerData { get; set; }
    public WorldData WorldData { get; set; }
}

public class PlayerData
{
    public Player? Player { get; set; }
    public string? RoomSlug { get; set; }
}

public class WorldData
{
    public List<Room> Rooms { get; set; }
}