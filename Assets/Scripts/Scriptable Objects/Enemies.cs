using GameDevHQ.Enemy.EnemyClassNS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new EnemyList", menuName = "Scriptable Objects/Spawn Manager/Enemy List")]
public class Enemies : ScriptableObject
{
    public EnemyType[] enemyScriptables;
    private List<EnemyClass> _enemyClasses = new List<EnemyClass>();

    public void SetClassList()
    {
        for (int i = 0; i < enemyScriptables.Length; i++)
        {
            EnemyClass enemy = enemyScriptables[i].enemyPrefab.GetComponent<EnemyClass>();
            if (enemy != null)
            {
                _enemyClasses.Add(enemy);
            }
        }
    }

    public GameObject GetEnemyPrefab(int enemyID)
    {
        for (int i = 0; i < enemyScriptables.Length; i++)
        {
            if (_enemyClasses[i].GetEnemyID() == enemyID)
            {
                return enemyScriptables[i].enemyPrefab;
            }
        }
        return null;
    }

    public EnemyClass GetEnemyClass(int enemyID)
    {
        for (int i = 0; i < _enemyClasses.Count; i++)
        {
            if (_enemyClasses[i].GetEnemyID() == enemyID)
            {
                return _enemyClasses[i];
            }
        }
        return null;
    }
}
