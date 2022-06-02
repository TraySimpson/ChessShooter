using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MapController))]
public class GameController : MonoBehaviour
{
    [SerializeField] private int _unitsPerTeam = 5;
    [SerializeField] private GameObject _unitPrefab;
    [SerializeField] private Material _enemyMat;
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
            if (map.CanPlace(x, y))
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
        GameObject unit = Instantiate(_unitPrefab, new Vector3(x, .8f, y), Quaternion.identity);
        WorldObject unitObject = new WorldObject(unit, WorldObjectType.Unit);
        map.PlaceObject(x, y, unitObject);
        if (isEnemy)
            unit.GetComponent<Renderer>().material = _enemyMat;
    }
}

public enum GameState {
    Team1Turn,
    Team2Turn,
    Team1Win,
    Team2Win
}
