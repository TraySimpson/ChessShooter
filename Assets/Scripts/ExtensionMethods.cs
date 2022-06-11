using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    const int CAMERA_Z_OFFSET = 4;

    // Vector Extensions
    public static Vector2Int Get2DCoordinates(this GameObject gameObject) 
    {
        return new Vector2Int(
            Mathf.RoundToInt(gameObject.transform.position.x),
            Mathf.RoundToInt(gameObject.transform.position.z));
    }

    public static Vector3 ToVector3(this Vector2Int vector2)
    {
        return new Vector3(
            vector2.x,
            0,
            vector2.y);
    }

    public static Vector2Int Get2DCoords(this Vector3 vector)
    {
        return new Vector2Int(
            Mathf.RoundToInt(vector.x),
            Mathf.RoundToInt(vector.z)
        );
    }

    public static Vector3Int Get3DCoords(this Vector3 vector)
    {
        return new Vector3Int(
            Mathf.RoundToInt(vector.x),
            Mathf.RoundToInt(vector.z),
            Mathf.FloorToInt(vector.y)
        );
    }

    // List
    public static T Peek<T>(this List<T> list) 
    {
        return list.Count != 0 ? list[list.Count - 1] : default(T);
    }

    public static T GetAndRemove<T>(this List<T> list, int index) 
    {
        T item = list[index];
        list.RemoveAt(index);
        return item;
    }
}
