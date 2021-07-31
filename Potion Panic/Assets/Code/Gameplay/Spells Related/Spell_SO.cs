using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Scriptable Objects/Spell")]
public class Spell_SO : ScriptableObject
{
    public string Name;

    public string Description;

    public Sprite spellPreview;

    public List<Ingredient_SO> requiredIngredients;

    public GameObject spellPrefab;

    public int NumberOfUses;

}
