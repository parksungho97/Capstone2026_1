using Fusion;
using UnityEngine;

public class ItemData
{
    public ItemData(string name, Sprite icon, string desc, Mesh mesh = null, Material material = null)
    {
        Name = name;
        Icon = icon;
        Desc = desc;
        Mesh = mesh;
        Material = material;
    }

    public string Name { get; }
    public Sprite Icon { get; }
    public string Desc { get; }
    public Mesh Mesh { get; set; }
    public Material Material { get; set; }
}
