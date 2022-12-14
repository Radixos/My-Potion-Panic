using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using FMODUnity;

public class PlayerController : MonoBehaviour
{
    [Header("-Core-")]
    public float speed;
    private float speedMultiplier;
    public float health;
    public const float maxHealth = 100.0f;
    public bool isDead;
    public float kills;

    private Vector3 previousPosition;
    private List<string> animStates = new List<string>();
    private Animator anim;

    [Header("-Blink-")]
    public bool isBlinking; // Blinking to indicate i-frames upon respawn
    private float blinkDuration; // Time the player will stay invulnerable
    private float blinkDelay; // Time interval to switch render state
    public GameObject playerSkin; // Model to blink

    [Header("-Interactables-")]
    public Transform carryingLocation; // Location where the picked up ingredient will be placed
    public Ingredient carryingIngredient; // Ingredient that the player is carrying
    public bool holdIngredient; // Boolean to check whether the player is carrying an ingredient or not
    public Cauldron myCauldron;

    private bool inTriggerRange; // In range of a trigger object. OnTriggerStay works only for a couple of frames
    private GameObject collidingObject; // Ingredient that the player is in range with

    [Header("-Controller-")]
    // To frequently check if the controller is connected
    // or has been switched to a different type.
    [Range(0, 1)]
    public float deadZone;
    private float controllerConnectionCheckTimer;
    private float controllerConnectionCheckDelay;
    private string controllerType;

    [Range(1, 4)]
    public int playerID; // Player Num in Game

    // PUSH OTHER PLAYERS
    private LayerMask playerLayer;
    private bool isPushed;
    private Vector3 newPushLocation;
    private float pushDelay; // To prevent push spam
    private float pushDistance;

    // SPELL CASTING
    [Header("-Spell Info-")]
    public Spell_SO spellInfo;
    public bool hasSpell;
    public int spellUses;

    private bool inSpellAnim;
    private bool isCasting;
    private bool spellCasted;
    private ObjectPool spellPool;

    // UI
    [Header("-UI-")]
    public Transform aimArrow;

    private PlayerManager playerManager;

    //FMOD Events
    [Header("-SFX-")]
    public EventReference iceMinePath;
    public EventReference volcanicBlastPath, natureArrowPath, pickupPath;

    // Start is called before the first frame update
    void Start()
    {
        health = 100.0f;
        speedMultiplier = 1.0f;

        controllerConnectionCheckDelay = 3.0f;
        controllerConnectionCheckTimer = controllerConnectionCheckDelay;

        blinkDuration = 2.0f;
        blinkDelay = 0.15f;

        anim = GetComponent<Animator>();

        animStates.Add("isMovingForward");
        animStates.Add("castedAreaMagic");
        animStates.Add("castedProjectileMagic");

        myCauldron.OnSuccessEvent += MyCauldron_OnSuccessEvent;
        myCauldron.OnFailureEvent += MyCauldron_OnFailureEvent;

        playerLayer = 1 << 6;
        pushDelay = 2.0f;

        StartCoroutine(ControllerCheck());

        //hasSpell = true;
        //spellPool = GameObject.Find("Player " + playerID.ToString() + " Ice Mine Pool").GetComponent<ObjectPool>();
        //spellUses = 100;

        playerManager = FindObjectOfType<PlayerManager>();
    }

    private void MyCauldron_OnSuccessEvent(Spell_SO brewedSpell)
    {
        spellInfo = brewedSpell;
        spellUses = brewedSpell.NumberOfUses;
        spellPool = GameObject.Find("Player " + playerID.ToString() +
            " " + brewedSpell.spellPrefab.name).GetComponent<ObjectPool>();

        hasSpell = true;
    }

    private void MyCauldron_OnFailureEvent()
    {
        health = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPushed)
        {
            float dist = Vector3.Distance(transform.position, newPushLocation);
            Vector3 dir = (newPushLocation - transform.position).normalized;

            Vector3 newPosition = transform.position + dir.normalized * dist / pushDistance * 10 * Time.deltaTime;
            NavMeshHit hit;
            bool isValid = NavMesh.SamplePosition(newPosition, out hit, 0.3f, NavMesh.AllAreas);

            // This check is so that the player does not clip into static objects
            if (isValid)
            {
                transform.position = Vector3.MoveTowards(transform.position, newPushLocation, dist / pushDistance * 10 * Time.deltaTime);

                if (dist <= 0.2f)
                {
                    isPushed = false;
                    newPushLocation = Vector3.zero;
                }
            }
            else
            {
                isPushed = false;
                newPushLocation = Vector3.zero;
            }

        }
        else
        {
            // Cancel any external forces acting on the Player
            GetComponent<Rigidbody>().velocity = Vector3.zero;

            // Invulnerability
            if (isBlinking)
                BlinkPlayer();

            // If not in a spell animation, has controller connected and if the match isn't over, then proceed
            if (!inSpellAnim && !(controllerType == "") && !playerManager.matchCompleted)
            {
                // This is a specific block when using projectile type spells
                if (!isCasting)
                {
                    Move();
                    Animate();
                }

                CastORPush();

                // DROP INGREDIENT ON THE GROUND
                if (carryingIngredient != null && !inTriggerRange && holdIngredient)
                {
                    if (Input.GetButtonDown("Interact " + controllerType + " " + playerID.ToString()))
                    {
                        carryingIngredient.transform.parent = null;
                        carryingIngredient.gameObject.GetComponent<Rigidbody>().useGravity = true;
                        holdIngredient = false;
                        carryingIngredient = null;
                    }

                }

                // To update for actions in range of interacting objects
                if (inTriggerRange)
                    InTriggerRangeAction();
            }
            else
            {
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("Area Magic"))
                {
                    // When the animation is about to end
                    if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9)
                    {
                        inSpellAnim = false;
                        anim.SetBool("castedAreaMagic", false);

                        if (spellUses <= 0)
                        {
                            hasSpell = false;
                            spellInfo = null;
                        }
                    }
                    // when the animation is halfway through and execute this one time
                    else if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5 && !spellCasted)
                    {
                        // Using a pooled spell object
                        GameObject obj = spellPool.GetPooledObject();
                        obj.transform.position = transform.position + transform.up;
                        obj.SetActive(true);

                        // Setting the caster as this to increment kills
                        obj.GetComponent<SpellBehaviour>().caster = this;
                        spellCasted = true;
                    }
                }
                else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Projectile Magic"))
                {
                    if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9)
                    {
                        inSpellAnim = false;
                        isCasting = false;
                        anim.SetBool("castedProjectileMagic", false);

                        if (spellUses <= 0)
                        {
                            hasSpell = false;
                            spellInfo = null;
                        }
                    } 
                    else if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7 && !spellCasted)
                    {
                        RuntimeManager.PlayOneShot(natureArrowPath, transform.position);

                        GameObject obj = spellPool.GetPooledObject();
                        obj.transform.position = transform.position + transform.up;
                        obj.transform.rotation = transform.rotation;
                        obj.SetActive(true);

                        obj.GetComponent<SpellBehaviour>().caster = this;
                        obj.GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);
                        spellCasted = true;
                    }
                }
            }
        }
    }

    void BlinkPlayer()
    {
        if (blinkDuration > 0)
        {
            // Oscillate between the active states of the mesh
            // to indicate invulnerability
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
        // For resetting animation purposes
        previousPosition = transform.position;

        float inputLeftX = Input.GetAxisRaw("Horizontal Left " + playerID.ToString());
        float inputLeftY = Input.GetAxisRaw("Vertical Left " + playerID.ToString());

        Vector3 dir = new Vector3(inputLeftX, 0, inputLeftY);

        float magnitude = dir.magnitude;

        if (magnitude > deadZone)
        {
            // If the new location is within the NavMesh area, then move player
            Vector3 newPosition = transform.position + dir.normalized * Time.deltaTime * speed * speedMultiplier;
            NavMeshHit hit;
            bool isValid = NavMesh.SamplePosition(newPosition, out hit, 0.3f, NavMesh.AllAreas);

            // To avoid constant flicker with hard colliders
            RaycastHit rayHit;
            LayerMask wallLayer = 1 << 0;
            bool isFreeSpace = !Physics.Raycast(transform.position + (0.25f * Vector3.up),
                dir.normalized, out rayHit, 0.5f, wallLayer, QueryTriggerInteraction.Ignore);

            if (isValid && isFreeSpace)
            {
                if ((transform.position - hit.position).magnitude >= 0.02f)
                    transform.position = newPosition;
            }
        }

        Vector3 faceDir = (transform.position - previousPosition).normalized;

        if (faceDir.magnitude > 0.01f)
        {
            // Making the player rotate face forward in the direction of movement
            // Also rotate towards the new direction to make it look smoother
            Quaternion lookRotation = Quaternion.LookRotation(faceDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 1800 * Time.deltaTime);
        }

        if (holdIngredient && carryingIngredient != null) // Waiting to carry until the pickup "animation" is finished
        {
            carryingIngredient.transform.position = carryingLocation.position;
            carryingIngredient.transform.rotation = Quaternion.Euler(carryingIngredient.transform.rotation.x,
                0, carryingIngredient.transform.rotation.z);

            // Limiting speed to favour spell casters
            speedMultiplier = 0.75f;
        }
        else
            speedMultiplier = 1.0f;

    }

    void Aim()
    {
        float inputRightX = Input.GetAxisRaw("Horizontal Right " + controllerType + " " + playerID.ToString());
        float inputRightY = Input.GetAxisRaw("Vertical Right " + controllerType + " " + playerID.ToString());

        Vector3 faceDir = new Vector3(inputRightX, 0, inputRightY);

        // Like movement, making aim also rotate towards the intended direction to make it smoother
        if (faceDir.magnitude > deadZone)
        {
            Quaternion lookRotation = Quaternion.LookRotation(faceDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 1800 * Time.deltaTime);
        }
    }

    void CastORPush()
    {
        if (hasSpell) // CAST
        {
            SpellType type = spellInfo.spellType; //SpellType.AREA; // 

            if (type == SpellType.AREA)
            {
                if (Input.GetButtonDown("Cast " + playerID.ToString()))
                {
                    ResetAnimationToIdle();
                    SetAnimationActive("castedAreaMagic");

                    switch (spellInfo.Name)
                    {
                        case "Volcanic Blast":
                            RuntimeManager.PlayOneShot(volcanicBlastPath, transform.position);
                            break;
                        case "Ice Mine":
                            RuntimeManager.PlayOneShot(iceMinePath, transform.position);
                            break;
                    }

                    inSpellAnim = true;
                    spellCasted = false;

                    spellUses -= 1;
                }
            }
            else if (type == SpellType.PROJECTILE)
            {
                // Enter aiming as long as the button is held
                if (Input.GetButton("Cast " + playerID.ToString()))
                {
                    aimArrow.gameObject.SetActive(true);
                    isCasting = true;
                    ResetAnimationToIdle();
                    Aim();
                    aimArrow.rotation = transform.rotation;
                }

                // Cast the spell when the button is released
                if (Input.GetButtonUp("Cast " + playerID.ToString()))
                {
                    SetAnimationActive("castedProjectileMagic");
                    RuntimeManager.PlayOneShot(natureArrowPath, transform.position);
                    inSpellAnim = true;
                    spellCasted = false;
                    aimArrow.gameObject.SetActive(false);

                    --spellUses;

                    Debug.Log("Uses Left = " + spellUses.ToString());
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
                        if (Physics.Raycast(transform.position + (transform.right * j) + transform.up,
                            transform.forward, out hit, 1.5f, playerLayer))
                        {
                            Vector3 dir = hit.collider.gameObject.transform.position - transform.position;
                            dir = new Vector3(dir.x, 0.0f, dir.z); // Getting a straight line

                            hit.collider.gameObject.GetComponent<PlayerController>().PushPlayer(dir.normalized, 2.0f);

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
            SetAnimationActive("isMovingForward");
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

    void InTriggerRangeAction()
    {
        Ingredient collidingIngredient = collidingObject.GetComponent<Ingredient>();

        if (collidingIngredient != null)
        {
            if (Input.GetButtonDown("Interact " + controllerType + " " + playerID.ToString()))
            {
                RuntimeManager.PlayOneShot(pickupPath, transform.position);

                carryingIngredient = collidingIngredient;
                carryingIngredient.transform.parent = transform;
                carryingIngredient.SetTarget(carryingLocation);

                inTriggerRange = false;
                collidingIngredient.SetInputInfoState(false);

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
        ResetAnimationToIdle();
    }

    public void PlayerReset()
    {
        if (carryingIngredient != null)
        {
            carryingIngredient.transform.parent = null;
            carryingIngredient.SetKeyElementsState(true);
            holdIngredient = false;
            carryingIngredient = null;
            collidingObject = null;
        }

        isPushed = false;
        newPushLocation = Vector3.zero;
        pushDelay = 2.0f;
        pushDistance = 0.0f;

        inSpellAnim = false;
        isCasting = false;
        spellCasted = false;

        ResetAnimationToIdle();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasSpell) // Can only interact with ingredients and cauldron if player doesn't have a spell
        {
            Ingredient collidingIngredient = other.gameObject.GetComponent<Ingredient>();

            if ((collidingIngredient != null && carryingIngredient == null) ||
                other.gameObject.GetInstanceID() == myCauldron.gameObject.GetInstanceID())
            {
                inTriggerRange = true;
                collidingObject = other.gameObject;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (collidingObject != null)
        {
            if (other.gameObject == collidingObject)
            {
                inTriggerRange = false;
                collidingObject = null;
            }
        }
    }

    IEnumerator ControllerCheck()
    {
        while (true)
        {
            int controllerNum = 0;

            for (int i = 0; i < Input.GetJoystickNames().Length; i++)
            {
                if (!(Input.GetJoystickNames()[i] == ""))
                {
                    ++controllerNum;

                    // Cofirming that the controller in order matches with the player number
                    if (controllerNum == playerID)
                    {
                        if (Input.GetJoystickNames()[i].ToLower().Contains("xbox"))
                            controllerType = "Xbox";
                        else
                            controllerType = "PS";

                        break;
                    }

                }
                else
                    controllerType = ""; // If it gets disconnected in game

            }

            yield return new WaitForSeconds(2);
        }
    }
}