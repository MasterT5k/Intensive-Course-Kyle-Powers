using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITower
{
    int WarFundValue { get; set; }
    int TowerID { get; set; }
    GameObject AttackRange { get; set; }

    void PlaceMode(bool inPlaceMode);
}
