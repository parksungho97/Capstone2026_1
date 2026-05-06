using UnityEngine;

public class Item
{
    public Item(string name, Sprite icon, string desc, int maxCount = 1)
    {
        Name = name;
        Icon = icon;
        Desc = desc;
        MaxCount = maxCount;
    }

    public string Name { get; private set; }
    public Sprite Icon { get; private set; }
    public string Desc { get; private set; }
    public int MaxCount { get; private set; }
}
