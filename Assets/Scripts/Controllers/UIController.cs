using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField] private TMP_Text teamTurnText;
    [SerializeField] private TMP_Text actionPointText;
    private string actionPointPrefix;
    [SerializeField] private Canvas gadgetWheel;
    [SerializeField] private GameObject passToPlayerDialog;
    [SerializeField] private bool _showDialogBetweenTurns;

    private Unit _unit;

    #region Events
    private void Awake() {
        GameController.OnTurnChanged += ChangeTurn;
        GameController.OnActionPointsChanged += ChangeActionPoints;
        TouchController.OnUnitSelected += SelectUnit;
        ItemController.OnActiveItemSwitched += UpdateGadgetWheel;
    }

    private void OnDestroy() {
        GameController.OnTurnChanged -= ChangeTurn;
        GameController.OnActionPointsChanged += ChangeActionPoints;
        TouchController.OnUnitSelected -= SelectUnit;
        ItemController.OnActiveItemSwitched -= UpdateGadgetWheel;
    }
    #endregion

    private void Start() {
        actionPointPrefix = actionPointText.text;
        HideGadgetWheel();
    }

    private void HideGadgetWheel() {
        foreach (Transform child in gadgetWheel.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void ChangeTurn(GameState state, Vector2Int position) {
        teamTurnText.text = state == GameState.Team1Turn ? 
            "Team 1 turn" :
            "Team 2 turn";
        if (_showDialogBetweenTurns) {
            TogglePassToPlayerDialog(true);
        }
        HideGadgetWheel();
    }

    public void ChangeActionPoints(int points) {
        actionPointText.text = actionPointPrefix + points.ToString();
    }

    public void TogglePassToPlayerDialog(bool showDlg) {
        passToPlayerDialog.SetActive(showDlg);
    }

    public void SelectUnit(GameObject unit) {
        // TODO Setup unit UI and update here
        bool unitIsSelected = !(unit is null);
        ToggleUseButton(unitIsSelected);
        if (unitIsSelected) {
            UpdateGadgetWheel(unit.GetComponent<Unit>());
        } else {
            HideGadgetWheel();
        }        
    }

    private void ToggleUseButton(bool isActive) {
        Transform useButtonUI = gadgetWheel.transform.Find($"FireButton");
        useButtonUI.gameObject.SetActive(isActive);
    }

    private void UpdateGadgetWheel(Unit unit)
    {
        UpdateGadgetWheelWithAP(unit, GameController.Instance.CurrentActionPoints);
    }

    private void UpdateGadgetWheelWithAP(Unit unit, int maxAP) {
        int i = 1;
        print("Max AP: "+ maxAP);
        foreach (IUsable item in unit.Items)
        {
            UsableSO itemSO = item.GetStatSO();
            bool enabled = itemSO.APCost <= maxAP;
            Transform gadgetBorderUI = gadgetWheel.transform.Find($"HexBorder {i}");
            gadgetBorderUI.gameObject.SetActive(true);
            gadgetBorderUI.gameObject.GetComponent<Button>().interactable = enabled;
            Transform gadgetIconUI = gadgetBorderUI.Find("HexMask").Find("BG").Find("Gadget");
            gadgetIconUI.gameObject.GetComponent<Image>().sprite = itemSO.Icon;
            SetItemUIStatus(item.IsActive(), enabled, gadgetBorderUI.gameObject, gadgetBorderUI.Find("HexMask").gameObject);
            i++;
        }
        while (i < 4) {
            gadgetWheel.transform.Find($"HexBorder {i}").gameObject.SetActive(false);
            i++;
        }
    }

    private void SetItemUIStatus(bool isActive, bool isEnabled, GameObject hexBorder, GameObject hexMask) {
        Color32 borderColor = ((isEnabled, isActive)) switch 
        {
            (false, false) => ColorController.Instance.DisabledGadgetBorderColor,
            (false, true) => ColorController.Instance.DisabledGadgetBorderColor,
            (true, false) => ColorController.Instance.InactiveGadgetBorderColor,
            (true, true) => ColorController.Instance.ActiveGadgetBorderColor,
        };
        hexBorder.GetComponent<Image>().color = borderColor;
        GameObject background = hexMask.transform.Find("BG").gameObject;
        background.GetComponent<Image>().color = isEnabled ? ColorController.Instance.EnabledGadgetBGColor : ColorController.Instance.DisabledGadgetBGColor;
        hexBorder.GetComponent<RectTransform>().sizeDelta = isActive ? new Vector2(150, 150) : new Vector2(144, 144);
        hexMask.GetComponent<RectTransform>().sizeDelta = isActive ? new Vector2(128, 128) : new Vector2(128, 144);
    }

    public void OnUnitIconPressed(int index) {

    }
}
