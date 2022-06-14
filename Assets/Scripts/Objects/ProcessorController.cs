using TMPro;
using UnityEngine;

public class ProcessorController : MonoBehaviour
{
    public float load;
    public float loadSmoothing;
    private float smoothedLoad;
    private float loadVelocity;

    public TMP_Text cpuText, loadText;

    private Color color, shadowColor;
    public Color Color => color;
    private Material material;

    private void Awake()
    {
        material = GetComponent<MeshRenderer>().material;
    }

    private void Update()
    {
        smoothedLoad = Mathf.SmoothDamp(smoothedLoad, load, ref loadVelocity, loadSmoothing);

        loadText.text = $"{Mathf.FloorToInt(smoothedLoad * 100)}%";
        material.SetFloat("_ShadowPos", smoothedLoad);
    }

    public void Setup(int index, float hue)
    {
        cpuText.text = $"CPU {index}";

        color = Color.HSVToRGB(hue, 1f, 1);
        shadowColor = Color.HSVToRGB(hue, 0.3f, 0.7f);
        material.SetColor("_Color", color);
        material.SetColor("_ShadowColor", shadowColor);

        Color darkerColor = new Color(color.r * 0.3f, color.g * 0.3f, color.b * 0.3f, color.a);
        cpuText.color = darkerColor;
        loadText.color = darkerColor;

        gameObject.name = $"Processor {index}";
    }
}
