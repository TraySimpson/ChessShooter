using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MapController))]
public class GameController : MonoBehaviour
{

    #region Events
    public static event Action<GameState, Vector2Int> OnTurnChanged = delegate { };
    public static event Action<GameState> OnGameEnded = delegate { };
    #endregion

    [SerializeField] private int _unitsPerTeam = 5;
    [SerializeField] private int _actionPointsPerTurn = 6;
    [SerializeField] private GameObject _unit1Prefab;
    [SerializeField] private GameObject _unit2Prefab;

    public List<GameObject> Team1;
    public List<GameObject> Team2;
    public List<UnitFOV> Team1FOVs;
    public List<UnitFOV> Team2FOVs;

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

    private MapController _map;
    private WeaponFactory _weaponFactory;

    private void Awake() {
        UnitDamage.OnUnitDied += UnitDied;
    }

    private void OnDisable() {
        UnitDamage.OnUnitDied -= UnitDied;
    }

    public void EndTurn() {
        CurrentActionPoints = _actionPointsPerTurn;
        State = State == GameState.Team1Turn ? GameState.Team2Turn : GameState.Team1Turn;
        ToggleFOVs();
        OnTurnChanged.Invoke(State, GetRandomUnitPos());
    }

    public void ToggleFOVs() {
        foreach (var fov in Team1FOVs)
        {
            fov.Toggle(State == GameState.Team1Turn);
        }

        foreach (var fov in Team2FOVs)
        {
            fov.Toggle(State == GameState.Team2Turn);
        }
    }

    public Vector2Int GetRandomUnitPos() {
        return State == GameState.Team1Turn ?
            Team1.First().transform.position.Get2DCoords() :
            Team2.First().transform.position.Get2DCoords();
     }

    private void UnitDied(GameObject unit) {
        if (unit.GetComponent<Unit>().Team == Team.Team1) {
            UnitFOV fov = unit.GetComponent<UnitFOV>();
            Team1FOVs.Remove(fov);
            Team1.Remove(unit);
        } else {
            UnitFOV fov = unit.GetComponent<UnitFOV>();
            Team2FOVs.Remove(fov);
            Team2.Remove(unit);
        }
        CheckForWin();
    }

    private void CheckForWin() {
        if (Team1.Count == 0) {
            print("Team 2 wins!");
            State = GameState.Team2Win;
            OnGameEnded.Invoke(State);
        }
        if (Team2.Count == 0) {
            print("Team 1 wins!");
            State = GameState.Team1Win;
            OnGameEnded.Invoke(State);
        }
    }

    void Start()
    {
        _map = GetComponent<MapController>();
        _map.SetupMap();
        _weaponFactory = GetComponent<WeaponFactory>();
        int midWidth = _map.unitWidth / 2;
        Team1 = new List<GameObject>();
        Team2 = new List<GameObject>();
        SpawnUnits(midWidth, 2);
        SpawnUnits(midWidth, _map.unitHeight - 3, true);
        State = GameState.Team2Turn;
        StartCoroutine(WaitToStart(.5f));
    }

    // TODO fix this to properly wait for load finish
    private IEnumerator WaitToStart(float timeDelay) {
        float end = Time.time + timeDelay;
        while (Time.time < end) {
            yield return null;
        }
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
                x += UnityEngine.Random.Range(-1, 2);
                y += UnityEngine.Random.Range(-1, 2);
                i--;
            }
        }
    }

    private void SpawnUnit(int x, int y, bool isTeam2)
    {
        GameObject unit = Instantiate((isTeam2 ? _unit2Prefab : _unit1Prefab), new Vector3(x, .8f, y), (isTeam2 ? Quaternion.Euler(0, 180, 0) : Quaternion.identity));
        unit.GetComponent<Unit>().Team = isTeam2 ? Team.Team2 : Team.Team1;
        _weaponFactory.GiveSniper(unit);
        WorldObject unitObject = new WorldObject(unit, WorldObjectType.Unit, 0);
        _map.PlaceObject(x, y, unitObject);
        if (isTeam2) {
            Team2.Add(unit);
            Team2FOVs.Add(unit.GetComponent<UnitFOV>());
        } else {
            Team1.Add(unit);
            Team1FOVs.Add(unit.GetComponent<UnitFOV>());
        }
    }
}

public enum GameState {
    Team1Turn,
    Team2Turn,
    Team1Win,
    Team2Win
}
