using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitDamage : MonoBehaviour, IDamagable
{
    public static event Action<GameObject> OnUnitDied = delegate { };

    [SerializeField] private int totalHealth = 100;
    [SerializeField] private int currentHealth;

    private void Start() {
        currentHealth = totalHealth;
    }

    public void TakeDamage(int damage) {
        currentHealth -= damage;
        print("Unit took " + damage + " points of damage");
        if (currentHealth <= 0)
            Die();
    }

    private void Die() {
        OnUnitDied(gameObject);
        Destroy(gameObject);
        Debug.Log("Unit died");
    }

    public int GetHealth() => currentHealth;
    public int GetResistance() => totalHealth;
}
