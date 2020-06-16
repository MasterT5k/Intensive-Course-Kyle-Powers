using GameDevHQ.Interface.ITowerNS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class AttackRange : MonoBehaviour
{
    private ITower _tower;
    [SerializeField]
    private List<GameObject> _enemysInRange = new List<GameObject>();

    void Start()
    {
        _tower = transform.GetComponentInParent<ITower>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            GameObject enemy = other.gameObject;
            _enemysInRange.Add(enemy);
            _tower.AttackEnemy(_enemysInRange[0]);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            GameObject enemy = other.gameObject;
            _enemysInRange.Remove(enemy);
            if (_enemysInRange.Count > 0)
            {
                _tower.AttackEnemy(_enemysInRange[0]);
            }
            else
            {
                _tower.NoEnemiesInRange();
            }
        }
    }
}
