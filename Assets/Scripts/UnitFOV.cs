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
    [SerializeField] private Material fovMaterial;
    [SerializeField] private Mesh fovMesh;
    private GameObject fovObject;
    private List<bool> pointHits;
    private float halfDegrees;
    private Vector3 _cachedPosition;
    private Vector3 positionOffset;

    void Start() {
        mask =~ LayerMask.GetMask("UnitGroup1", "UnitGroup2");
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
        fovObject.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
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
        Vector3[] vertices = new Vector3[vertexCount + 4];
        int[] triangles = new int[(vertexCount - 2) * 3 + 6];

        //Draw FOV
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

        //Draw surrounding Box
        Vector2Int coordinates = transform.position.Get2DCoords();
        float yOffset = transform.position.y;
        vertices[vertices.Length - 4] = transform.InverseTransformPoint(new Vector3(coordinates.x - 1.5f, yOffset, coordinates.y + 1.5f));
        vertices[vertices.Length - 3] = transform.InverseTransformPoint(new Vector3(coordinates.x + 1.5f, yOffset, coordinates.y + 1.5f));
        vertices[vertices.Length - 2] = transform.InverseTransformPoint(new Vector3(coordinates.x - 1.5f, yOffset, coordinates.y - 1.5f));
        vertices[vertices.Length - 1] = transform.InverseTransformPoint(new Vector3(coordinates.x + 1.5f, yOffset, coordinates.y - 1.5f));

        triangles[triangles.Length - 6] = vertices.Length - 4;
        triangles[triangles.Length - 5] = vertices.Length - 3;
        triangles[triangles.Length - 4] = vertices.Length - 2;
        triangles[triangles.Length - 3] = vertices.Length - 2;
        triangles[triangles.Length - 2] = vertices.Length - 3;
        triangles[triangles.Length - 1] = vertices.Length - 1;

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
