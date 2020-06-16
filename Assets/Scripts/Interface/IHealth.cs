using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevHQ.Interface.IHealth
{
    public interface IHealth
    {
        int Health { get; set; }

        void Damage(int amount);
    }
}

