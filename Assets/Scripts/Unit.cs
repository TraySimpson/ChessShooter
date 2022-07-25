using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Unit : MonoBehaviour
{
    [SerializeField] public Team Team { get; set; }
    public List<IUsable> Items { get; set; }
    public IUsable ActiveItem() {
        var activeItems = Items.Where(i => i.IsActive());
        return activeItems.Count() == 0 ? null : activeItems.First();
    }

    private void Awake() {
        Items = new List<IUsable>();
    }
}

public enum Team {
    Team1,
    Team2
}
