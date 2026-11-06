using UnityEngine;
using UnityEngine.UI;

public class Disappearing : MonoBehaviour
{
    private double startTime = 0.0f;
    public float staticDuration = 0;
    public float fadeDuration = 1;
    private Text text;

    void Start()
    {
        startTime = Time.timeAsDouble;
        text = GetComponent<Text>();
    }

    void Update()
    {
        double diff = Time.timeAsDouble - staticDuration - startTime;
        float alpha = Mathf.Clamp(1 - (float)diff, 0, 1);
        var tempColor = text.color;
        tempColor.a = alpha;
        text.color = tempColor;
    }
}
