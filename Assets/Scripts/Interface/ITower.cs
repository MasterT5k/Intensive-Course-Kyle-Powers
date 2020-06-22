using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevHQ.Interface.IHealthNS;

namespace GameDevHQ.Interface.ITowerNS
{
    public interface ITower
    {
        int WarFundValue { get; set; }
        int TowerID { get; set; }
        int DamageAmount { get; set; }
        float AttackDelay { get; set; }
        Transform RotationObj { get; set; }
        MeshRenderer AttackRange { get; set; }
        GameObject EnemyToTarget { get; set; }
        IHealth TargetHealth { get; set; }
        bool IsEnemyInRange { get; set; }
        List<GameObject> EnemiesInRange { get; set; }

        void Init();
        void PlaceMode(bool inPlaceMode);
        void RemoveEnemy(GameObject enemy);
        void AttackEnemy(GameObject enemy);
        void NoEnemiesInRange();
    }
}
