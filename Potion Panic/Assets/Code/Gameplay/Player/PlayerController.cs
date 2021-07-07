using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public float speed;

    private bool inTriggerRange;
    private GameObject collidingObject; // Ingredient that the player is in range with

    public Transform carryingLocation;
    public Ingredient carryingIngredient; // Ingredient that the player is carrying

    public Cauldron myCauldron;

    // Start is called before the first frame update
    //void Start()
    //{

    //}

    // Update is called once per frame
    void Update()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;

        Move();
        AimWithMouse();

        if (inTriggerRange)
            InTriggerRangeAction();
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
                    transform.position = newPosition;
            }
        }
    }

    void AimWithMouse()
    {
        Vector3 mousePos = Input.mousePosition;

        Ray camRay = Camera.main.ScreenPointToRay(mousePos); // Ray casted from screen post into the world
        RaycastHit hit;

        if (Physics.Raycast(camRay, out hit, Mathf.Infinity))
        {
            if (hit.transform.CompareTag("Ground")) // If hit anything ground, use impact point to look at
            {
                Vector3 aimTarget = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                transform.LookAt(aimTarget);
            }
        }
    }

    void InTriggerRangeAction()
    {
        Ingredient collidingIngredient = collidingObject.GetComponent<Ingredient>();

        if (collidingIngredient != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                carryingIngredient = collidingIngredient;
                carryingIngredient.transform.parent = transform;
                carryingIngredient.SetTarget(carryingLocation);
            }

        }
        else
        {
            Cauldron collidingCauldron = collidingObject.GetComponent<Cauldron>();

            if (collidingCauldron == myCauldron && carryingIngredient != null)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    collidingCauldron.AssignDroppingIngredient(carryingIngredient);

                    carryingIngredient.SetTarget(collidingCauldron.dropLocation);
                    carryingIngredient.transform.parent = null;
                    carryingIngredient = null;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Ingredient collidingIngredient = other.gameObject.GetComponent<Ingredient>();

        if (collidingIngredient != null || other.gameObject.GetInstanceID() == myCauldron.gameObject.GetInstanceID())
        {
            inTriggerRange = true;
            collidingObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(collidingObject != null)
        {
            inTriggerRange = false;
            collidingObject = null;
        }

    }
}