using UnityEngine;

public class ProcessBlock : MonoBehaviour
{
    public Vector3 targetPosition;
    public float moveTime;
    public float deactivationRadius;
    private Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, moveTime);

        if ((transform.position - targetPosition).magnitude < deactivationRadius)
            gameObject.SetActive(false);
    }

    public void Setup(Vector3 position, Vector3 targetPosition, float moveTime)
    {
        transform.position = position;
        this.targetPosition = targetPosition;
        this.moveTime = moveTime;

        transform.LookAt(targetPosition, Vector3.up);
    }
}
