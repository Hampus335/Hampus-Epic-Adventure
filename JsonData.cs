using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hampus_Epic_Adventure;

internal class JsonData
{
    public Player Player { get; set; }
    public string currentRoomSlug { get; set; }

    public JsonData(Player player, string roomSlug)
    {
        Player = player;
        currentRoomSlug = roomSlug;
    }
}

