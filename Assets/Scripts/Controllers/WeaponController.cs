using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public void FireAtTarget(GameObject shooter, GameObject target) {
        IDamagable targetHealth = target.GetComponent<IDamagable>();
        if (!(targetHealth is null))
            targetHealth.TakeDamage(100);
    }
}
