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

    private float controllerConnectionCheckTimer;
    private float controllerConnectionCheckDelay;

    private string controllerType;

    [Range(1, 4)]
    public int playerID;

    // Start is called before the first frame update
    void Start()
    {
        controllerConnectionCheckDelay = 3.0f;
        controllerConnectionCheckTimer = 0.0f;

        if (Input.GetJoystickNames().Length > 0)
        {
            if (Input.GetJoystickNames()[playerID - 1] == "Controller (Xbox One For Windows)")
                controllerType = "Xbox";
            else
                controllerType = "PS";
        }


        //Debug.Log(controllerType);

    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;

        Move();
        Aim();
        ControllerConnectionCheck();

        // To update for actions in range of interacting objects
        //if (inTriggerRange)
        //InTriggerRangeAction();
    }

    void Move()
    {
        float inputLeftX = Input.GetAxis("Horizontal Left " + playerID.ToString());
        float inputLeftY = Input.GetAxis("Vertical Left " + playerID.ToString());

        Vector3 dir = new Vector3(inputLeftX, 0, inputLeftY);

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

    //void AimWithMouse()
    //{
    //    Vector3 mousePos = Input.mousePosition;

    //    Ray camRay = Camera.main.ScreenPointToRay(mousePos); // Ray casted from screen post into the world
    //    RaycastHit hit;

    //    if (Physics.Raycast(camRay, out hit, Mathf.Infinity))
    //    {
    //        if (hit.transform.CompareTag("Ground")) // If hit anything ground, use impact point to look at
    //        {
    //            Vector3 aimTarget = new Vector3(hit.point.x, transform.position.y, hit.point.z);
    //            transform.LookAt(aimTarget);
    //        }
    //    }
    //}

    void Aim()
    {
        float inputRightX = 0.0f;
        float inputRightY = 0.0f;

        if (controllerType == "Xbox")
        {
            inputRightX = Input.GetAxisRaw("Horizontal Right Xbox " + playerID.ToString());
            inputRightY = Input.GetAxisRaw("Vertical Right Xbox " + playerID.ToString());
        }
        else if (controllerType == "PS")
        {
            inputRightX = Input.GetAxisRaw("Horizontal Right PS " + playerID.ToString());
            inputRightY = Input.GetAxisRaw("Vertical Right PS " + playerID.ToString());
        }

        Vector3 faceDir = new Vector3(inputRightX, 0, inputRightY);

        //transform.rotation = Quaternion.Euler(0, Mathf.Atan2(faceDir.y, faceDir.x) * Mathf.Rad2Deg, 0);

        if (faceDir.magnitude > 0)
        {
            Quaternion playerRotation = Quaternion.LookRotation(faceDir, Vector3.up);
            transform.rotation = playerRotation;
        }

    }

    void ControllerConnectionCheck()
    {
        if (controllerConnectionCheckTimer >= controllerConnectionCheckDelay)
        {
            if (Input.GetJoystickNames().Length > 0)
            {
                if (!(Input.GetJoystickNames()[playerID - 1] == ""))
                {
                    if (Input.GetJoystickNames()[playerID - 1] == "Controller (Xbox One For Windows)")
                        controllerType = "Xbox";
                    else
                        controllerType = "PS";
                }
            }

            controllerConnectionCheckTimer = 0.0f;
        }
        else
            controllerConnectionCheckTimer += Time.deltaTime;

    }

    void InTriggerRangeAction()
    {
        Ingredient collidingIngredient = collidingObject.GetComponent<Ingredient>();

        if (collidingIngredient != null)
        {
            //if (Input.GetKeyDown(KeyCode.E))
            if (Input.GetButtonDown("Interact " + playerID))
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
                //if (Input.GetKeyDown(KeyCode.E))
                if (Input.GetButtonDown("Interact " + playerID))
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
        if (collidingObject != null)
        {
            inTriggerRange = false;
            collidingObject = null;
        }

    }
}