using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cauldron : MonoBehaviour
{
    public Transform dropLocation;

    private Ingredient droppingIngredient;
    private bool ingredientDropped;

    public Transform cauldronCore;

    public PlayerController cauldronOwner;

    public GameObject inputInfo;

    // Variables for post consumption of ingredients
    private bool ingredientLimitReached;
    private bool spellBrewed;
    private float brewDelay = 0.25f;

    public List<Ingredient> consumedIngredients = new List<Ingredient>();
    public List<Spell_SO> spellPool; // Should be Spell Prefab

    // EVENTS
    public delegate void OnSuccess(Spell_SO brewedSpell);
    public event OnSuccess OnSuccessEvent;

    public delegate void OnFailure();
    public event OnFailure OnFailureEvent;

    // UI
    public List<GameObject> ingredientIconMasks; // Circle Mask
    public List<GameObject> ingredientIcons; // Actual Image

    // Start is called before the first frame update
    //void Start()
    //{
    //}

    // Update is called once per frame
    void Update()
    {
        if (droppingIngredient != null)
            ConsumeIngredient();
        else if (ingredientLimitReached)
        {
            brewDelay -= Time.deltaTime;

            if(brewDelay <= 0)
            {
                BrewSpell();
                brewDelay = 0.25f;
            }
        }


        ManageUI();
    }

    void ConsumeIngredient()
    {
        if (Vector3.Distance(droppingIngredient.transform.position, cauldronCore.position) <= 0.3f)
        {
            consumedIngredients.Add(droppingIngredient);

            if (consumedIngredients.Count >= 3)
                ingredientLimitReached = true;

            //droppingIngredient.gameObject.SetActive(false); // Set to false if ingredients are object pooled
            Destroy(droppingIngredient.gameObject);
            droppingIngredient = null;

        }
    }

    void BrewSpell()
    {
        List<Ingredient> correctIngredients = new List<Ingredient>();

        for (int i = 0; i < spellPool.Count; i++) // Looping through Spells
        {
            List<Ingredient_SO> updatedRequiredIngredients = new List<Ingredient_SO>();

            for (int k = 0; k < spellPool[i].requiredIngredients.Count; k++)
                updatedRequiredIngredients.Add(spellPool[i].requiredIngredients[k]);

            for (int j = 0; j < consumedIngredients.Count; j++) // Looping through every ingredient of a spell
            {
                if (updatedRequiredIngredients.Contains(consumedIngredients[j].ingredientInfo))
                {
                    correctIngredients.Add(consumedIngredients[j]);
                    updatedRequiredIngredients.Remove(consumedIngredients[j].ingredientInfo);

                    if (correctIngredients.Count >= 3)
                    {
                        OnSuccessEvent?.Invoke(spellPool[i]);

                        spellBrewed = true;
                        break;
                    }
                }
            }

            correctIngredients.Clear();

            if (spellBrewed)
                break;
        }

        if (!spellBrewed)
        {
            OnFailureEvent?.Invoke();
        }

        // Reset Cauldron
        ingredientLimitReached = false;
        spellBrewed = false;
        consumedIngredients.Clear();
    }

    void ManageUI()
    {
        if (consumedIngredients.Count > 0)
        {
            for (int i = 0; i < consumedIngredients.Count; i++)
            {
                ingredientIconMasks[i].gameObject.SetActive(true);
                ingredientIcons[i].GetComponent<Image>().sprite = consumedIngredients[i].ingredientInfo.ingredientPreview;
            }
        }
        else
            for (int i = 0; i < ingredientIconMasks.Count; i++)
                ingredientIconMasks[i].SetActive(false);
    }

    public void AssignDroppingIngredient(Ingredient newIngredient)
    {
        droppingIngredient = newIngredient;
        SetInputInfoState(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!ingredientLimitReached)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (other.gameObject.GetInstanceID() == cauldronOwner.gameObject.GetInstanceID())
                    if (cauldronOwner.carryingIngredient != null)
                        SetInputInfoState(true);
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject.GetInstanceID() == cauldronOwner.gameObject.GetInstanceID())
                if (cauldronOwner.carryingIngredient != null)
                    SetInputInfoState(false);
        }
    }

    void SetInputInfoState(bool state)
    {
        inputInfo.SetActive(state);
    }

}
