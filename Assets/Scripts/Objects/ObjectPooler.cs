using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public GameObject prefab;
    private List<GameObject> objects;

    public int objectCount;

    public void SetupObjects()
    {
        objects = new List<GameObject>();
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < objectCount; i++)
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            objects.Add(obj);
        }
    }

    public GameObject RequestObject()
    {
        foreach (GameObject obj in objects)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }
        return null;
    }

    public T Request<T>() where T : MonoBehaviour
    {
        GameObject obj = RequestObject();
        if (obj == null)
            return null;
        return obj.GetComponent<T>();
    }
}
