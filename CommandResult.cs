using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hampus_Epic_Adventure
{
    public record CommandResult(string? Text = null, bool ClearScreen = true);
}
