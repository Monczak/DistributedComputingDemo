using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ObjectPooler))]
public class ProcessBlockManager : MonoBehaviour
{
    public int processBlockCount;

    public float blockSendInterval;
    public float blockMoveTime;

    private ObjectPooler pooler;

    private void Awake()
    {
        pooler = GetComponent<ObjectPooler>();
        pooler.objectCount = processBlockCount;
        pooler.SetupObjects();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SendProcessBlock(ProcessorController sourceProcessor, ProcessorController targetProcessor)
    {
        ProcessBlock block = pooler.Request<ProcessBlock>();
        if (block == null) return;
        block.Setup(sourceProcessor.transform.position, targetProcessor.transform.position, blockMoveTime);
    }

    public void SendProcessBlocks(ProcessorController sourceProcessor, ProcessorController targetProcessor, int count)
    {
        IEnumerator Send()
        {
            for (int i = 0; i < count; i++)
            {
                SendProcessBlock(sourceProcessor, targetProcessor);
                yield return new WaitForSeconds(blockSendInterval);
            }
        }
        StartCoroutine(Send());
    }
}
