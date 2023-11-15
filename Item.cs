public class Item
{
    public string Name { get; set; }
}

public class Key : Item
{
    public int ID { get; set; }

    public Key(int id, string name)
    {
        ID = id;
        Name = name;
    }
}