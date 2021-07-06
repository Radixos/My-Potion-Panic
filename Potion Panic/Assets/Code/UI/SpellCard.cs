using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellCard : MonoBehaviour
{
    public Spell_SO spell;

    public Text spellName;
    public Text Description;
    public Image spellPreview;

    public List<Text> ingredientsList;

    public Text NumOfUses;

    // Start is called before the first frame update
    void Start()
    {
        if (ingredientsList.Count != spell.requiredIngredients.Count)
            Debug.LogError("Ingredients List does not match with Scriptable Object");
    }

    // Update is called once per frame
    void Update()
    {
        spellName.text = spell.Name;
        Description.text = spell.Description;
        spellPreview.sprite = spell.spellImage;

        for(int i = 0; i < spell.requiredIngredients.Count; i++)
        {
            ingredientsList[i].text = (i + 1).ToString() + ". " + spell.requiredIngredients[i].GetComponent<Ingredient>().Name;
        }

        NumOfUses.text = spell.NumberOfUses.ToString();

    }
}
