using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decoy : MonoBehaviour
{
    private MeshRenderer _rangeRender;
    // Start is called before the first frame update
    void Start()
    {
        _rangeRender = transform.Find("Attack Range").GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
