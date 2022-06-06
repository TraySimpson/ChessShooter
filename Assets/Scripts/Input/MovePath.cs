using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePath : MonoBehaviour
{
    private List<GameObject> _movePath;
    [SerializeField] private GameObject _pathPrefab;

    public void Awake() {
        _movePath = new List<GameObject>();
    }

    public int GetPathLength() {
        return _movePath.Count;
    }

    public GameObject GetLast() {
        return _movePath.Count > 0 ? _movePath.Peek() : null;
    }

    public GameObject GetSecondLast() {
        return _movePath.Count > 1 ? _movePath[_movePath.Count-2] : null;
    }

    private Vector2Int? GetLastCoords() {
        Vector2Int? coords = null;
        if ((_movePath.Count > 0))
            coords = GetLast().Get2DCoordinates();
        return coords;
    }

    private Vector2Int? GetSecondLastCoords() {
        Vector2Int? coords = null;
        if ((_movePath.Count > 1))
            coords = GetSecondLast().Get2DCoordinates();
        return coords;
    }

    public void AddCoords(Vector2Int coords, int maxLength) {
        Vector2Int? lastCoords = GetLastCoords();
        if (_movePath.Count != 0 && coords == lastCoords) return;

        Vector2Int? secondLastCoords = GetSecondLastCoords();
        if (coords == secondLastCoords) {
            RemoveLast();
        } else {
            if (GetPathLength() == maxLength) return;
            GameObject pathObject = Instantiate(_pathPrefab, coords.ToVector3(), Quaternion.identity);
            _movePath.Add(pathObject);
        }
    }

    public void RemoveLast() {
        GameObject last = GetLast();
        _movePath.RemoveAt(_movePath.Count-1);
        Destroy(last);
    }

    public bool IsEmpty() {
        return _movePath.Count == 0;
    }

    public Stack<Vector2Int> GetForwardPath() {
        Stack<Vector2Int> forwardPath = new Stack<Vector2Int>();
        for (int i = _movePath.Count-1; i >= 0; i--)
        {
            GameObject pathObj = _movePath[i];
            _movePath.RemoveAt(i);
            Vector2Int coords = pathObj.transform.position.Get2DCoords();
            forwardPath.Push(coords);
            Destroy(pathObj);
        }
        return forwardPath;
    }
}
