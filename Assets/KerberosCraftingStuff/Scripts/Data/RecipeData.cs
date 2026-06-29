using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewList", menuName = "FactoryFrontline/RecipeList")]
public class RecipeData : ScriptableObject
{
    public Recipe[] recipes;
}

[System.Serializable]
public class Recipe
{
    [Header("Tower Info")]
    public string towerName;
    public int towerID;
    public GameObject packagePrefab;
    [Space]
    [Header("Components")]
    public int weapID;
    public int coreID;
    public int baseID;
}
