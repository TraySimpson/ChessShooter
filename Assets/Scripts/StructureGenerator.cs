using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MapController))]
public class StructureGenerator : MonoBehaviour
{
    [SerializeField] private int paddingOffset = 3;
    [SerializeField] private int seed;
    [SerializeField] private int minRoomNumber = 7;
    [SerializeField] private int minAverageRoomSize = 15;
    [SerializeField] private int maxAverageRoomSize = 25;
    [SerializeField] private float roomSizeVariationPercent = .2f;
    [SerializeField] private float? filledSpacePercentageOverride = .75f;

    [SerializeField] private const float oneDoorRoomPercentStep = .2f;
    [SerializeField] private const float twoDoorRoomPercentStep = .4f;
    [SerializeField] private const float threeDoorRoomPercentStep = .9f;

    private int minXYvalue;
    private int maxXValue;
    private int maxYValue;

    private MapController _map;

    private void Start() {
        _map = GetComponent<MapController>();
        _map.SetupMap();
        GenerateStrutureMap(_map.unitWidth, _map.unitHeight);
    }

    public void GenerateStrutureMap(int width, int height) {
        Random.InitState(seed);
        width -= paddingOffset * 2;
        height -= paddingOffset * 2;

        minXYvalue = paddingOffset;
        maxXValue = width + paddingOffset - 1;
        maxYValue = height + paddingOffset - 1;
        int totalLotSize = width * height;
        
        // Percentage of lot space used
        float filledSpacePercentage = filledSpacePercentageOverride ?? Random.Range(.4f, 1);
        int lotSize = Mathf.RoundToInt(totalLotSize * filledSpacePercentage);
        if (lotSize < minRoomNumber * minAverageRoomSize) 
            lotSize = minRoomNumber * minAverageRoomSize;

        // Determines size of main rooms based on min/max constraints
        float spaciousFactor = Random.Range(0f, 1f);
        print($"Space factor {spaciousFactor}");
        int averageRoomSize = Mathf.RoundToInt(Mathf.Lerp(minAverageRoomSize, maxAverageRoomSize, spaciousFactor));
        while (averageRoomSize * minRoomNumber > lotSize) {
            averageRoomSize--;
        }

        // Determine number of main rooms
        int numberOfMainRooms = Mathf.FloorToInt((lotSize * filledSpacePercentage)/averageRoomSize);

        print($"Building house with total lot: {totalLotSize}     Used lot: {lotSize}      Room count: {numberOfMainRooms}      Average room size: {averageRoomSize}");

        int minRoomSize = Mathf.FloorToInt(averageRoomSize * (1-roomSizeVariationPercent));
        int maxRoomSize = Mathf.CeilToInt(averageRoomSize * (1 + roomSizeVariationPercent)) + 1;
        print($"Min room size: {minRoomSize}    Max room size: {maxRoomSize-1}");

        print($"Bindbox with minXY: {minXYvalue}    width: {width}    height: {height}");
        GridRectangle lotRectangle = new GridRectangle(minXYvalue, minXYvalue, width, height);
        _map.DrawBoundingBox(lotRectangle);
        List<Vector2Int> externalDoors = new List<Vector2Int>();

        int roomSpaceBudget = averageRoomSize * numberOfMainRooms;
        FactorStore factorStore = new FactorStore();
        for (int i = 0; i < numberOfMainRooms; i++) {
            Vector2Int startPosition = externalDoors.Count != 0 ?
                externalDoors.GetAndRemove(Random.Range(0, externalDoors.Count)) :
                lotRectangle.GetRandomEdgePosition();

            int roomSize = Random.Range(minRoomSize, maxRoomSize);
            List<int> factors = factorStore.GetFactors(roomSize);
            //Prime value encountered
            if (factors.Count == 0) {
                print("skipping prime for now");
                continue;
            }
            int roomWidth = factors[Random.Range(0, factors.Count)];
            GridRectangle roomRectangle = new GridRectangle(startPosition, roomWidth, roomSize/roomWidth);
            _map.FillRectangle(roomRectangle, i);
            for (int doorIndex = 0; doorIndex < GetRandomNumberOfDoors(); doorIndex ++) {
                externalDoors.Add(roomRectangle.GetRandomEdgePosition(1));
            }
        }
    }

    public int GetRandomNumberOfDoors() {
        float determiner = Random.Range(0f, 1f);
        if (determiner < oneDoorRoomPercentStep)
            return 1;
        if (determiner < twoDoorRoomPercentStep)
            return 1;
        if (determiner < threeDoorRoomPercentStep)
            return 1;
        return 4;
    }
}

public class GridRectangle {
    public int Height { get; set; }
    public int Width { get; set; }
    // Bottom Left coordinate (inclusive)
    public Vector2Int Origin { get; set; }

    public GridRectangle(Vector2Int origin, int width, int height) {
        this.Origin = origin;
        this.Height = height;
        this.Width = width;
    }
    public GridRectangle(int originX, int originY, int width, int height) {
        this.Origin = new Vector2Int(originX, originY);
        this.Height = height;
        this.Width = width;
    }

    public int GetMinX() => Origin.x;
    public int GetMinY() => Origin.y;
    public int GetMaxX() => Origin.x + Width - 1;
    public int GetMaxY() => Origin.y + Height - 1;

    public Vector2Int GetRandomEdgePosition(int offset = 0) {
        int direction = Random.Range(0, 4);
        int xValue = 0;
        int yValue = 0;
        switch (direction) {
            case 0:
                xValue = Random.Range(GetMinX(), GetMaxX());
                yValue = GetMaxY() + offset;
                break;
            case 1:
                xValue = GetMaxX() + offset;
                yValue = Random.Range(GetMinY(), GetMaxY());
                break;
            case 2:
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
}






//OLD ROOM SEED GENERATION
//     for(int i = 0; i < numberOfMainRooms; i++) {
//     int x;
//     int y;
//     do {
//         x = Random.Range(minXYvalue, maxXValue);
//         y = Random.Range(minXYvalue, maxXValue);
//     }
//     while(!(_map.PlaceRoomSeed(x, y, i)));
// }



// List<RoomType> rooms = GetRoomTypes();
// List<RoomType> requiredRooms = rooms.FindAll(r => r.IsRequired);

//OLD ROOM LIST
// public List<RoomType> GetRoomTypes() {
//     List<RoomType> rooms = new List<RoomType>();
//     rooms.Add(new RoomType("kitchen", true, 9, 36));
//     rooms.Add(new RoomType("bathroom", true, 4, 15));
//     rooms.Add(new RoomType("bedroom", true, 6, 36));

//     rooms.Add(new RoomType("livingroom", false, 9, 36));
//     rooms.Add(new RoomType("closet", false, 1, 6));
//     rooms.Add(new RoomType("diningroom", false, 9, 36));
//     return rooms;
// }

// public class RoomType {
//     public string Name { get; set; }
//     public bool IsRequired { get; set; }
//     public int MinSquareSize { get; set; }
//     public int MaxSquareSize { get; set; }

//     public RoomType(string name, bool required, int min, int max) {
//         Name = name;
//         IsRequired = required;
//         MinSquareSize = min;
//         MaxSquareSize = max;
//     }
// }

// public enum PorchType 
// {
//     None = 0,
//     Front = 1,
//     Back = 2,
//     Both = 3,
//     Wrap = 4
// }

// public enum Garage
// {
//     None = 0,
//     Single = 1,
//     Double = 2
// }
