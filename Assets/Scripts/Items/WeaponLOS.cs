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
    
    private int? _unitInstanceId;
    private bool _isActive;

    #region Event listener
    private void Awake() {
        TouchController.OnUnitSelected += ToggleFromUnitSelected;
        ItemController.OnActiveItemSwitched += ToggleFromUnitSelected;
        _raycastComparer = new RaycastHitComparer();
        weaponStats = (WeaponSO)transform.parent.GetComponent<Weapon>().GetStatSO();
        lineRenderer = GetComponent<LineRenderer>();
        ToggleVisibility(false);
    }

    private void OnDisable() {
        TouchController.OnUnitSelected -= ToggleFromUnitSelected;
        ItemController.OnActiveItemSwitched -= ToggleFromUnitSelected;
    }
    #endregion

    public void ToggleFromUnitSelected(Unit unit) {
        ToggleFromUnitSelected(unit.ActiveItem().GetGameObject());
    }

    public void ToggleFromUnitSelected(GameObject unit) {
        if (_unitInstanceId is null)
            _unitInstanceId = transform.parent.parent.gameObject.GetInstanceID();
        print("Comparing weapon LOS ");
        ToggleVisibility(!(unit is null) && _unitInstanceId == unit.GetInstanceID());
    }

    private void ToggleVisibility(bool isActive) {
        _isActive = isActive;
        lineRenderer.enabled = isActive;
    }

    void Start()
    {
        SetLineLength();
    }

    void Update()
    {
        if (transform.parent.transform.hasChanged && _isActive)
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
