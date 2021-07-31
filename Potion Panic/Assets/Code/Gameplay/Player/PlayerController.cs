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

    private Vector3 previousPosition;
    private List<string> animStates = new List<string>();
    private Animator anim;

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

    // TESTING STUFF
    private bool hasSpell = true;
    private bool spellCasted;
    private bool isCasting;

    [Header("Controller")]
    // To frequently check if the controller is connected
    // or has been switched to a different type.
    private float controllerConnectionCheckTimer;
    private float controllerConnectionCheckDelay;
    private string controllerType;
    [Range(0, 1)]
    public float deadZone;

    [Range(1, 4)]
    public int playerID; // Player Num in Game

    // PUSH OTHER PLAYERS
    private LayerMask playerLayer;
    private bool isPushed;
    private Vector3 newPushLocation;
    private float pushDelay; // To prevent push spam
    private float pushDistance;

    // Start is called before the first frame update
    void Start()
    {
        health = 100.0f;

        controllerConnectionCheckDelay = 3.0f;
        controllerConnectionCheckTimer = controllerConnectionCheckDelay;

        ControllerConnectionCheck();

        blinkDuration = 2.0f;
        blinkDelay = 0.15f;

        anim = GetComponent<Animator>();

        animStates.Add("isMovingForward");
        animStates.Add("isMovingForwardRight");
        animStates.Add("isMovingForwardLeft");
        animStates.Add("isMovingBackward");
        animStates.Add("isMovingBackwardRight");
        animStates.Add("isMovingBackwardLeft");
        animStates.Add("isStrafingRight");
        animStates.Add("isStrafingLeft");

        myCauldron.OnSuccessEvent += MyCauldron_OnSuccessEvent;
        myCauldron.OnFailureEvent += MyCauldron_OnFailureEvent;

        playerLayer = 1 << 6;
        pushDelay = 2.0f;
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
        if (isPushed)
        {
            float dist = Vector3.Distance(transform.position, newPushLocation);

            transform.position = Vector3.MoveTowards(transform.position, newPushLocation,
                dist / pushDistance * 10 * Time.deltaTime);

            if (dist <= 0.2f)
            {
                isPushed = false;
                newPushLocation = Vector3.zero;
            }
        }
        else
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;

            if (isBlinking)
                BlinkPlayer();

            ControllerConnectionCheck();

            if (!spellCasted)
            {
                if(!isCasting)
                {
                    Move();
                    Animate();
                }

                CastORPush();
            }
            else
            {
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Area Magic"))
                {
                    if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9)
                    {
                        spellCasted = false;
                        anim.SetBool("castedAreaMagic", false);
                    }
                }
                else if(anim.GetCurrentAnimatorStateInfo(0).IsName("Projectile Magic"))
                {
                    if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9)
                    {
                        spellCasted = false;
                        isCasting = false;
                        anim.SetBool("castedProjectileMagic", false);
                    }
                }
            }

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
        previousPosition = transform.position;

        float inputLeftX = Input.GetAxisRaw("Horizontal Left " + playerID.ToString());
        float inputLeftY = Input.GetAxisRaw("Vertical Left " + playerID.ToString());

        Vector3 dir = new Vector3(inputLeftX, 0, inputLeftY);

        float magnitude = dir.magnitude;

        if (magnitude > deadZone)
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

        Vector3 faceDir = (transform.position - previousPosition).normalized;

        if (faceDir.magnitude > 0.01f)
        {
            Quaternion playerRotation = Quaternion.LookRotation(faceDir, Vector3.up);
            transform.rotation = playerRotation;
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

        if (faceDir.magnitude > deadZone)
        {
            Quaternion playerRotation = Quaternion.LookRotation(faceDir, Vector3.up);
            transform.rotation = playerRotation;
        }
    }

    void CastORPush()
    {
        if (hasSpell) // CAST
        {
            float val = 2; // WILL BE ENUM FROM SPELL INFO
            
            if(val == 1)
            {
                if (Input.GetButtonDown("Cast " + playerID.ToString()))
                {
                    ResetAnimationToIdle();
                    anim.SetBool("castedAreaMagic", true);
                    spellCasted = true;
                }
            }
            else
            {
                if(Input.GetButton("Cast " + playerID.ToString()))
                {
                    isCasting = true;
                    ResetAnimationToIdle();
                    Aim();
                }

                if(Input.GetButtonUp("Cast " + playerID.ToString()))
                {
                    anim.SetBool("castedProjectileMagic", true);
                    spellCasted = true;
                }

            }
        }
        else // PUSH
        {
            if (pushDelay <= 0)
            {
                if (Input.GetButtonDown("Cast " + playerID.ToString()))
                {
                    RaycastHit hit;
                    float j = -0.25f;

                    for (int i = 0; i < 3; i++) // To broaden the Raycast check
                    {
                        if (Physics.Raycast(transform.position + (transform.right * j) + (transform.up),
                            transform.forward, out hit, 1.5f, playerLayer))
                        {
                            Vector3 dir = hit.collider.gameObject.transform.position - transform.position;
                            dir = new Vector3(dir.x, 0.0f, dir.z); // Getting a straight line

                            hit.collider.gameObject.GetComponent<PlayerController>().PushPlayer(dir.normalized, 2.0f);
                            //hit.collider.gameObject.GetComponent<Rigidbody>().AddForce(dir.normalized * 10.0f, ForceMode.Impulse);

                            pushDelay = 2.0f;
                            break;
                        }

                        j += 0.25f;
                    }
                }
            }
            else
                pushDelay -= Time.deltaTime;
        }

    }

    void Animate()
    {
        if (transform.position != previousPosition)
        {
            SetAnimationActive("isMovingForward");

            //Vector3 val = transform.position - previousPosition;

            //Vector3 moveVector = Vector3.Normalize(transform.position - previousPosition);
            //float angle = Vector3.Angle(transform.forward, moveVector);

            //// RUNNING FORWARD
            //if (angle < 30.0f)


            //// RUNNING FORWARD DIAGONAL
            //else if (angle >= 30 && angle < 60)
            //{
            //    float sidewaysAngle = Vector3.Angle(transform.right, moveVector);

            //    // RUN FORWARD RIGHT
            //    if (sidewaysAngle < 90.0f)
            //        SetAnimationActive("isMovingForwardRight");
            //    else
            //        SetAnimationActive("isMovingForwardLeft");
            //}
            //// STRAFING
            //else if (angle >= 60 && angle <= 120)
            //{
            //    float sidewaysAngle = Vector3.Angle(transform.right, moveVector);

            //    // STRAFING RIGHT
            //    if (sidewaysAngle < 90.0f)
            //        SetAnimationActive("isStrafingRight");
            //    // STRAFING LEFT
            //    else
            //        SetAnimationActive("isStrafingLeft");
            //}
            //// RUNNING BACKWARD DIAGONAL
            //else if (angle > 120 && angle <= 150)
            //{
            //    float sidewaysAngle = Vector3.Angle(transform.right, moveVector);

            //    // RUNNING BACKWARD 
            //    if (sidewaysAngle < 90.0f)
            //        SetAnimationActive("isMovingBackwardRight");
            //    else
            //        SetAnimationActive("isMovingBackwardLeft");
            //}
            //// RUNNING BACKWARD
            //else if (angle > 150)
            //    SetAnimationActive("isMovingBackward");
        }
        else
            ResetAnimationToIdle();
    }

    void SetAnimationActive(string stateName)
    {
        for (int i = 0; i < animStates.Count; i++)
        {
            if (animStates[i] == stateName)
                anim.SetBool(animStates[i], true);
            else
                anim.SetBool(animStates[i], false);
        }
    }

    void ResetAnimationToIdle()
    {
        for (int i = 0; i < animStates.Count; i++)
            anim.SetBool(animStates[i], false);
    }

    void ControllerConnectionCheck()
    {
        if (controllerConnectionCheckTimer >= controllerConnectionCheckDelay)
        {
            if (Input.GetJoystickNames().Length > playerID - 1)
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

    public void PushPlayer(Vector3 dir, float pushDist)
    {
        isPushed = true;
        newPushLocation = transform.position + (dir * pushDist);
        pushDistance = pushDist;
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