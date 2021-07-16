using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    public Ingredient_SO ingredientInfo;

    private Transform target;

    public GameObject inputInfo;

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
                else
                    transform.parent.gameObject.GetComponent<PlayerController>().holdIngredient = true;
            }
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        SetKeyElementsState(false);
        SetInputInfoState(false);
    }

    public void SetKeyElementsState(bool state)
    {
        GetComponent<Rigidbody>().useGravity = state;
        GetComponent<BoxCollider>().enabled = state;
        GetComponent<SphereCollider>().enabled = state;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
            SetInputInfoState(true);

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            SetInputInfoState(false);
    }

    void SetInputInfoState(bool state)
    {
        inputInfo.SetActive(state);
    }
}
