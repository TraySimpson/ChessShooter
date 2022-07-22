using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField] private TMP_Text teamTurnText;

    private void Awake() {
        GameController.OnTurnChanged += ChangeTurn;
    }

    private void OnDestroy() {
        GameController.OnTurnChanged -= ChangeTurn;
    }

    public void ChangeTurn(GameState state, Vector2Int position) {
        teamTurnText.text = state == GameState.Team1Turn ? 
            "Team 1 turn" :
            "Team 2 turn";
    }

}
