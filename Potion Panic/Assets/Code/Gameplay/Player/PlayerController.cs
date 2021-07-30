using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [Header("Core")]
    public float speed;
    public float health;
    public const float maxHealth = 100.0f;
    public bool isDead;
    public float kills;

    [Header("Blink")]
    public bool isBlinking; // Blinking to indicate i-frames upon respawn
    private float blinkDuration; // Time the player will stay invulnerable
    private float blinkDelay; // Time interval to switch render state
    public GameObject playerSkin; // Model to blink

    [Header("Spell Related")]
    private bool inTriggerRange; // In range of a trigger object. OnTriggerStay works only for a couple of frames
    private GameObject collidingObject; // Ingredient that the player is in range with

    public Transform carryingLocation; // Location where the picked up ingredient will be placed
    public Ingredient carryingIngredient; // Ingredient that the player is carrying
    public bool holdIngredient; // Boolean to check whether the player is carrying an ingredient or not

    public Cauldron myCauldron;

    [Header("Controller")]
    // To frequently check if the controller is connected
    // or has been switched to a different type.
    private float controllerConnectionCheckTimer;
    private float controllerConnectionCheckDelay;
    private string controllerType;

    [Range(1, 4)]
    public int playerID; // Player Num in Game

    // Start is called before the first frame update
    void Start()
    {

        health = 100.0f;

        controllerConnectionCheckDelay = 3.0f;
        controllerConnectionCheckTimer = controllerConnectionCheckDelay;

        ControllerConnectionCheck();

        blinkDuration = 2.0f;
        blinkDelay = 0.15f;

        myCauldron.OnSuccessEvent += MyCauldron_OnSuccessEvent;
        myCauldron.OnFailureEvent += MyCauldron_OnFailureEvent;
    }

    private void MyCauldron_OnSuccessEvent(Spell_SO brewedSpell)
    {
        Debug.Log("Success! Brewed " + brewedSpell.Name);
    }

    private void MyCauldron_OnFailureEvent()
    {
        Debug.Log("Failure! Player " + playerID.ToString() + " died!");
        health = 0;
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;

        if (isBlinking)
            BlinkPlayer();

        ControllerConnectionCheck();

        Move();
        Aim();

        if (health <= 0)
        {
            // Post Death Functions
            if (carryingIngredient != null)
            {
                carryingIngredient.transform.parent = null;
                carryingIngredient.SetKeyElementsState(true);
                holdIngredient = false;
                carryingIngredient = null;
            }
        }

        // To update for actions in range of interacting objects
        if (inTriggerRange)
            InTriggerRangeAction();
    }

    void BlinkPlayer()
    {
        if (blinkDuration > 0)
        {
            if (blinkDelay <= 0)
            {
                // Subjected to change once final models are in
                playerSkin.gameObject.SetActive(!playerSkin.gameObject.activeSelf);
                blinkDelay = 0.15f;
            }
            else
                blinkDelay -= Time.deltaTime;

            blinkDuration -= Time.deltaTime;
        }
        else
        {
            blinkDelay = 0.15f;
            blinkDuration = 3.0f;
            playerSkin.gameObject.SetActive(true);
            isBlinking = false;
        }
    }

    void Move()
    {
        float inputLeftX = Input.GetAxisRaw("Horizontal Left " + playerID.ToString());
        float inputLeftY = Input.GetAxisRaw("Vertical Left " + playerID.ToString());

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

        if (holdIngredient && carryingIngredient != null) // Waiting to carry until the pickup "animation" is finished
            carryingIngredient.transform.position = carryingLocation.position;
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
        float inputRightX = Input.GetAxisRaw("Horizontal Right " + controllerType + " " + playerID.ToString());
        float inputRightY = Input.GetAxisRaw("Vertical Right " + controllerType + " " + playerID.ToString());

        Vector3 faceDir = new Vector3(inputRightX, 0, inputRightY);

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
            if (Input.GetButtonDown("Interact " + controllerType + " " + playerID.ToString()))
            {
                carryingIngredient = collidingIngredient;
                carryingIngredient.transform.parent = transform;
                carryingIngredient.SetTarget(carryingLocation);

                inTriggerRange = false;
                collidingObject = null;
            }
        }
        else
        {
            Cauldron collidingCauldron = collidingObject.GetComponent<Cauldron>();

            if (collidingCauldron == myCauldron && carryingIngredient != null)
            {
                if (Input.GetButtonDown("Interact " + controllerType + " " + playerID.ToString()))
                {
                    collidingCauldron.AssignDroppingIngredient(carryingIngredient);

                    carryingIngredient.SetTarget(collidingCauldron.dropLocation);
                    carryingIngredient.transform.parent = null;
                    holdIngredient = false;
                    carryingIngredient = null;

                    inTriggerRange = false;
                    collidingObject = null;
                }
            }
        }
    }

    public void Respawn()
    {
        health = maxHealth;
        isDead = false;
        isBlinking = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Ingredient collidingIngredient = other.gameObject.GetComponent<Ingredient>();

        if ((collidingIngredient != null && carryingIngredient == null) ||
            other.gameObject.GetInstanceID() == myCauldron.gameObject.GetInstanceID())
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