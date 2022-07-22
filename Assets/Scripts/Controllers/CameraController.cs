using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    int CAMERA_Z_OFFSET = 4;

    [SerializeField]
    private float smoothTime = 4f;
    private Vector3 velocity = Vector3.zero;
    private Vector3? targetPosition;
    private bool isFlipped;

    #region Turn Events
    private void Awake()
    {
        GameController.OnTurnChanged += ChangeTurn;
    }

    private void OnDestroy()
    {
        GameController.OnTurnChanged -= ChangeTurn;
    }

    public void ChangeTurn(GameState state, Vector2Int position) 
    {
        if (state == GameState.Team1Turn) {
            transform.rotation = Quaternion.Euler(60, 0, 0);
            isFlipped = false;
        } else {
            transform.rotation = Quaternion.Euler(60, 180, 0);
            isFlipped = true;
        }
        MoveToCoords(position);
    }
    #endregion

    private void Start()
    {
        targetPosition = null;
        isFlipped = false;
    }

    private void Update()
    {
        if (targetPosition is null) return;
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition.Value,
            ref velocity,
            smoothTime);
        if (transform.position == targetPosition) targetPosition = null;
    }

    public void MoveToCoords(Vector2Int coordinates)
    {
        targetPosition = new Vector3(
            coordinates.x,
            transform.position.y,
            (isFlipped ? coordinates.y + CAMERA_Z_OFFSET : coordinates.y - CAMERA_Z_OFFSET)
        );
    }

    public void MoveToCoords(Vector2Int coordinates, float yAdjustment)
    {
        targetPosition = new Vector3(
            coordinates.x,
            transform.position.y + yAdjustment,
            (isFlipped ? coordinates.y + CAMERA_Z_OFFSET : coordinates.y - CAMERA_Z_OFFSET)
        );
    }
}
