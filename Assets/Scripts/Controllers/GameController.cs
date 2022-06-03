using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MapController))]
public class GameController : MonoBehaviour
{
    [SerializeField] private int _unitsPerTeam = 5;
    [SerializeField] private GameObject _unit1Prefab;
    [SerializeField] private GameObject _unit2Prefab;
    public GameState state;

    private MapController map;

    void Start()
    {
        map = GetComponent<MapController>();
        map.SetupMap();
        int midWidth = map.unitWidth / 2;
        SpawnUnits(midWidth, 2);
        SpawnUnits(midWidth, map.unitHeight - 3, true);
    }

    private void SpawnUnits(int x, int y, bool isEnemy = false)
    {
        for (int i = 0; i < _unitsPerTeam; i++)
        {
            if (map.IsValidCoords(x, y))
            {
                SpawnUnit(x, y, isEnemy);
            }
            else
            {
                //TODO fix this shit lol
                x += Random.Range(-1, 2);
                y += Random.Range(-1, 2);
                i--;
            }
        }
    }

    private void SpawnUnit(int x, int y, bool isEnemy)
    {
        GameObject unit = Instantiate((isEnemy ? _unit2Prefab : _unit1Prefab), new Vector3(x, .8f, y), (isEnemy ? Quaternion.Euler(0, 180, 0) : Quaternion.identity));
        WorldObject unitObject = new WorldObject(unit, WorldObjectType.Unit);
        map.PlaceObject(x, y, unitObject);
    }
}

public enum GameState {
    Team1Turn,
    Team2Turn,
    Team1Win,
    Team2Win
}
