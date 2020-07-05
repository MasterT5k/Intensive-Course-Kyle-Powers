using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevHQ.Interface.IHealthNS
{
    public interface IHealth
    {
        int StartingHealth { get; set; }
        int Health { get; set; }
        bool Damaged { get; set; }
        GameObject HealthBar { get; }
        GameObject MainCamera { get; }
        MeshRenderer HealthRender { get; }
        MaterialPropertyBlock MatBlock { get; set; }

        void AlignToCamera();
        void Damage(int amount);
        void Destroyed();
    }
}

