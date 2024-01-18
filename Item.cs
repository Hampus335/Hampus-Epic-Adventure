using System.Text.Json.Serialization;


[JsonDerivedType(typeof(Key), typeDiscriminator: "Key")]
public class Item
{
    public string? Name { get; set; }

    public virtual string? DisplayHelp()
    {
        return null;
    }
}

public class Key : Item
{
    public int ID { get; set; }
    public string ItemHint { get; set; }
    public Key(int id, string name)
    {
        ID = id;
        Name = name;
        ItemHint = $"Pick up the {Name}, by saying \"take {Name}\".";
    }
    public override string? DisplayHelp()
    {
        return ItemHint;
    }
}