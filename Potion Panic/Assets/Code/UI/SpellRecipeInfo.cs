using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellRecipeInfo : MonoBehaviour
{
    public Spell_SO spell;

    public Image spellIcon;
    public List<Image> ingredients;

    // Start is called before the first frame update
    void Start()
    {
        spellIcon.sprite = spell.spellPreview;

        for(int i = 0; i < ingredients.Count; i++)
            ingredients[i].sprite = spell.requiredIngredients[i].ingredientPreview;
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}
}
