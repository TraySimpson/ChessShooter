using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [SerializeField] private int _unitWidth;
    [SerializeField] private int _unitHeight;
    [SerializeField] private int negativeLevels = 0;
    [SerializeField] private int positiveLevels = 1;
    [SerializeField] private int activeLevel = 0;
    [SerializeField] private GameObject _plane;
    [SerializeField] private WorldObject[,,] WorldMap {get; set;}
    [SerializeField] private WorldEdgeObject[,,] WorldEdgesMap { get; set; }

    public int unitWidth => _unitWidth;
    public int unitHeight => _unitHeight;

    private GridRectangle bounds;
    private bool hasInitialized = false;

    private void Awake() {
        UnitDamage.OnUnitDied += RemoveObject;
    }

    private void OnDisable() {
        UnitDamage.OnUnitDied -= RemoveObject;
    }

    public void SetupMap()
    {
        ScalePlane();
        SetupGrids();
        hasInitialized = true;
    }

    public void RemoveObject(GameObject removeObject) {
        Vector2Int currentCoords = removeObject.Get2DCoordinates();
        if (IsValidCoords(currentCoords))
            WorldMap[currentCoords.x, currentCoords.y, activeLevel] = null;
    }

    public bool CanPlace(int x, int y, bool checkBounds = false) { 
        return IsValidCoords(x,y) && 
            WorldMap[x, y, activeLevel] is null && 
            (!checkBounds || InBounds(x,y)); 
    }
    public bool CanPlace(Vector2Int coords) {return CanPlace(coords.x, coords.y);}

    public bool InBounds(int x, int y) {
        return x >= bounds.GetMinX() &&
            x < bounds.GetMaxX() &&
            y >= bounds.GetMinY() &&
            y < bounds.GetMaxY();
    }

    public bool IsValidCoords(int x, int y) {
        return x >= 0 &&
            x < unitWidth &&
            y >= 0 &&
            y < unitHeight;
    }
    public bool IsValidCoords(Vector2Int coords) { return IsValidCoords(coords.x, coords.y); }

    public WorldObject GetObjectAtCoords(Vector2Int coords) {
        if (!IsValidCoords(coords)) return null;
        return WorldMap[coords.x, coords.y, activeLevel];
    }

    public void PlaceObject(int x, int y, WorldObject placeObject) {
        WorldMap[x, y, activeLevel] = placeObject;
    }
    public void PlaceObject(int x, int y, WorldEdgeObject placeObject){
        WorldEdgesMap[x, y, activeLevel] = placeObject;
    }

    public List<Vector2Int> GetEmptyAdjacentPositions(GridRectangle room) {
        List<Vector2Int> positions = new List<Vector2Int>();
        int bottomY = room.GetMinY() - 1;
        int topY = room.GetMaxY() + 1;
        for (int x = room.GetMinX() - 1; x <= room.GetMaxX() + 1; x++) {
            AddCoordsIfInBounds(ref positions, x, bottomY);
            AddCoordsIfInBounds(ref positions, x, topY);
        }
        int bottomX = room.GetMinX() - 1;
        int topX = room.GetMaxX() + 1;
        for (int y = room.GetMinY(); y <= room.GetMaxY(); y++) {
            AddCoordsIfInBounds(ref positions, bottomX, y);
            AddCoordsIfInBounds(ref positions, topX, y);
        }
        return positions;
    }

    public void AddCoordsIfInBounds(ref List<Vector2Int> list, int x, int y) {
        if (CanPlace(x, y, true))
            list.Add(new Vector2Int(x, y));
    }

    public void MoveObject(GameObject gameObject, Vector2Int newCoordinates) {
        //Update 3D Array
        Vector2Int currentCoords = gameObject.Get2DCoordinates();
        WorldObject worldObject = WorldMap[currentCoords.x, currentCoords.y, activeLevel];
        WorldMap[currentCoords.x, currentCoords.y, activeLevel] = null;
        WorldMap[newCoordinates.x, newCoordinates.y, activeLevel] = worldObject;   
    }

    private void ScalePlane() {
        if (_plane is null)
            _plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        _plane.transform.localScale = new Vector3(unitWidth / 5, 1, unitHeight / 5);
        _plane.transform.position = new Vector3((unitWidth / 2 - .5f), 0, (unitHeight / 2 - .5f));
        _plane.GetComponent<Renderer>().material.mainTextureScale = new Vector2(unitWidth, unitHeight);
    }

    private void SetupGrids() {
        int levelCount = negativeLevels + positiveLevels;
        WorldMap = new WorldObject[unitWidth, unitHeight, levelCount];
        WorldEdgesMap = new WorldEdgeObject[unitWidth + 1, (unitHeight * 2) + 1, levelCount];
    }

    public WorldObject GetObjectAt(int x, int y) {
        return WorldMap[x, y, activeLevel];
    }
    public WorldObject GetObjectAt(Vector2Int coords) { return GetObjectAt(coords.x, coords.y);}

    public void AddDoor(Vector2Int worldCoords, int roomId) {
        Direction direction = GetDoorRoomDirection(worldCoords, roomId);
        Vector2Int doorCoords = GetEdgeAtCoords(worldCoords, direction);
        WorldEdgesMap[doorCoords.x, doorCoords.y, activeLevel] = new WorldEdgeObject(null, WorldEdgeObjectType.Door);
    }

    public Direction GetDoorRoomDirection(Vector2Int worldCoords, int roomId) {
        if (RoomIsAtCoords(worldCoords - Vector2Int.left, roomId))
            return Direction.Left;
        if (RoomIsAtCoords(worldCoords - Vector2Int.right, roomId))
            return Direction.Right;
        if (RoomIsAtCoords(worldCoords - Vector2Int.up, roomId))
            return Direction.Up;
        return Direction.Down;
    }

    public Vector2Int GetEdgeAtCoords(Vector2Int coords, Direction direction) {
        Vector2Int edgePos;
        switch (direction) {
            case Direction.Left:
                edgePos = new Vector2Int(coords.x, (2 * coords.y) + 1);
                break;
            case Direction.Right:
                edgePos = new Vector2Int(coords.x + 1, (2 * coords.y) + 1);
                break;
            case Direction.Up:
                edgePos = new Vector2Int(coords.x, (2 * coords.y) + 2);
                break;
            default:
                edgePos = new Vector2Int(coords.x, 2 * coords.y);
                break;
        }
        return edgePos;
    }

    public bool RoomIsAtCoords(Vector2Int worldCoords, int roomId) {
        return IsValidCoords(worldCoords) && !(GetObjectAt(worldCoords) is null) && GetObjectAt(worldCoords).RoomId == roomId;
    }

    public void SetBounds(GridRectangle bounds) {
        this.bounds = bounds;
        DrawBoundingBox(bounds);
    }

    private void DrawBoundingBox(GridRectangle rectangle) {
        WorldObject roomObject = new WorldObject(1000);
        int minY = rectangle.GetMinY() - 1;
        int maxY = rectangle.GetMaxY() + 1;
        for (int x = 0; x < _unitWidth; x++)
        {
            WorldMap[x, minY, activeLevel] = roomObject;
            WorldMap[x, maxY, activeLevel] = roomObject;
        }
        int minX = rectangle.GetMinX() - 1;
        int maxX = rectangle.GetMaxX() + 1;
        for (int y = 0; y < _unitHeight; y++)
        {
            WorldMap[minX, y, activeLevel] = roomObject;
            WorldMap[maxX, y, activeLevel] = roomObject;
        }
    }

    public PointSpacing GetSpaceAtCoords(Vector2Int coords, GridRectangle bounds = null) {
        int minX = bounds is null ? 0 : bounds.GetMinX();
        int minY = bounds is null ? 0 : bounds.GetMinY();
        int maxX = bounds is null ? _unitWidth : bounds.GetMaxX();
        int maxY = bounds is null ? _unitHeight : bounds.GetMaxY();
        int up = coords.y;
        int right = coords.x;
        int down = coords.y;
        int left = coords.x;
        while (left >= minX && WorldMap[left, coords.y, activeLevel] is null)
            left--;
        while (right <= maxX && WorldMap[right, coords.y, activeLevel] is null)
            right++;
        while (down >= minY && WorldMap[coords.x, down, activeLevel] is null)
            down--;
        while (up <= maxY && WorldMap[coords.x, up, activeLevel] is null)
            up++;
        return new PointSpacing(coords, up - coords.y - 1, right - coords.x - 1, coords.y - down - 1, coords.x - left - 1);
    }

    public void FillRectangle(GridRectangle rectangle, int roomId) {
        WorldObject roomObject = new WorldObject(roomId);
        for (int x = rectangle.GetMinX(); x <= rectangle.GetMaxX(); x++) {
            for (int y = rectangle.GetMinY(); y <= rectangle.GetMaxY(); y++) {
                WorldMap[x, y, activeLevel] = roomObject;
            }
        }
    }

    // private void OnDrawGizmos() {
    //     if (!hasInitialized) return;
    //     for (int i = 0; i < unitWidth; i++) {
    //         for (int j = 0; j < unitHeight; j++) {
    //             WorldObject worldObject = WorldMap[i, j, activeLevel];
    //             if (worldObject is null) continue;
    //             Gizmos.color = GetColorFromID(worldObject.RoomId);
    //             Gizmos.DrawCube(new Vector3(i, activeLevel, j), Vector3.one);
    //         }
    //     }
    //     int edgesWidth = unitWidth + 1;
    //     int edgesHeight = (unitHeight * 2) + 1;
    //     for (int i = 0; i < edgesWidth; i++) {
    //         for (int j = 0; j < edgesHeight; j++) {
    //             WorldEdgeObject worldObject = WorldEdgesMap[i, j, activeLevel];
    //             if (worldObject is null) continue;
    //             Gizmos.color = Color.black;
    //             Gizmos.DrawSphere(GetEdgePosition(i, j), 1f);
    //         }
    //     }
    // }

    private Vector3 GetEdgePosition(int x, int y) {
        return new Vector3(
            (y % 2 == 0 ? x + .5f : x),
            activeLevel,
            y / 2);
    }

    private Color GetColorFromID(int id) {
        Random.InitState(id);
        return Random.ColorHSV();
    }
}