using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpellType { AREA = 0, PROJECTILE = 1 };

[CreateAssetMenu(fileName = "New Spell", menuName = "Scriptable Objects/Spell")]
public class Spell_SO : ScriptableObject
{
    public string Name;

    public string Description;

    public Sprite spellPreview;

    public List<Ingredient_SO> requiredIngredients;

    public GameObject spellPrefab;

    public SpellType spellType;

    public int NumberOfUses;

}