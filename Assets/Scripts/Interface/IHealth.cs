using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevHQ.Interface.IHealth
{
    public interface IHealth
    {
        int StartingHealth { get; set; }
        int Health { get; set; }

        void Damage(int amount);
        void Destroyed();
    }
}

