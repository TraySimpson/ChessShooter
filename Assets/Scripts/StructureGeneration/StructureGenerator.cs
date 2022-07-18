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

    private GridRectangle lotRectangle;
    private List<Vector2Int> externalDoors;
    private int minXYvalue;
    private int maxXValue;
    private int maxYValue;
    private int width;
    private int height;
    private int numberOfMainRooms;
    private int minRoomSize;
    private int maxRoomSize;

    private MapController _map;

    private void Start() {
        SetupMap();
        SetupStructureAttributes();
        GenerateStruture();
    }

    public void SetupMap() {
        _map = GetComponent<MapController>();
        _map.SetupMap();
        width = _map.unitWidth;
        height = _map.unitHeight;
    }

    public void SetupStructureAttributes () {
        Random.InitState(seed);
        width -= paddingOffset * 2;
        height -= paddingOffset * 2;

        minXYvalue = paddingOffset;
        maxXValue = width + paddingOffset - 1;
        maxYValue = height + paddingOffset - 1;
        int totalLotSize = width * height;

        float filledSpacePercentage = filledSpacePercentageOverride ?? Random.Range(.4f, 1);
        int lotSize = Mathf.RoundToInt(totalLotSize * filledSpacePercentage);
        if (lotSize < minRoomNumber * minAverageRoomSize)
            lotSize = minRoomNumber * minAverageRoomSize;

        // Determines size of main rooms based on min/max constraints
        float spaciousFactor = Random.Range(0f, 1f);
        print($"Space factor {spaciousFactor}");
        int averageRoomSize = Mathf.RoundToInt(Mathf.Lerp(minAverageRoomSize, maxAverageRoomSize, spaciousFactor));
        while (averageRoomSize * minRoomNumber > lotSize)
        {
            averageRoomSize--;
        }

        numberOfMainRooms = Mathf.FloorToInt((lotSize * filledSpacePercentage) / averageRoomSize);

        print($"Building house with total lot: {totalLotSize}     Used lot: {lotSize}      Room count: {numberOfMainRooms}      Average room size: {averageRoomSize}");

        minRoomSize = Mathf.FloorToInt(averageRoomSize * (1 - roomSizeVariationPercent));
        maxRoomSize = Mathf.CeilToInt(averageRoomSize * (1 + roomSizeVariationPercent)) + 1;
        print($"Min room size: {minRoomSize}    Max room size: {maxRoomSize - 1}");

        lotRectangle = new GridRectangle(minXYvalue, minXYvalue, width, height);
        _map.SetBounds(lotRectangle);
    }

    public void GenerateStruture() {
        externalDoors = new List<Vector2Int>();
        FactorStore factorStore = new FactorStore();
        for (int i = 0; i < numberOfMainRooms; i++) {
            Vector2Int doorPosition = externalDoors.Count != 0 ?
                externalDoors.GetAndRemove(Random.Range(0, externalDoors.Count)) :
                lotRectangle.GetRandomEdgePosition();

            int roomSize = Random.Range(minRoomSize, maxRoomSize);
            List<int> factors = factorStore.GetFactors(roomSize);
            //Prime value encountered
            if (factors.Count == 0) {
                print("skipping prime for now");
                continue;
            }            
            GridRectangle roomRectangle = GetRandomGridRectangle(doorPosition, roomSize, factors);
            _map.FillRectangle(roomRectangle, i);
            AddRoomDoors(roomRectangle, i);
        }
    }

    public GridRectangle GetRandomGridRectangle(Vector2Int doorPosition, int roomSize, List<int> factors) {
        PointSpacing space = _map.GetSpaceAtCoords(doorPosition, lotRectangle);
        int roomWidth = factors[Random.Range(0, factors.Count)];
        int roomHeight = roomSize / roomWidth;
        if (space.UpSpace + space.DownSpace + 1 < roomHeight)
            roomHeight = space.UpSpace + space.DownSpace + 1;
        if (space.LeftSpace + space.RightSpace + 1 < roomWidth)
            roomWidth = space.LeftSpace + space.RightSpace + 1;
        Vector2Int origin = GetRandomOrigin(doorPosition, roomWidth, roomHeight, space);
        return new GridRectangle(origin, roomWidth, roomHeight);
    }

    public Vector2Int GetRandomOrigin(Vector2Int start, int width, int height, PointSpacing space) {
        List<Direction> blockedDirections = space.GetBlockedDirections();
        switch(blockedDirections.Count) {
            case 1:
                switch(blockedDirections[0]) {
                    case Direction.Left:
                        return new Vector2Int(start.x, Random.Range(start.y, start.y - height));
                    case Direction.Down:
                        return new Vector2Int(Random.Range(start.x - width + 1, start.x), start.y);
                    case Direction.Right:
                        return new Vector2Int((start.x - width + 1), Random.Range(start.y, start.y - height));
                    default:
                        return new Vector2Int(Random.Range(start.x - width + 1, start.x), (start.y - height + 1));
                }
            // case 2:
            //     break;
            // case 3:
            //     break;
            default:
                return start;
        }
    }

    private void AddRoomDoors(GridRectangle roomRectangle, int roomId) {
        List<Vector2Int> possibleSpaces = _map.GetEmptyAdjacentPositions(roomRectangle);
        for (int doorIndex = 0; doorIndex < GetRandomNumberOfDoors(); doorIndex++) {
            Vector2Int doorSpace;
            do {
                doorSpace = possibleSpaces[Random.Range(0, possibleSpaces.Count)];
            } while (externalDoors.Contains(doorSpace));
            externalDoors.Add(doorSpace);
            _map.AddDoor(doorSpace, roomId);
        }
    }

    public int GetRandomNumberOfDoors() {
        float determiner = Random.Range(0f, 1f);
        if (determiner < oneDoorRoomPercentStep)
            return 1;
        if (determiner < twoDoorRoomPercentStep)
            return 2;
        if (determiner < threeDoorRoomPercentStep)
            return 3;
        return 4;
    }
}