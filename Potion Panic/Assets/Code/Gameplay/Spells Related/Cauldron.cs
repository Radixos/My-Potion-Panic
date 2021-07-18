using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private List<Ingredient> consumedIngredients = new List<Ingredient>();
    private List<Ingredient> correctIngredients = new List<Ingredient>();
    public List<Spell_SO> spellPool; // Should be Spell Prefab

    // EVENTS
    public delegate void OnSuccess(Spell_SO brewedSpell);
    public event OnSuccess OnSuccessEvent;

    public delegate void OnFailure(Transform t);
    public event OnFailure OnFailureEvent;

    // Start is called before the first frame update
    //void Start()
    //{

    //}

    // Update is called once per frame
    void Update()
    {
        if (ingredientLimitReached)
            BrewSpell();
        else
            ConsumeIngredient();
    }

    void ConsumeIngredient()
    {
        if (droppingIngredient != null)
        {
            if (Vector3.Distance(droppingIngredient.transform.position, cauldronCore.position) <= 0.2f)
            {
                // PERFORM INGREDIENT ACTION
                consumedIngredients.Add(droppingIngredient);

                if (consumedIngredients.Count >= 3)
                    ingredientLimitReached = true;

                droppingIngredient.gameObject.SetActive(false); // Set to false if ingredients are object pooled
                droppingIngredient = null;
            }
        }
    }

    void BrewSpell()
    {
        for (int i = 0; i < spellPool.Count; i++) // Looping through Spells
        {
            for (int j = 0; j < consumedIngredients.Count; j++) // Looping through every ingredient of a spell
            {
                if (spellPool[i].requiredIngredients.Contains(consumedIngredients[j].ingredientInfo))
                {
                    correctIngredients.Add(consumedIngredients[j]);

                    if(correctIngredients.Count >= 3)
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
            OnFailureEvent?.Invoke(transform);
        }

        // Reset Cauldron
        ingredientLimitReached = false;
        spellBrewed = false;
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
