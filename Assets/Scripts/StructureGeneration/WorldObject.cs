using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObject
{
    public GameObject GameObject { get; set; }
    public WorldObjectType Type { get; set; }
    public int RoomId { get; set; }


    public WorldObject(GameObject gameObject, WorldObjectType type, int roomId)
    {
        this.GameObject = gameObject;
        this.Type = type;
        this.RoomId = roomId;
    }

    public WorldObject(int roomId)
    {
        this.RoomId = roomId;
    }

    public override string ToString()
    {
        return $"Room Id: {RoomId}";
    }
}

public enum WorldObjectType
{
    Unit,
    HalfStructure,
    FullStructure
}
