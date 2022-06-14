using UnityEngine;

public class LineController : MonoBehaviour
{
    public ProcessorController sourceProcessor, targetProcessor;
    public float timeToLive;
    public AnimationCurve alphaCurve;

    private LineRenderer lineRenderer;
    private Color color;

    private float spawnTime;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        float lifetime = (Time.time - spawnTime) / timeToLive;

        lineRenderer.material.SetColor("_BaseColor", color);
        lineRenderer.material.SetFloat("_Alpha", alphaCurve.Evaluate(lifetime) * 0.6f);

        if (lifetime > 1)
            gameObject.SetActive(false);
    }

    public void Setup(ProcessorController sourceProcessor, ProcessorController targetProcessor, float timeToLive)
    {
        this.sourceProcessor = sourceProcessor;
        this.targetProcessor = targetProcessor;

        lineRenderer.SetPositions(new Vector3[2]
        {
            RemapPosition(sourceProcessor.transform.position),
            RemapPosition(targetProcessor.transform.position)
        });
        color = sourceProcessor.Color;
        lineRenderer.material.SetColor("_BaseColor", color);
        lineRenderer.material.SetFloat("_Alpha", alphaCurve.Evaluate(0) * 0.6f);

        spawnTime = Time.time;
        this.timeToLive = timeToLive;
    }

    private Vector3 RemapPosition(Vector3 pos)
    {
        return new Vector3(pos.x, pos.z, 0);
    }
}
