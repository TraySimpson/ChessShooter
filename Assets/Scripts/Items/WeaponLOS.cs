using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLOS : MonoBehaviour
{
    private WeaponSO weaponStats;
    private LineRenderer lineRenderer;
    private RaycastHit[] hits;
    private IComparer<RaycastHit> _raycastComparer;
    [SerializeField] private int distancePastFalloff = 2;

    void Start()
    {
        _raycastComparer = new RaycastHitComparer();
        weaponStats = (WeaponSO)transform.parent.GetComponent<Weapon>().GetStatSO();
        lineRenderer = GetComponent<LineRenderer>();
        SetLineLength();
    }

    void Update()
    {
        if (transform.parent.transform.hasChanged)
        {
            UpdateLineRenderer();
            transform.parent.transform.hasChanged = false;
        }
    }

    void SetLineLength() {
        var points = new Vector3[2];
        points[0] = Vector3.zero;
        points[1] = new Vector3(0, 0, weaponStats.MaxDistance);
        lineRenderer.SetPositions(points);
    }

    public void UpdateLineRenderer() {
        //Get line collisions
        hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), weaponStats.MaxDistance);
        Array.Sort(hits, _raycastComparer);

        //Update width points
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0.0f, 0.05f);
        curve.AddKey(1.0f, 0.02f);
        lineRenderer.widthCurve = curve;

        //Update color gradient
        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.green, 0.0f), new GradientColorKey(Color.red, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );
        lineRenderer.colorGradient = gradient;
    }

    public RaycastHit[] GetHits() => hits;
}

public class RaycastHitComparer : IComparer<RaycastHit> {
    public int Compare(RaycastHit a, RaycastHit b) {
        return a.distance.CompareTo(b.distance);
    }
}
