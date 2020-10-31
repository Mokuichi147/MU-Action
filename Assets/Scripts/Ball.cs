using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public TableTennisAgent Player_A;
    public TableTennisAgent Player_B;
    
    private int bound_count;
    private string pre_action;

    void Start()
    {
        Reset();
    }

    void Reset()
    {
        bound_count = 0;
        pre_action = "";
    }

    void FixedUpdate()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        
    }
}
