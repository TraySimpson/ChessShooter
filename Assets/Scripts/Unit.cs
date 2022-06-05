using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] public Team Team {get; set;}
}

public enum Team {
    Team1,
    Team2
}
