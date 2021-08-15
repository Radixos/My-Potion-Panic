using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject poolingObject;
    public int poolingAmount;

    private List<GameObject> objectPool = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < poolingAmount; i++)
        {
            GameObject obj = Instantiate(poolingObject);
            obj.SetActive(false);
            objectPool.Add(obj);
        }
    }

    // Update is called once per frame
    //void Update()
    //{

    //}

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < objectPool.Count; i++)
        {
            if (!objectPool[i].activeSelf)
                return objectPool[i];
        }

        GameObject obj = Instantiate(poolingObject);
        objectPool.Add(obj);
        return obj;
    }

}