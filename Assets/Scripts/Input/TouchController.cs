using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchController : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    private CameraController _cameraController;
    private MapController _map;
    private GameController _gameController;

    // Pan
    [SerializeField] private Vector3? touchStart;

    // Rotate
    [SerializeField] private float rotationSpeed = 10f;
    private bool hitRotationHandle;
    private Quaternion startRotation;
    private int _fingerId;

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
        Vector3? touchPosition = GetTouchWorldPos(touch.position, touch.phase == TouchPhase.Began);

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
            if (coordinates != currentSelectedUnit.Get2DCoordinates() && (_movePath.Count == 0 || coordinates != _movePath.Peek().Get2DCoordinates())) {
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

    private void OnTouchEnd(Vector3 touchPosition) {
        Vector2Int coordinates = touchPosition.Get2DCoords();
        WorldObject worldObject = _map.GetObjectAtCoords(coordinates);
        if (!(worldObject is null) && !(currentSelectedUnit is null) && GameObject.ReferenceEquals(worldObject.gameObject, currentSelectedUnit)) {
            SelectUnit(selectedUnit is null ? currentSelectedUnit : null);
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
        hitRotationHandle = false;
    }

    public Vector3? GetTouchWorldPos(Vector3 touchPosition, bool startTouch) {
        RaycastHit hit;
        if (Physics.Raycast(_camera.ScreenPointToRay(touchPosition), out hit, Mathf.Infinity, uiMask))
            hitRotationHandle = true;
        if (Physics.Raycast(_camera.ScreenPointToRay(touchPosition), out hit, Mathf.Infinity, groundMask))
            return hit.point;
        return null;
    }

    private void MoveUnit(Vector2Int coordinates)
    {
        _map.MoveObject(currentSelectedUnit, coordinates);
        _cameraController.MoveToCoords(coordinates);
        SelectUnit(null);
    }

    private void SelectUnit(GameObject unit)
    {
        bool unitIsNull = unit is null;
        if (unitIsNull && selectedUnit)
            selectedUnit.GetComponent<Outline>().enabled = false;
        selectedUnit = unit;
        if (!unitIsNull) {
            selectedUnit.GetComponent<Outline>().enabled = true;
            _cameraController.MoveToCoords(selectedUnit.transform.position.Get2DCoords());
        }
    }
}