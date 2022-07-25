using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField] private TMP_Text teamTurnText;
    [SerializeField] private Canvas gadgetWheel;

    private void Awake() {
        GameController.OnTurnChanged += ChangeTurn;
        TouchController.OnUnitSelected += SelectUnit;
    }

    private void OnDestroy() {
        GameController.OnTurnChanged -= ChangeTurn;
        TouchController.OnUnitSelected -= SelectUnit;
    }

    private void Start() {
        foreach (Transform child in gadgetWheel.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void ChangeTurn(GameState state, Vector2Int position) {
        teamTurnText.text = state == GameState.Team1Turn ? 
            "Team 1 turn" :
            "Team 2 turn";
    }

    public void SelectUnit(GameObject unit) {
        // TODO Setup unit UI and update here

        UpdateGadgetWheel(unit.GetComponent<Unit>());
    }

    private void UpdateGadgetWheel(Unit unit) {
        int i = 1;
        foreach (IUsable item in unit.Items)
        {
            UsableSO gadgetSO = item.GetStatSO();
            Transform gadgetBorderUI = gadgetWheel.transform.Find($"HexBorder {i}");
            gadgetBorderUI.gameObject.SetActive(true);
            Transform gadgetIconUI = gadgetBorderUI.Find("HexMask").Find("BG").Find("Gadget");
            gadgetIconUI.gameObject.GetComponent<Image>().sprite = gadgetSO.Icon;
            i++;
        }
        while (i < 4) {
            gadgetWheel.transform.Find($"HexBorder {i}").gameObject.SetActive(false);
            i++;
        }
    }

}
