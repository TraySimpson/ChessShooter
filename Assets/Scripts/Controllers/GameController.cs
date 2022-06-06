using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MapController))]
public class GameController : MonoBehaviour
{
    [SerializeField] private int _unitsPerTeam = 5;
    [SerializeField] private int _actionPointsPerTurn = 6;
    [SerializeField] private GameObject _unit1Prefab;
    [SerializeField] private GameObject _unit2Prefab;
    public GameState State {get; private set;}
    private int _currentActionPoints;
    public int CurrentActionPoints { 
        get => _currentActionPoints;
        set {
            _currentActionPoints = value;
            if (_currentActionPoints < 1)
                EndTurn();
        }
    }

    [SerializeField] private int unit1Count;
    [SerializeField] private int unit2Count;

    private MapController _map;

    private void Awake() {
        UnitDamage.OnUnitDied += UnitDied;
    }

    private void OnDisable() {
        UnitDamage.OnUnitDied -= UnitDied;
    }

    public void EndTurn() {
        State = State == GameState.Team1Turn ? GameState.Team2Turn :  GameState.Team1Turn;
        CurrentActionPoints = _actionPointsPerTurn;

        if (State == GameState.Team2Turn) {
            //TODO
            print("Skipped NPC Turn");
            EndTurn();
        }
    }

    private void UnitDied(GameObject unit) {
        if (unit.GetComponent<Unit>().Team == Team.Team1) {
            unit1Count--;
        } else {
            unit2Count--;
        }
        CheckForWin();
    }

    private void CheckForWin() {
        if (unit1Count == 0)
            print("Team 2 wins!");
        if (unit2Count == 0)
            print("Team 1 wins!");
    }

    void Start()
    {
        _map = GetComponent<MapController>();
        _map.SetupMap();
        int midWidth = _map.unitWidth / 2;
        unit1Count = 0;
        unit2Count = 0;
        SpawnUnits(midWidth, 2);
        SpawnUnits(midWidth, _map.unitHeight - 3, true);
        State = GameState.Team2Turn;
        EndTurn();
    }

    private void SpawnUnits(int x, int y, bool isTeam2 = false)
    {
        for (int i = 0; i < _unitsPerTeam; i++)
        {
            if (_map.CanPlace(x, y))
            {
                SpawnUnit(x, y, isTeam2);
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

    private void SpawnUnit(int x, int y, bool isTeam2)
    {
        GameObject unit = Instantiate((isTeam2 ? _unit2Prefab : _unit1Prefab), new Vector3(x, .8f, y), (isTeam2 ? Quaternion.Euler(0, 180, 0) : Quaternion.identity));
        unit.GetComponent<Unit>().Team = isTeam2 ? Team.Team2 : Team.Team1;
        WorldObject unitObject = new WorldObject(unit, WorldObjectType.Unit, 0);
        _map.PlaceObject(x, y, unitObject);
        if (isTeam2) {
            unit2Count++;
        } else {
            unit1Count++;
        }
    }
}

public enum GameState {
    Team1Turn,
    Team2Turn,
    Team1Win,
    Team2Win
}
