using GameDevHQ.Enemy.EnemyClassNS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevHQ.Other.EndPointNS
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class EndPoint : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Enemy")
            {
                EnemyClass enemy = other.GetComponent<EnemyClass>();
                if (enemy != null)
                {
                    enemy.ReachedPathEnd();
                }
            }
        }
    }
}

