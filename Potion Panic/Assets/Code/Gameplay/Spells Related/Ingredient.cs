using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    public string Name;

    private Transform target;

    // Start is called before the first frame update
    //void Start()
    //{
        
    //}

    // Update is called once per frame
    void Update()
    {
        if(target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, 5 * Time.deltaTime);

            if(Vector3.Distance(transform.position, target.position) <= 0.2f)
            {
                transform.position = target.position;
                target = null;

                if (transform.parent == null) // If not carried by player
                    GetComponent<Rigidbody>().useGravity = true;
            }
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        SetKeyElementsState(false);
    }

    public void SetKeyElementsState(bool state)
    {
        GetComponent<Rigidbody>().useGravity = state;
        GetComponent<BoxCollider>().enabled = state;
        GetComponent<SphereCollider>().enabled = state;
    }
}
