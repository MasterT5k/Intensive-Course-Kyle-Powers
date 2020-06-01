using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crawler : EnemyClass
{
    private void OnDisable()
    {
        reused = true;
        distance = Mathf.Infinity;
    }

    private void OnEnable()
    {
        if (reused == true)
        {
            Respawn();
        }
    }
    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
}
