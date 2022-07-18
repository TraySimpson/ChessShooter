using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(MapController))]
[RequireComponent(typeof(GameController))]
[RequireComponent(typeof(WeaponController))]
[RequireComponent(typeof(MovePath))]
public class TouchController : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    private CameraController _cameraController;
    private MapController _map;
    private GameController _gameController;
    private WeaponController _weaponController;

    // Pan
    [SerializeField] private Vector3? touchStart;

    // Rotate
    [SerializeField] private float rotationSpeed = 10f;
    private bool hitRotationHandle;
    private Quaternion startRotation;
    private int _fingerId;

    // Move 
    [SerializeField] private float timeToMove = .2f;

    // Unit Control
    private MovePath _movePath;
    private GameObject selectedUnit;
    private GameObject currentSelectedUnit;
    private int groundMask;
    private int uiMask;

    void Start() {
        _map = GetComponent<MapController>();
        _gameController = GetComponent<GameController>();
        _weaponController = GetComponent<WeaponController>();
        _movePath = GetComponent<MovePath>();
        _camera = Camera.main;
        _cameraController = _camera.GetComponent<CameraController>();
        groundMask = LayerMask.GetMask("Ground");
        uiMask = LayerMask.GetMask("UI");
    }

    void Update() {
        if (Input.touchCount == 1)
            HandleSingleTouch();
    }

    private void HandleSingleTouch() {
        Touch touch = Input.GetTouch(0);
        _fingerId = touch.fingerId;
        Vector3? touchPosition = GetTouchWorldPos(touch.position);
        //Touch outside the map
        if (touchPosition is null) {
            CleanupTouchVars();
            return;
        }

        switch (touch.phase) {
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

    private void OnTouchBegin(Vector3 touchPosition) {
        Vector2Int coordinates = touchPosition.Get2DCoords();
        WorldObject worldObject = _map.GetObjectAtCoords(coordinates);
        if (!(worldObject is null) && worldObject.Type == WorldObjectType.Unit)
            currentSelectedUnit = worldObject.GameObject;
        if (!(selectedUnit is null) && EventSystem.current.IsPointerOverGameObject(_fingerId)) {
            hitRotationHandle = true;
            startRotation = selectedUnit.transform.rotation;
        }
        touchStart = touchPosition;
    }

    private void OnTouchUpdate(Vector3 touchPosition) {
        if (!(currentSelectedUnit is null)) {
            Vector2Int coordinates = touchPosition.Get2DCoords();
            if (CanAddToPath(coordinates)) {
                _movePath.AddCoords(coordinates, _gameController.CurrentActionPoints);
            }
        } else if (!(touchStart is null)) {
            if (hitRotationHandle && !(selectedUnit is null)) {
                // Rotate
                selectedUnit.transform.rotation = startRotation * Quaternion.Euler(0, (touchPosition - touchStart.Value).x * rotationSpeed, 0);
            } else {
                // Pan
                Vector3 distance = touchStart.Value - touchPosition;
                _camera.transform.position += distance;
            }
        }
    }

    private bool CanAddToPath(Vector2Int coordinates) {
        return _map.CanPlace(coordinates.x, coordinates.y) &&
            coordinates != currentSelectedUnit.Get2DCoordinates();
    }

    private void OnTouchEnd(Vector3 touchPosition) {
        Vector2Int coordinates = touchPosition.Get2DCoords();
        WorldObject worldObject = _map.GetObjectAtCoords(coordinates);
        if (TouchedSingleUnit(worldObject)) {
            if (selectedUnit is null) {
                SelectUnit(currentSelectedUnit);
            } else {
                _weaponController.FireAtTarget(selectedUnit, worldObject.GameObject);
                _gameController.CurrentActionPoints--;
            }
        }
        MoveFromPath();
        CleanupTouchVars();
    }

    private bool TouchedSingleUnit(WorldObject worldObject) {
        return !(worldObject is null) && !(currentSelectedUnit is null) && GameObject.ReferenceEquals(worldObject.GameObject, currentSelectedUnit);
    }

    private void MoveFromPath() {
        if (_movePath.IsEmpty()) return;
        Vector2Int coords = _movePath.GetLast().transform.position.Get2DCoords();
        _map.MoveObject(currentSelectedUnit, coords);
        Stack<Vector2Int> forwardPath = _movePath.GetForwardPath();
        StartCoroutine(MoveUnitAlongPath(currentSelectedUnit, forwardPath));
    }

    private IEnumerator MoveUnitAlongPath(GameObject unit, Stack<Vector2Int> path) {
        // TODO Disable move input
        int actionPoints = path.Count;
        float unitY = unit.transform.position.y;
        Vector3 targetPosition = Vector3.zero;
        Vector2Int coords = Vector2Int.zero;
        while (path.Count > 0)
        {
            float elapsedTime = 0f;
            coords = path.Pop();
            Vector3 startingPos = unit.transform.position;
            Quaternion startingRotation = unit.transform.rotation;
            targetPosition = new Vector3(coords.x, unitY, coords.y);
            Quaternion targetRotation = Quaternion.LookRotation((targetPosition - startingPos).normalized);
            while (elapsedTime < timeToMove) {
                unit.transform.position = Vector3.Lerp(startingPos, targetPosition, (elapsedTime / timeToMove));
                unit.transform.rotation = Quaternion.Lerp(startingRotation, targetRotation, (elapsedTime / timeToMove));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        print($"Removing {actionPoints} ap");
        _gameController.CurrentActionPoints -= actionPoints;
        SelectUnit(null);
        // TODO Enable move input
    }

    private void CleanupTouchVars() {
        touchStart = null;
        currentSelectedUnit = null;
        hitRotationHandle = false;
    }

    public Vector3? GetTouchWorldPos(Vector3 touchPosition) {
        RaycastHit hit;
        Ray ray = _camera.ScreenPointToRay(touchPosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundMask)) {
            return hit.point;
        }
        return null;
    }

    private void SelectUnit(GameObject unit)
    {
        bool unitIsNull = unit is null;
        if (unitIsNull && selectedUnit)
            selectedUnit.GetComponent<Outline>().enabled = false;
        selectedUnit = unit;
        if (!unitIsNull) {
            selectedUnit.GetComponent<Outline>().enabled = true;
            // _cameraController.MoveToCoords(selectedUnit.transform.position.Get2DCoords());
        }
    }
}