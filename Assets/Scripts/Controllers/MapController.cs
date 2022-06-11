using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [SerializeField] private int _unitWidth;
    public int unitWidth {get {return _unitWidth;} set {}}
    [SerializeField] private int _unitHeight;
    public int unitHeight { get { return _unitHeight; } set { } }

    private bool hasInitialized = false;

    [SerializeField] private int negativeLevels = 0;
    [SerializeField] private int positiveLevels = 1;
    [SerializeField] private int activeLevel = 0;
    [SerializeField] private GameObject _plane;
    [SerializeField] private WorldObject[,,] WorldMap {get; set;}
    [SerializeField] private WorldEdgeObject[,,] WorldEdgesMap { get; set; }

    private void Awake() {
        UnitDamage.OnUnitDied += RemoveUnit;
    }

    private void OnDisable() {
        UnitDamage.OnUnitDied -= RemoveUnit;
    }

    public void SetupMap()
    {
        ScalePlane();
        SetupGrids();
        hasInitialized = true;
    }

    public void RemoveUnit(GameObject unit) {
        Vector2Int currentCoords = unit.Get2DCoordinates();
        if (IsValidCoords(currentCoords))
            WorldMap[currentCoords.x, currentCoords.y, activeLevel] = null;
    }

    public bool CanPlace(int x, int y)
    {   
        return IsValidCoords(x,y) && WorldMap[x, y, activeLevel] is null;
    }

    public bool IsValidCoords(Vector2Int coords) { return IsValidCoords(coords.x, coords.y);}

    public bool IsValidCoords(int x, int y) {
        return x >= 0 &&
            x < unitWidth &&
            y >= 0 &&
            y < unitHeight;
    }

    public WorldObject GetObjectAtCoords(Vector2Int coords) 
    {
        if (!IsValidCoords(coords)) return null;
        return WorldMap[coords.x, coords.y, activeLevel];
    }

    public void PlaceObject(int x, int y, WorldObject placeObject) 
    {
        WorldMap[x, y, activeLevel] = placeObject;
    }

    public void PlaceObject(int x, int y, WorldEdgeObject placeObject)
    {
        WorldEdgesMap[x, y, activeLevel] = placeObject;
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
        WorldEdgesMap = new WorldEdgeObject[unitWidth, (unitHeight * 2) - 1, levelCount];
    }

    // private void FillWorldGrid<T>(ref T[,] grid, T fillObject) {
    //     grid = new T[unitWidth, unitHeight];
    //     for (int i = 0; i < unitWidth; i++) {
    //         for (int j = 0; j < unitHeight; j++) {
    //             grid[i,j] = fillObject;
    //         }
    //     }
    // }

    public WorldObject GetObjectAt(int x, int y) {
        return WorldMap[x, y, activeLevel];
    }

    public bool PlaceRoomSeed(int x, int y, int roomId)
    {
        if (!(WorldMap[x, y, activeLevel] is null)) return false;
        WorldMap[x, y, activeLevel] = new WorldObject(roomId);
        return true;
    }

    public void DrawBoundingBox(GridRectangle rectangle) {
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

    public void FillRectangle(GridRectangle rectangle, int roomId) {
        WorldObject roomObject = new WorldObject(roomId);
        for (int x = rectangle.GetMinX(); x <= rectangle.GetMaxX(); x++) {
            for (int y = rectangle.GetMinY(); y <= rectangle.GetMaxY(); y++) {
                WorldMap[x, y, activeLevel] = roomObject;
            }
        }
    }

    private void OnDrawGizmos() {
        if (!hasInitialized) return;
        for (int i = 0; i < unitWidth; i++) {
            for (int j = 0; j < unitHeight; j++) {
                WorldObject worldObject = WorldMap[i, j, activeLevel];
                if (worldObject is null) continue;
                Gizmos.color = GetColorFromID(worldObject.RoomId);
                Gizmos.DrawCube(new Vector3(i, activeLevel, j), Vector3.one);
            }
        }
    }

    private Color GetColorFromID(int id) {
        Random.InitState(id);
        return Random.ColorHSV();
    }
}

public class WorldEdgeObject
{
    public GameObject GameObject { get; set; }
    public WorldEdgeObjectType Type { get; set;}

    public WorldEdgeObject(GameObject gameObject, WorldEdgeObjectType type) 
    {
        this.GameObject = gameObject;
        this.Type = type;
    }
}

public class WorldObject {
    public GameObject GameObject {get; set;}
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

public enum WorldEdgeObjectType {
    HalfStructure,
    FullStructure,
    Door
}

public enum WorldObjectType {
    Unit,
    HalfStructure,
    FullStructure
}
