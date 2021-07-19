using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ingredient", menuName = "Scriptable Objects/Ingredient")]
public class Ingredient_SO : ScriptableObject
{
    public string Name;

    public GameObject ingredientPrefab;

    public Sprite ingredientPreview;
}
