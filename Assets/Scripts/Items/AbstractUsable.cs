using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbstractUsable : MonoBehaviour, IUsable
{
    public GameObject GameObject;
    public Sprite Icon { get; set; }
    public bool _IsActive { get; set; }

    public void Equip()
    {
        GameObject.SetActive(true);
    }

    public void Unequip()
    {
        GameObject.SetActive(false);
    }

    public void Use()
    {
        print("Using weapon");
    }

    public bool IsActive() => _IsActive;
}
