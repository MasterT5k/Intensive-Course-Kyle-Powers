using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevHQ.Tower.ITowerNS
{
    public interface ITower
    {
        int WarFundValue { get; set; }
        int TowerID { get; set; }
        GameObject AttackRange { get; set; }

        void PlaceMode(bool inPlaceMode);
    }
}
