using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    public Ingredient_SO ingredientInfo;

    private Transform target;

    public GameObject inputInfo;

    public BoxCollider boxCollider;
    private LayerMask groundLayer;

    // Start is called before the first frame update
    void Start()
    {
        groundLayer = 1 << 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, 5 * Time.deltaTime);

            if (Vector3.Distance(transform.position, target.position) <= 0.2f)
            {
                transform.position = target.position;
                target = null;

                if (transform.parent == null) // If not carried by player
                {
                    GetComponent<Rigidbody>().useGravity = true;
                    boxCollider.enabled = false;
                }
                else
                    transform.parent.gameObject.GetComponent<PlayerController>().holdIngredient = true;
            }
        }
        else
        {
            if (boxCollider.enabled) // A hack to know that it's not going to cauldron
            {
                if (Physics.Raycast(boxCollider.gameObject.transform.position, -Vector3.up, 0.5f, groundLayer))
                    GetComponent<SphereCollider>().enabled = true;
                else
                    GetComponent<SphereCollider>().enabled = false;
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
        GetComponent<SphereCollider>().enabled = state;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject.GetComponent<PlayerController>().carryingIngredient == null &&
                !other.gameObject.GetComponent<PlayerController>().hasSpell)
            {
                SetInputInfoState(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SetInputInfoState(false);
        }
    }

    public void SetInputInfoState(bool state)
    {
        inputInfo.SetActive(state);
    }
}
