using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    private MapController _map;
    private GameController _gameController;

    //Pan
    [SerializeField] private Vector3? touchStart;

    //Unit Control
    private Stack<GameObject> _movePath;
    private GameObject selectedUnit;
    private GameObject currentSelectedUnit;
    private int groundMask;

    [SerializeField] private GameObject pathPrefab;

    void Start()
    {
        _map = GetComponent<MapController>();
        _gameController = GetComponent<GameController>();
        _camera = Camera.main;
        _movePath = new Stack<GameObject>();
        groundMask = LayerMask.GetMask("Ground");
    }

    void Update()
    {
        if (Input.touchCount == 1)
            HandleSingleTouch();
    }

    private void HandleSingleTouch()
    {
        Touch touch = Input.GetTouch(0);
        Vector3? touchPosition = GetTouchWorldPos(touch.position);
        if (touchPosition is null) {
            print($"shits' broke {touch.phase}");
            CleanupTouchVars();
            return;
        }
        switch(touch.phase) {
            case TouchPhase.Began:
                OnTouchBegin(touchPosition.Value);
                break;
            case TouchPhase.Moved:
                OnTouchUpdate(touchPosition.Value);
                break;
            case TouchPhase.Stationary:
                break;
            default:
                OnTouchEnd(touchPosition.Value);
                break;
        }
    }

    private void OnTouchBegin(Vector3 touchPosition)
    {
        Vector2Int coordinates = touchPosition.Get2DCoords();
        WorldObject worldObject = _map.GetObjectAtCoords(coordinates);
        if (!(worldObject is null) && worldObject.type == WorldObjectType.Unit) {
            currentSelectedUnit = worldObject.gameObject;
        }
        touchStart = touchPosition;
    }

    private void OnTouchUpdate(Vector3 touchPosition)
    {
        if (!(currentSelectedUnit is null)) {
            Vector2Int coordinates = touchPosition.Get2DCoords();
            if (coordinates != currentSelectedUnit.Get2DCoordinates() && (_movePath.Count == 0 || coordinates != _movePath.Peek().Get2DCoordinates())) {
                GameObject pathObject = Instantiate(pathPrefab, coordinates.ToVector3(), Quaternion.identity);
                _movePath.Push(pathObject);
                // TODO Handle undoing a path, break out as own class
            }
        } else if (!(touchStart is null)) {
            Vector3 distance = touchStart.Value - touchPosition;
            _camera.transform.position += distance;
        }
    }

    private void OnTouchEnd(Vector3 touchPosition)
    {
        Vector2Int coordinates = touchPosition.Get2DCoords();
        WorldObject worldObject = _map.GetObjectAtCoords(coordinates);
        if (!(worldObject is null) && !(selectedUnit is null) && GameObject.ReferenceEquals(worldObject.gameObject, selectedUnit))
        {
            selectedUnit = null;
        }
        bool firstPop = true;
        while (_movePath.Count > 0) {
            GameObject pathObj = _movePath.Pop();
            if (firstPop) {
                firstPop = false;
                MoveUnit(coordinates);
            }
            Destroy(pathObj);
        }
        CleanupTouchVars();
    }

    private void CleanupTouchVars() {
        touchStart = null;
        _movePath = new Stack<GameObject>();
        currentSelectedUnit = null;
    }

    public Vector3? GetTouchWorldPos(Vector3 touchPosition) {
        RaycastHit hit;
        if (Physics.Raycast(_camera.ScreenPointToRay(touchPosition), out hit, Mathf.Infinity, groundMask))
            return hit.point;
        return null;
    }

    private void MoveUnit(Vector2Int coordinates)
    {
        _map.MoveObject(currentSelectedUnit, coordinates);
        _camera.MoveToCoords(coordinates);
        SelectUnit(null);
    }

    private void SelectUnit(GameObject unit)
    {
        // selectedUnit = unit;
        // if (!(selectedUnit is null))
        //     selectedUnit.GetComponent<Outline>().enabled = unit is null;
        // if (!isSelected)
        //     selectedUnit = null;
    }
}