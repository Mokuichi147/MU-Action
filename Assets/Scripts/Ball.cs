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

    void NoWin()
    {
        player_a.SetReward(-1);
        player_b.SetReward(-1);
        Reset();
    }

    void Win(string cgo_name)
    {
        switch (pre_action)
        {
            case "serve":
                WinB();
                break;
            case "Rubber_A":
            case "Table_A":
                WinB();
                break;
            case "Rubber_B":
            case "Table_B":
                WinA();
                break;
        }
    }

    void FixedUpdate()
    {
        var y_pos = this.transform.position.y;
        if (pre_pos == y_pos)
            action_count++;
        else
        {
            action_count = 0;
            pre_pos = y_pos;
        }
        if (action_count > 20)
        {
            NoWin();
        }
        else if (y_pos <= 0f)
        {
            NoWin();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        var cgo = collision.gameObject;
        if (cgo.CompareTag("Ground"))
        {
            Win("ground");
        }
        else if (cgo.gameObject.name == "Rubber_A")
        {
            if (serve)
            {
                player_a.SetReward(0.2f);
                next_action = "Table_B";
                pre_action = "Rubber_A";
            }
            else
            {
                next_action = "Table_A";
                pre_action = "Rubber_A";
            }
        }
        else if (cgo.gameObject.name == "Rubber_B")
        {
            next_action = "Table_B";
            pre_action = "Rubber_B";
        }
        else if (cgo.gameObject.name == "Table_A")
        {
            player_a.SetReward(0.6f);
            next_action = "Rubber_B";
            pre_action = "Table_A";
        }
        else if (cgo.gameObject.name == "Table_B")
        {
            if (serve)
            {
                player_a.SetReward(0.4f);
                next_action = "Table_A";
                serve = false;
            }
            else
            {
                player_b.SetReward(0.6f);
                next_action = "Rubber_A";
                pre_action = "Table_B";
            }
        }
        else
        {
            Win(cgo.gameObject.name);
        }
    }
}
