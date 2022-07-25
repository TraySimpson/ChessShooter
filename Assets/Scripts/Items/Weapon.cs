using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour, IUsable
{
    private bool _active;

    public WeaponSO WeaponStats;

    public UsableType GetUsableType() => UsableType.Weapon;
    public UsableSO GetStatSO() => WeaponStats;
    public bool IsActive() => _active;
    public void SetActive(bool active) {
        _active = active;
        gameObject.SetActive(active);
    }

}
