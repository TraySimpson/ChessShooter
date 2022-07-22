using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour, IUsable
{
    public bool _isActive {get; set;}
    public WeaponSO WeaponStats;

    public UsableType GetUsableType() => UsableType.Weapon;
    public UsableSO GetStatSO() => WeaponStats;
    public bool IsActive() => _isActive;
}
