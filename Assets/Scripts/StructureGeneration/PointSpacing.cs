using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointSpacing
{
    public Vector2Int Origin { get; set; }
    public int UpSpace { get; set; }
    public int RightSpace { get; set; }
    public int DownSpace { get; set; }
    public int LeftSpace { get; set; }

    public PointSpacing(Vector2Int origin, int up, int right, int down, int left)
    {
        Origin = origin;
        UpSpace = up;
        RightSpace = right;
        DownSpace = down;
        LeftSpace = left;
    }

    public List<Direction> GetBlockedDirections()
    {
        List<Direction> directions = new List<Direction>();
        if (UpSpace == 0) directions.Add(Direction.Up);
        if (RightSpace == 0) directions.Add(Direction.Right);
        if (DownSpace == 0) directions.Add(Direction.Down);
        if (LeftSpace == 0) directions.Add(Direction.Left);
        return directions;
    }

    public override string ToString()
    {
        return $"Origin: {Origin}     Up: {UpSpace}, Right: {RightSpace}, Down: {DownSpace}, Left: {LeftSpace}";
    }
}

public enum Direction
{
    Up = 0,
    Right = 1,
    Down = 2,
    Left = 3
}
