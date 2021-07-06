using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public float speed;

    public Transform carryingLocation;
    private Ingredient carryingIngredient;

    // Start is called before the first frame update
    //void Start()
    //{

    //}

    // Update is called once per frame
    void Update()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;

        Move();
        //CarryIngredient();
    }

    void Move()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(inputX, 0, inputZ);

        float magnitude = dir.sqrMagnitude;

        if (magnitude >= 0.1f)
        {
            Vector3 newPosition = transform.position + dir.normalized * Time.deltaTime * speed;
            NavMeshHit hit;
            bool isValid = NavMesh.SamplePosition(newPosition, out hit, 0.3f, NavMesh.AllAreas);

            if (isValid)
            {
                if ((transform.position - hit.position).magnitude >= 0.02f)
                {
                    transform.position = newPosition;
                    Debug.Log("Move OK");
                }

            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Ingredient collidingIngredient = other.gameObject.GetComponent<Ingredient>();

        if(collidingIngredient != null)
        {
            carryingIngredient = collidingIngredient;
            carryingIngredient.transform.parent = transform;
            carryingIngredient.SetTarget(carryingLocation);
            //carryingIngredient.gameObject.GetComponent<Rigidbody>().useGravity = false;
            //carryingIngredient.gameObject.GetComponent<SphereCollider>().enabled = false; // Trigger
            //carryingIngredient.gameObject.GetComponent<BoxCollider>().enabled = false; // Normal

        }
        else
        {
            Cauldron collidingCauldron = other.gameObject.GetComponent<Cauldron>();

            if (collidingCauldron != null && carryingIngredient != null)
            {
                collidingCauldron.AssignDroppingIngredient(carryingIngredient);

                carryingIngredient.SetTarget(collidingCauldron.dropLocation);
                carryingIngredient.transform.parent = null;
                carryingIngredient = null;
            }
        }

    }
}
