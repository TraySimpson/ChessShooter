using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(MapController))]
[RequireComponent(typeof(GameController))]
[RequireComponent(typeof(WeaponController))]
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
    private Stack<GameObject> _movePath;
    private GameObject selectedUnit;
    private GameObject currentSelectedUnit;
    private int groundMask;
    private int uiMask;

    [SerializeField] private GameObject pathPrefab;

    void Start() {
        _map = GetComponent<MapController>();
        _gameController = GetComponent<GameController>();
        _weaponController = GetComponent<WeaponController>();
        _camera = Camera.main;
        _cameraController = _camera.GetComponent<CameraController>();
        _movePath = new Stack<GameObject>();
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
        if (!(worldObject is null) && worldObject.type == WorldObjectType.Unit)
            currentSelectedUnit = worldObject.gameObject;
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
                GameObject pathObject = Instantiate(pathPrefab, coordinates.ToVector3(), Quaternion.identity);
                _movePath.Push(pathObject);
                // TODO Handle undoing a path, break out as own class
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
            coordinates != currentSelectedUnit.Get2DCoordinates() &&
            (_movePath.Count == 0 || coordinates != _movePath.Peek().Get2DCoordinates());
    }

    private void OnTouchEnd(Vector3 touchPosition) {
        Vector2Int coordinates = touchPosition.Get2DCoords();
        WorldObject worldObject = _map.GetObjectAtCoords(coordinates);
        if (TouchedSingleUnit(worldObject)) {
            if (selectedUnit is null) {
                SelectUnit(currentSelectedUnit);
            } else {
                _weaponController.FireAtTarget(selectedUnit, worldObject.gameObject);
            }
        }
        MoveFromPath();
        CleanupTouchVars();
    }

    private bool TouchedSingleUnit(WorldObject worldObject) {
        return !(worldObject is null) && !(currentSelectedUnit is null) && GameObject.ReferenceEquals(worldObject.gameObject, currentSelectedUnit);
    }

    private void MoveFromPath() {
        if (_movePath.Count == 0) return;
        Stack<Vector2Int> forwardPath = new Stack<Vector2Int>();
        bool hasUpdatedMap = false;
        while (_movePath.Count > 0)
        {
            GameObject pathObj = _movePath.Pop();
            Vector2Int coords = pathObj.transform.position.Get2DCoords();
            if (!hasUpdatedMap) {
                _map.MoveObject(currentSelectedUnit, coords);
                hasUpdatedMap = true;
            }
            forwardPath.Push(coords);
            Destroy(pathObj);
        }
        StartCoroutine(MoveUnitAlongPath(currentSelectedUnit, forwardPath));
    }

    private IEnumerator MoveUnitAlongPath(GameObject unit, Stack<Vector2Int> path) {
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
        SelectUnit(null);
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