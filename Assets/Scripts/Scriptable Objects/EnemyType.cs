using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Enemy", menuName = "Scriptable Objects/Spawn Manager/Enemy")]
public class EnemyType : ScriptableObject
{
    public string enemyName;
    public int enemyID = -1;
    public GameObject enemyPrefab;
}
