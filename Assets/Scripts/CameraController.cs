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


    private void Start()
    {
        targetPosition = null;
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
            coordinates.y - CAMERA_Z_OFFSET
        );
    }

    public void MoveToCoords(Vector2Int coordinates, float yAdjustment)
    {
        targetPosition = new Vector3(
            coordinates.x,
            transform.position.y + yAdjustment,
            coordinates.y - CAMERA_Z_OFFSET
        );
    }
}
