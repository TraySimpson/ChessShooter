using UnityEngine;

public class WorldEdgeObject
{
    public GameObject GameObject { get; set; }
    public WorldEdgeObjectType Type { get; set; }

    public WorldEdgeObject(GameObject gameObject, WorldEdgeObjectType type)
    {
        this.GameObject = gameObject;
        this.Type = type;
    }
}

public enum WorldEdgeObjectType
{
    HalfStructure,
    FullStructure,
    Door
}
