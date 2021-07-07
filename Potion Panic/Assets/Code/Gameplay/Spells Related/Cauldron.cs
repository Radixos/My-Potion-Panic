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

    // Start is called before the first frame update
    //void Start()
    //{

    //}

    // Update is called once per frame
    void Update()
    {
        ConsumeIngredient();
    }

    void ConsumeIngredient()
    {
        if (droppingIngredient != null)
        {
            if (Vector3.Distance(droppingIngredient.transform.position, cauldronCore.position) <= 0.2f)
            {
                // PERFORM INGREDIENT ACTION

                droppingIngredient.gameObject.SetActive(false); // False if ingredients are object pooled
                droppingIngredient = null;
            }
        }
    }

    public void AssignDroppingIngredient(Ingredient newIngredient)
    {
        droppingIngredient = newIngredient;
        SetInputInfoState(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject.GetInstanceID() == cauldronOwner.gameObject.GetInstanceID())
                if (cauldronOwner.carryingIngredient != null)
                    SetInputInfoState(true);
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
