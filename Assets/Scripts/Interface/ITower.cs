using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevHQ.Interface.ITowerNS
{
    public interface ITower
    {
        int WarFundValue { get; set; }
        int TowerID { get; set; }
        MeshRenderer AttackRange { get; set; }
        GameObject EnemyToTarget { get; set; }
        bool IsEnemyInRange { get; set; }

        void PlaceMode(bool inPlaceMode);
        void AttackEnemy(GameObject enemy);
        void NoEnemiesInRange();
    }
}
