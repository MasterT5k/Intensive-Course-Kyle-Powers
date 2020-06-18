using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevHQ.Interface.ITowerNS
{
    public interface ITower
    {
        int WarFundValue { get; set; }
        int TowerID { get; set; }
        int Damage { get; set; }
        float AttackDelay { get; set; }
        Transform RotationObj { get; set; }
        MeshRenderer AttackRange { get; set; }
        GameObject EnemyToTarget { get; set; }
        bool IsEnemyInRange { get; set; }
        List<GameObject> EnemiesInRange { get; set; }

        void Init();
        void PlaceMode(bool inPlaceMode);
        void RemoveEnemy(GameObject enemy);
        void AttackEnemy(GameObject enemy);
        void NoEnemiesInRange();
    }
}
