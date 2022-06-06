using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MapController))]
public class StructureGenerator : MonoBehaviour
{
    [SerializeField] private int paddingOffset = 3;
    [SerializeField] private int seed;
    [SerializeField] private int minRoomNumber = 3;
    [SerializeField] private int minAverageRoomSize = 7;
    [SerializeField] private int maxAverageRoomSize = 40;
    [SerializeField] private float roomSizeVariationPercent = .2f;

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

        int minXYvalue = paddingOffset - 1;
        int maxXValue = width + paddingOffset - 1;
        int maxYValue = height + paddingOffset - 1;
        int totalLotSize = width * height;
        
        // Percentage of lot space used
        float filledSpacePercentage = Random.Range(.2f, 1);
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
        int numberOfMainRooms = Mathf.FloorToInt(lotSize/averageRoomSize);

        print($"Building house with \nTotal lot: {totalLotSize} \nUsed lot: {lotSize} \nRoom count: {numberOfMainRooms} \nAverage room size: {averageRoomSize}");

        int remainingRoomSpace = lotSize;
        int minRoomSize = Mathf.FloorToInt(averageRoomSize * (1-roomSizeVariationPercent));
        int maxRoomSize = Mathf.CeilToInt(averageRoomSize * (1 + roomSizeVariationPercent));

        Stack<Vector2Int> roomStack = new Stack<Vector2Int>();
        Vector2Int startCoords = new Vector2Int(Random.Range(minXYvalue, maxXValue), minXYvalue);
        roomStack.Push(startCoords);

        for(int i = 0; i < numberOfMainRooms; i++) {
            int roomSize = Random.Range(minRoomSize, maxRoomSize);
            int edgeSize = Mathf.RoundToInt(Mathf.Sqrt(roomSize));
            int edgeSplit = Random.Range(0, edgeSize);
            Vector2Int direction = GetRandomDirection();
            Vector2Int currentCoords = startCoords;
            int usedSquareUnits = 0;
            for(int j = 0; j < edgeSize; j++) {
                if (!_map.PlaceRoomSeed(currentCoords.x, currentCoords.y, i)) {
                    
                }
            }
        }
    }

    private Vector2Int GetRandomDirection() {
        switch(Random.Range(0, 4)) {
            case 0:
                return Vector2Int.down;
            case 1:
                return Vector2Int.up;
            case 2: 
                return Vector2Int.left;
            default:
                return Vector2Int.right;
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
}

public class RoomType {
    public string Name { get; set; }
    public bool IsRequired { get; set; }
    public int MinSquareSize { get; set; }
    public int MaxSquareSize { get; set; }

    public RoomType(string name, bool required, int min, int max) {
        Name = name;
        IsRequired = required;
        MinSquareSize = min;
        MaxSquareSize = max;
    }
}

public enum PorchType 
{
    None = 0,
    Front = 1,
    Back = 2,
    Both = 3,
    Wrap = 4
}

public enum Garage
{
    None = 0,
    Single = 1,
    Double = 2
}
