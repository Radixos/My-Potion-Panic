using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellCard : MonoBehaviour
{
    public Spell_SO spellInfo;

    public Text spellName;
    public Text Description;
    public Image spellPreviewImage;

    public List<Image> ingredientsList;

    public Text NumOfUses;

    // Start is called before the first frame update
    void Start()
    {
        if (ingredientsList.Count != spellInfo.requiredIngredients.Count)
            Debug.LogError("Ingredients List does not match with list in Scriptable Object!");
    }

    // Update is called once per frame
    void Update()
    {
        spellName.text = spellInfo.Name;
        Description.text = spellInfo.Description;
        spellPreviewImage.sprite = spellInfo.spellPreview;

        for(int i = 0; i < spellInfo.requiredIngredients.Count; i++)
        {
            //ingredientsList[i].text = (i + 1).ToString() + ". " + spellInfo.requiredIngredients[i].GetComponent<Ingredient>().ingredientInfo.Name;
            ingredientsList[i].sprite = spellInfo.requiredIngredients[i].ingredientPreview;
        }

        NumOfUses.text = spellInfo.NumberOfUses.ToString();

    }
}
