using UnityEngine;
using System.Collections.Generic;

public class WeaponFactory : MonoBehaviour
{
    [SerializeField] private GameObject _sniperPrefab;
    [SerializeField] private GameObject _pistolPrefab;

    public void GiveSniper(GameObject unit) {
        GiveWeapon(unit, _sniperPrefab);
    }

    public void GivePistol(GameObject unit) {
        GiveWeapon(unit, _pistolPrefab);
    }

    private void GiveWeapon(GameObject unit, GameObject prefab) {
        GameObject weapon = Instantiate(prefab, unit.transform, false);
        Weapon sniperData = weapon.GetComponent<Weapon>();
        weapon.transform.parent = unit.transform;
        Unit unitData = unit.GetComponent<Unit>();
        unitData.Items.Add(sniperData);
    }
}
