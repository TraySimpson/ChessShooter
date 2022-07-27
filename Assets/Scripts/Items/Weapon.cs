using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour, IUsable
{
    private bool _active;
    public WeaponSO WeaponStats;
    private WeaponLOS _weaponLOS;

    public UsableType GetUsableType() => UsableType.Weapon;
    public UsableSO GetStatSO() => WeaponStats;
    public bool IsActive() => _active;
    public void SetActive(bool active) {
        _active = active;
        gameObject.SetActive(active);
        gameObject.transform.Find("LOS").gameObject.SetActive(active);
    }
    public GameObject GetGameObject() => gameObject;

    public RaycastHit[] GetLOSHits() {
        if (_weaponLOS is null)
            _weaponLOS = gameObject.transform.Find("LOS").GetComponent<WeaponLOS>();
        return _weaponLOS.GetHits();
    }

    public void UpdateLOS() {
        _weaponLOS.UpdateLineRenderer();
    }
}
