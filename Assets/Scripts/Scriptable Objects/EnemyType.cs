using GameDevHQ.Enemy.EnemyClassNS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Enemy", menuName = "Scriptable Objects/Spawn Manager/Enemy")]
public class EnemyType : ScriptableObject
{
    private EnemyClass _enemyClass;
    public string enemyName;
    public GameObject enemyPrefab;
    public EnemyClass enemyClass { get
        {
            if (_enemyClass == null)
                _enemyClass = enemyPrefab.GetComponent<EnemyClass>();

            return _enemyClass;
        } }
}
