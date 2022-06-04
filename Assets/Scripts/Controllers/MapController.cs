using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [SerializeField] private int _unitWidth;
    public int unitWidth {get {return _unitWidth;} set {}}
    [SerializeField] private int _unitHeight;
    public int unitHeight { get { return _unitHeight; } set { } }

    [SerializeField] private int negativeLevels = 0;
    [SerializeField] private int positiveLevels = 1;
    [SerializeField] private int activeLevel = 0;
    [SerializeField] private GameObject _plane;
    [SerializeField] private WorldObject[,,] _worldMap;
    [SerializeField] private WorldEdgeObject[,,] _worldEdgesMap;

    public void SetupMap()
    {
        ScalePlane();
        SetupGrids();
    }

    public bool CanPlace(int x, int y)
    {   
        return IsValidCoords(x,y) && _worldMap[x, y, activeLevel] is null;
    }

    public bool IsValidCoords(int x, int y) {
        return x >= 0 &&
            x < unitWidth &&
            y >= 0 &&
            y < unitHeight;
    }

    public WorldObject GetObjectAtCoords(Vector2Int coords) 
    {
        if (!IsValidCoords(coords.x, coords.y)) return null;
        return _worldMap[coords.x, coords.y, activeLevel];
    }

    public void PlaceObject(int x, int y, WorldObject placeObject) 
    {
        _worldMap[x, y, activeLevel] = placeObject;
    }

    public void PlaceObject(int x, int y, WorldEdgeObject placeObject)
    {
        _worldEdgesMap[x, y, activeLevel] = placeObject;
    }

    public void MoveObject(GameObject gameObject, Vector2Int newCoordinates) {
        //Update 3D Array
        Vector2Int currentCoords = gameObject.Get2DCoordinates();
        WorldObject worldObject = _worldMap[currentCoords.x, currentCoords.y, activeLevel];
        _worldMap[currentCoords.x, currentCoords.y, activeLevel] = null;
        _worldMap[newCoordinates.x, newCoordinates.y, activeLevel] = worldObject;   
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
        _worldMap = new WorldObject[unitWidth, unitHeight, levelCount];
        _worldEdgesMap = new WorldEdgeObject[unitWidth, (unitHeight * 2) - 1, levelCount];
    }

    // private void FillWorldGrid<T>(ref T[,] grid, T fillObject) {
    //     grid = new T[unitWidth, unitHeight];
    //     for (int i = 0; i < unitWidth; i++) {
    //         for (int j = 0; j < unitHeight; j++) {
    //             grid[i,j] = fillObject;
    //         }
    //     }
    // }
}

public class WorldEdgeObject
{
    public GameObject gameObject { get; set; }
    public WorldEdgeObjectType type { get; set;}

    public WorldEdgeObject(GameObject gameObject, WorldEdgeObjectType type) 
    {
        this.gameObject = gameObject;
        this.type = type;
    }
}

public class WorldObject {
    public GameObject gameObject {get; set;}
    public WorldObjectType type { get; set; }

    public WorldObject(GameObject gameObject, WorldObjectType type)
    {
        this.gameObject = gameObject;
        this.type = type;
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
