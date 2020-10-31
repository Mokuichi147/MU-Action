using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public TableTennisAgent player_a;
    public TableTennisAgent player_b;
    
    private Vector3 bool_pos;
    private Quaternion bool_rot;
    private Rigidbody bool_rb;
    private string pre_action;
    private string next_action;
    private int action_count;
    private bool serve;
    private float pre_pos;

    void Start()
    {
        bool_rb = this.GetComponent<Rigidbody>();
        bool_pos = this.transform.position;
        bool_rot = this.transform.rotation;
        PreSet();
    }

    void PreSet()
    {
        serve = true;
        pre_action = "serve";
        next_action = "a_racket";
        action_count = 0;
    }

    void Reset()
    {
        player_a.EndEpisode();
        player_b.EndEpisode();
        this.transform.position = bool_pos;
        this.transform.rotation = bool_rot;
        bool_rb.velocity = new Vector3(0f, 0f, 0f);
        PreSet();
    }

    void WinA()
    {
        player_a.SetReward(1);
        player_b.SetReward(-1);
        Reset();
    }
    void WinB()
    {
        player_a.SetReward(-1);
        player_b.SetReward(1);
        Reset();
    }

    void Win(string cgo_name)
    {
        switch (pre_action)
        {
            case "serve":
                WinB();
                break;
            case "Agent_A":
            case "Table_A":
                WinB();
                break;
            case "Agent_B":
            case "Table_B":
                WinA();
                break;
        }
    }

    void FixedUpdate()
    {
        if (pre_pos == this.transform.position.y)
            action_count++;
        else
        {
            action_count = 0;
            pre_pos = this.transform.position.y;
        }
        if (action_count > 200)
        {
            WinB();
            Debug.Log("hello");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        var cgo = collision.gameObject;
        if (cgo.CompareTag("Ground"))
        {
            Win("ground");
        }
        else if (cgo.CompareTag("Racket"))
        {
            if (next_action == cgo.gameObject.name)
            {
                if (pre_action == next_action)
                {
                    Win(cgo.gameObject.name);
                }
                pre_action = next_action;
                if (serve)
                {
                    next_action = "Table_B";
                    serve = false;
                }
                else
                {
                    next_action = cgo.gameObject.name == "Agent_A" ? "Table_B" : "Table_A";
                }
            }
            else
            {
                Win(cgo.gameObject.name);
            }
        }
        else if (cgo.CompareTag("Table"))
        {
            if (pre_action == next_action)
                {
                    Win(cgo.gameObject.name);
                }
            if (next_action == cgo.gameObject.name)
            {
                pre_action = next_action;
                next_action = cgo.gameObject.name == "Table_A" ? "Agent_A" : "Agent_B";
            }
            else
            {
                Win(cgo.gameObject.name);
            }
        }
    }
}
