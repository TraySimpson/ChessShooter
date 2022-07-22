using UnityEngine;
using System.Collections.Generic;

public class WeaponFactory : MonoBehaviour
{
    [SerializeField] private GameObject _sniperPrefab;

    public void GiveSniper(GameObject unit) {
        GameObject sniper = Instantiate(_sniperPrefab, unit.transform, false);
        Weapon sniperData = sniper.GetComponent<Weapon>();
        sniper.transform.parent = unit.transform;
        Unit unitData = unit.GetComponent<Unit>();
        unitData.Items.Add(sniperData);
    }
}
