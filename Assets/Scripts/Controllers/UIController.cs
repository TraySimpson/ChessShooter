using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField] private TMP_Text teamTurnText;
    [SerializeField] private Canvas gadgetWheel;
    [SerializeField] private Color activeItemColor;
    [SerializeField] private Color inactiveItemColor;


    private void Awake() {
        GameController.OnTurnChanged += ChangeTurn;
        TouchController.OnUnitSelected += SelectUnit;
        ItemController.OnActiveItemSwitched += UpdateGadgetWheel;
    }

    private void OnDestroy() {
        GameController.OnTurnChanged -= ChangeTurn;
        TouchController.OnUnitSelected -= SelectUnit;
        ItemController.OnActiveItemSwitched -= UpdateGadgetWheel;
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
        ToggleUseButton(!(unit is null));
        UpdateGadgetWheel(unit.GetComponent<Unit>());
    }

    private void ToggleUseButton(bool isActive) {
        Transform useButtonUI = gadgetWheel.transform.Find($"FireButton");
        useButtonUI.gameObject.SetActive(isActive);
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
            SetItemUIStatus(item.IsActive(), gadgetBorderUI.gameObject, gadgetBorderUI.Find("HexMask").gameObject);
            i++;
        }
        while (i < 4) {
            gadgetWheel.transform.Find($"HexBorder {i}").gameObject.SetActive(false);
            i++;
        }
    }

    private void SetItemUIStatus(bool isActive, GameObject hexBorder, GameObject hexMask) {
        hexBorder.GetComponent<Image>().color = isActive ? activeItemColor : inactiveItemColor;
        hexBorder.GetComponent<RectTransform>().sizeDelta = isActive ? new Vector2(150, 150) : new Vector2(144, 144);
        hexMask.GetComponent<RectTransform>().sizeDelta = isActive ? new Vector2(128, 128) : new Vector2(128, 144);
    }

}
