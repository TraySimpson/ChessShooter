using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Unit : MonoBehaviour
{
    [SerializeField] public Team Team { get; set; }
    public IEnumerable<IUsable> Items { get; set; }
    public IUsable ActiveItem => Items.Where(i => i.IsActive()).First();
}

public enum Team {
    Team1,
    Team2
}
