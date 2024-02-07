
using System.Text.Json.Serialization;


[JsonDerivedType(typeof(Key), typeDiscriminator: "Key")]
[JsonDerivedType(typeof(Sword), typeDiscriminator: "Sword")]
[JsonDerivedType(typeof(Heal), typeDiscriminator: "Heal")]


public class Item
{
    public string? Name { get; set; }
    public string? Hint { get; set; }

    public string? Description { get; set; }

    public virtual string? DisplayHelp()
    {
        return Hint;
    }
}

public class Key : Item
{
    public int ID { get; set; }
    public Key(int id, string name, string description)
    {
        ID = id;
        Name = name;
        Hint = $"Pick up the {Name}, by saying \"take {Name}\".";
        Description = ", " + description;
    }
}

public class Sword : Item
{
    public Sword(string name, string description)
    {
        Name = name;
        Hint = $"Pick up the {Name}, by saying \"take {Name}\".";
        Description = ", " + description;
    }
}

public class Heal : Item
{
    public int Amount { get; set; }
    public Heal(string name, int amount, string description)
    {
        Name = name;
        Amount = amount;
        Description = ", " + description;
        Hint = $"Pick up the {Name} by saying \"take {Name}\".";
    }
}
