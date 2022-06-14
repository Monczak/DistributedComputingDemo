using UnityEngine;

[RequireComponent(typeof(ObjectPooler))]
public class LineManager : MonoBehaviour
{
    private ObjectPooler pooler;
    public int lineCount;

    private void Awake()
    {
        pooler = GetComponent<ObjectPooler>();
        pooler.objectCount = lineCount;
        pooler.SetupObjects();
    }

    public void SpawnLine(ProcessorController sourceProcessor, ProcessorController targetProcessor, float timeToLive)
    {
        LineController line = pooler.Request<LineController>();
        if (line != null)
            line.Setup(sourceProcessor, targetProcessor, timeToLive);
    }
}