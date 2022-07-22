using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Unit : MonoBehaviour
{
    [SerializeField] public Team Team { get; set; }
    public ICollection<IUsable> Items { get; set; }
    public IUsable ActiveItem => Items.Where(i => i.IsActive()).First();

    private void Awake() {
        Items = new List<IUsable>();
    }
}

public enum Team {
    Team1,
    Team2
}
