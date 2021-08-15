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

    private bool onGround;
    private bool playerInRange;

    [SerializeField] private float destroyDelay;

    // Start is called before the first frame update
    void Start()
    {
        destroyDelay = GameObject.Find("Fountain_1_Low").GetComponent<FountainBehaviour>().destroyDelay;    //"" MUST contain the name of object to which "FountainBehaviour" script is attached!!!
        groundLayer = 1 << 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            CancelInvoke("DestroyIngredient");
            playerInRange = false;

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
                onGround = Physics.Raycast(boxCollider.gameObject.transform.position, -Vector3.up, 0.5f, groundLayer, QueryTriggerInteraction.Ignore);

                if (onGround)
                {
                    GetComponent<SphereCollider>().enabled = true;

                    if (!playerInRange)
                        Invoke("DestroyIngredient", destroyDelay);
                    else
                        CancelInvoke("DestroyIngredient");
                }
                else
                {
                    GetComponent<SphereCollider>().enabled = false;
                    CancelInvoke("DestroyIngredient");
                }
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
                playerInRange = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SetInputInfoState(false);
            playerInRange = false;
        }
    }

    public void SetInputInfoState(bool state)
    {
        inputInfo.SetActive(state);
    }

    private void DestroyIngredient()
    {
        Destroy(gameObject);
    }
}
