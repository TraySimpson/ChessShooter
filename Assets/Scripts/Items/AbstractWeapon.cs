using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbstractWeapon : MonoBehaviour, IUsable
{
    public GameObject GameObject;
    public bool _IsActive { get; set; }

    public void Equip() {
        GameObject.SetActive(true);
    }

    public void Unequip() {
        GameObject.SetActive(false);
    }

    public void Use() {
        print("Using weapon");
    }

    public bool IsActive() => _IsActive;
}
