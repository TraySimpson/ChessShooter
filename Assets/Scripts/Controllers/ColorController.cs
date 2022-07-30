using UnityEngine;

public class ColorController : MonoBehaviour
{
    private static ColorController instance;
    public static ColorController Instance {
        get {
            return instance ?? (instance = new GameObject("Singleton").AddComponent<ColorController>());
        }
    }

    // Gadget Wheel
    // Border
    public Color32 InactiveGadgetBorderColor { get; set; } = new Color32(38, 190, 217, 255);
    public Color32 ActiveGadgetBorderColor { get; set; } = new Color32(255, 241, 42, 255);
    public Color32 DisabledGadgetBorderColor { get; set; } = new Color32(80, 80, 80, 255);

    //Background
    public Color32 EnabledGadgetBGColor { get; set; } = new Color32(180, 180, 180, 255);
    public Color32 DisabledGadgetBGColor { get; set; } = new Color32(50, 50, 50, 255);
}
