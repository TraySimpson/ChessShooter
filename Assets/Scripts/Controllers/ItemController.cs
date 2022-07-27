using System;
using System.Linq;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    #region Events
    public static event Action<Unit> OnActiveItemSwitched = delegate { };
    #endregion

    [SerializeField] private TouchController _touchController;

    [SerializeField] private float MaxDistance = 40f;
    // public void FireAtTarget(GameObject shooter, GameObject target) {
    //     IDamagable targetHealth = target.GetComponent<IDamagable>();
    //     if (!(targetHealth is null))
    //         targetHealth.TakeDamage(100);
    // }

    private void Start() {
        _touchController = GetComponent<TouchController>();
    }

    public void EquipItem(int buttonIndex) {
        Unit unit = _touchController.SelectedUnit.GetComponent<Unit>();
        unit.ActiveItem()?.SetActive(false);
        unit.Items.ToArray()[buttonIndex-1].SetActive(true);
        OnActiveItemSwitched?.Invoke(unit);
    }

    public void UseItem() {
        print("Using item");
        IUsable item = _touchController.SelectedUnit.GetComponent<Unit>().ActiveItem();
        UseItemSpecific(_touchController.SelectedUnit, item);
    }

    public void UseItemSpecific(GameObject user, IUsable item) {
        switch (item.GetUsableType())
        {
            case UsableType.Weapon:
                FireWeapon(user, (Weapon)item);
                break;
            case UsableType.Throwable:
                break;
            case UsableType.Deployable:
                break;
            default:
                throw new System.Exception("Item type not recognized");
        }
    }

    private void FireWeapon(GameObject user, Weapon item) {
        WeaponSO stats = (WeaponSO)item.GetStatSO();
        if (stats.SpreadDegrees == 0) {
            RaycastHit[] hits = item.GetLOSHits();
            int penetrationReduction = 0;
            print("Hits: " + hits.Length);
            foreach (RaycastHit hit in hits)
            {
                // Hit doesn't exist anymore
                if (hit.transform is null) continue;
                IDamagable damageHit = hit.transform.gameObject.GetComponent<IDamagable>();
                int damage = GetDamageAtRange(stats, hit.distance, penetrationReduction);
                if (damage < 1) break;
                damageHit.TakeDamage(damage);
                penetrationReduction += damageHit.GetResistance();
            }
        }
        else {
            //TODO Shotgun projectiles
        }
        item.UpdateLOS();
    }

    private int GetDamageAtRange(WeaponSO stats, float distance, int penetrationReduction) {
        int dropOffRange = stats.FalloffEnd - stats.FalloffStart;
        float dropOffFactor = Mathf.Abs(distance - stats.FalloffStart) / dropOffRange;
        float damage = Mathf.Lerp(
            stats.MaxDamage, 
            stats.MinDamage,
            dropOffFactor);
        int penetration = stats.Penetration - penetrationReduction;
        if (penetration < 1) return 0;
        return Mathf.RoundToInt(damage * (penetration/stats.Penetration));
    }
}