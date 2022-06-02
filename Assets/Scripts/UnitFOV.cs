using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFOV : MonoBehaviour
{
    [SerializeField] private float viewDistance;
    [SerializeField] private int viewDegrees;
    [SerializeField] private float degreeResolution;

    private LayerMask mask;
    [SerializeField] private List<Vector3> points;
    [SerializeField] private bool drawMisses = false;
    [SerializeField] private Material fovMaterial;
    [SerializeField] private Mesh fovMesh;
    private GameObject fovObject;
    private List<bool> pointHits;
    private float halfDegrees;
    private Vector3 _cachedPosition;
    private Vector3 positionOffset;

    void Start() {
        mask =~ LayerMask.GetMask("UnitGroup1");
        points = new List<Vector3>();
        SetupMesh();
        DrawFOV();
    }

    private void SetupMesh() {
        fovObject = new GameObject("FOV", typeof(MeshRenderer), typeof(MeshFilter));
        fovObject.transform.localScale = transform.localScale;
        fovMesh = new Mesh();
        fovObject.GetComponent<MeshFilter>().mesh = fovMesh;
        fovObject.GetComponent<MeshRenderer>().material = fovMaterial;
    }

    private void Update() {
        DrawFOV();
    }

    private void DrawFOV() {
        if (transform.hasChanged) {
            CalculateFOV();
            transform.hasChanged = false;
        }
    }

    private void CalculateFOV() {
        fovObject.transform.position = transform.position;
        fovObject.transform.rotation = transform.rotation;
        
        halfDegrees = viewDegrees / 2;
        points = new List<Vector3>();
        pointHits = new List<bool>();
        _cachedPosition = transform.position;
        Vector3 forward = transform.forward;

        // for (float verticalDegrees = -halfDegrees; verticalDegrees <= halfDegrees; verticalDegrees += degreeResolution) {
            for (float horizontalDegrees = -halfDegrees; horizontalDegrees <= halfDegrees; horizontalDegrees += degreeResolution) {
                RaycastPoint(Quaternion.Euler(0, horizontalDegrees, 0) * forward);
            }
        // }
        DrawFOVMesh();
    }

    private void RaycastPoint(Vector3 direction) {
        RaycastHit hit;
        if (Physics.Raycast(_cachedPosition, direction, out hit, viewDistance, mask)) {
            points.Add(hit.point);
            pointHits.Add(true);
        } else {
            points.Add((direction * viewDistance) + _cachedPosition);
            pointHits.Add(false);
        }
    }

    private void DrawFOVMesh() {
        int vertexCount = points.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(points[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        fovMesh.Clear();

        fovMesh.vertices = vertices;
        fovMesh.triangles = triangles;
        fovMesh.RecalculateNormals();
    }

    // private void OnDrawGizmos() {
    //     _cachedPosition = transform.position;
    //     int i = 0;
    //     foreach(Vector3 point in points) {
    //         Gizmos.color = pointHits[i] ? Color.red : Color.black;
    //         if (drawMisses || pointHits[i])
    //             Gizmos.DrawLine(_cachedPosition, point);
    //         i++;
    //     }
    // }
}
