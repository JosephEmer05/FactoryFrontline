using UnityEngine;

[CreateAssetMenu(fileName = "NewComponent", menuName = "FactoryFrontline/Component")]
public class ComponentData : ScriptableObject
{
    public string componentName;
    public int componentID;
    public ComponentType type;
    public Sprite icon;
    public GameObject prefab;
}

public enum ComponentType
{
    Base,
    Core,
    Weapon
}
