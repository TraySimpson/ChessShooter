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
    public Color32 InactiveGadgetColor { get; set; } = new Color32(38, 190, 217, 255);
    public Color32 ActiveGadgetColor { get; set; } = new Color32(255, 241, 42, 255);
}
