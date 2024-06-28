using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Plant", menuName = "Plant")]
public class plantObject : ScriptableObject
{
    public string plantName;
    public GameObject[] plant;
    public float timeBetweenStages;

}
