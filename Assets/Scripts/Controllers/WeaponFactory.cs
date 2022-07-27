using UnityEngine;
using System.Collections.Generic;

public class WeaponFactory : MonoBehaviour
{
    [SerializeField] private GameObject _sniperPrefab;
    [SerializeField] private GameObject _pistolPrefab;
    [SerializeField] private GameObject _arPrefab;

    public void GiveSniper(GameObject unit) {
        GiveWeapon(unit, _sniperPrefab);
    }

    public void GiveAR(GameObject unit) {
        GiveWeapon(unit, _arPrefab);
    }

    public void GivePistol(GameObject unit) {
        GiveWeapon(unit, _pistolPrefab);
    }

    private void GiveWeapon(GameObject unit, GameObject prefab) {
        GameObject weapon = Instantiate(prefab, unit.transform, false);
        Weapon sniperData = weapon.GetComponent<Weapon>();
        weapon.transform.parent = unit.transform;
        Unit unitData = unit.GetComponent<Unit>();
        sniperData.SetActive(unitData.Items.Count == 0);
        unitData.Items.Add(sniperData);
    }
}
