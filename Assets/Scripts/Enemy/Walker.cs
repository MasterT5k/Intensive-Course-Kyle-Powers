using UnityEngine;
using UnityEngine.AI;

public class Walker : EnemyClass
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
