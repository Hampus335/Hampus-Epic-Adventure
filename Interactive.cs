using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hampus_Epic_Adventure;

public class InteractiveItem
{
    public string Name { get; set; }

    public virtual void SayHello()
    {
        Console.WriteLine("hello" + Name);
    }
}

public class Door : InteractiveItem
{
    private void WriteInfo()
    {
        Console.WriteLine("Hello, this is a door that can not be used at the moment");
    }
}