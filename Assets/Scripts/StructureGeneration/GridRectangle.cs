using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridRectangle
{
    public int Height { get; set; }
    public int Width { get; set; }
    // Bottom Left coordinate (inclusive)
    public Vector2Int Origin { get; set; }

    public GridRectangle(Vector2Int origin, int width, int height)
    {
        this.Origin = origin;
        this.Height = height;
        this.Width = width;
    }
    public GridRectangle(int originX, int originY, int width, int height)
    {
        this.Origin = new Vector2Int(originX, originY);
        this.Height = height;
        this.Width = width;
    }

    public int GetMinX() => Origin.x;
    public int GetMinY() => Origin.y;
    public int GetMaxX() => Origin.x + Width - 1;
    public int GetMaxY() => Origin.y + Height - 1;

    public List<Vector2Int> GetAllEdgePositions(int offset = 1, GridRectangle bounds = null) {
        List<Vector2Int> edgePositions = new List<Vector2Int>();

        return edgePositions;
    }

    public Vector2Int GetRandomEdgePosition(int offset = 0, GridRectangle bounds = null)
    {
        List<Direction> blockedDirections = GetOutOfBoundsDirections(bounds);
        Direction direction;
        do
        {
            direction = (Direction)Random.Range(0, 4);
        } while (blockedDirections.Contains(direction));
        int xValue = 0;
        int yValue = 0;
        switch (direction)
        {
            case Direction.Up:
                xValue = Random.Range(GetMinX(), GetMaxX());
                yValue = GetMaxY() + offset;
                break;
            case Direction.Right:
                xValue = GetMaxX() + offset;
                yValue = Random.Range(GetMinY(), GetMaxY());
                break;
            case Direction.Down:
                xValue = Random.Range(GetMinX(), GetMaxX());
                yValue = GetMinY() - offset;
                break;
            default:
                xValue = GetMinX() - offset;
                yValue = Random.Range(GetMinY(), GetMaxY());
                break;
        }
        return new Vector2Int(xValue, yValue);
    }

    public List<Direction> GetOutOfBoundsDirections(GridRectangle bounds, int offset = 1)
    {
        List<Direction> blockedDirections = new List<Direction>();
        if (bounds is null) return blockedDirections;

        if (GetMinX() - offset < bounds.GetMinX())
            blockedDirections.Add(Direction.Left);
        if (GetMinY() - offset < bounds.GetMinY())
            blockedDirections.Add(Direction.Down);
        if (GetMaxX() + offset > bounds.GetMaxX())
            blockedDirections.Add(Direction.Right);
        if (GetMaxY() + offset > bounds.GetMaxY())
            blockedDirections.Add(Direction.Up);
        return blockedDirections;
    }
}
