using System.Text.Json.Serialization;


[JsonDerivedType(typeof(Key), typeDiscriminator: "Key")]
[JsonDerivedType(typeof(Sword), typeDiscriminator: "Sword")]
[JsonDerivedType(typeof(Heal), typeDiscriminator: "Heal")]


public class Item
{
    public string? Name { get; set; }
    public string? Hint { get; set; }

    public virtual string? DisplayHelp()
    {
        return Hint;
    }
}

public class Key : Item
{
    public int ID { get; set; }
    public Key(int id, string name)
    {
        ID = id;
        Name = name;
        Hint = $"Pick up the {Name}, by saying \"take {Name}\".";
    }
}

public class Sword : Item
{
    public Sword(string name)
    {
        Name = name;
        Hint = $"Pick up the {Name}, by saying \"take {Name}\".";
    }
}

public class Heal : Item
{
    public int Amount { get; set; }
    public string Description { get; set; }
    public Heal(string name, int amount, string description)
    {
        Name = name;
        Amount = amount;
        Description = $"Consume the \"{description}\"";
        Hint = $"Pick up the {Name} by saying \"take {Name}\".";
    }
}