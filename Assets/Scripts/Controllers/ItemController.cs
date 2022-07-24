using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField] private float MaxDistance = 40f;
    // public void FireAtTarget(GameObject shooter, GameObject target) {
    //     IDamagable targetHealth = target.GetComponent<IDamagable>();
    //     if (!(targetHealth is null))
    //         targetHealth.TakeDamage(100);
    // }

    public void UseItem(GameObject user, IUsable item) {
        switch (item.GetUsableType())
        {
            case UsableType.Weapon:
                FireWeapon(user, item);
                break;
            case UsableType.Throwable:
                break;
            case UsableType.Deployable:
                break;
            default:
                throw new System.Exception("Item type not recognized");
        }
    }

    private void FireWeapon(GameObject user, IUsable item) {
        WeaponSO stats = (WeaponSO)item.GetStatSO();
        if (stats.SpreadDegrees == 0) {
            RaycastHit[] hits = GetHits(user.transform);
        }
        else {
            //TODO Shotgun projectiles
        }
    }

    private RaycastHit[] GetHits(Transform startTransform) {
        Vector3 direction = startTransform.TransformDirection(Vector3.forward);
        RaycastHit[] hits = new RaycastHit[5];
        Physics.RaycastNonAlloc(startTransform.position, direction, hits, MaxDistance);
        return hits;
    }
}
