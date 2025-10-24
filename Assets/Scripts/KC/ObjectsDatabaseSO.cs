using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu]
public class TowersDatabaseSO : ScriptableObject
{
    public List<TowerData> TowersData;
}

[Serializable]
public class TowerData
{
    [field: SerializeField]
    public string Name { get; private set; }
    [field: SerializeField]
    public int ID { get; private set; }
    [field: SerializeField]
    public GameObject Prefab { get; private set; }
}