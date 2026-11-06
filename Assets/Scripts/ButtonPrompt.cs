using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(UnityEngine.UI.Image))]
public class ButtonPrompt : MonoBehaviour
{
    private InputAction leapAction;
    private double dismissedTime = 0.0f;
    private UnityEngine.UI.Image image;

    void Start()
    {
        leapAction = InputSystem.actions.FindAction("Leap");
        image = GetComponent<UnityEngine.UI.Image>();
    }

    void Update()
    {
        if (leapAction.WasPressedThisFrame() && dismissedTime == 0.0f)
        {
            dismissedTime = Time.timeAsDouble;
        }
        if (dismissedTime != 0.0f)
        {
            double diff = Time.timeAsDouble - dismissedTime;
            float alpha = Mathf.Clamp(1 - (float)diff, 0, 1);
            var tempColor = image.color;
            tempColor.a = alpha;
            image.color = tempColor;
        }
    }
}
