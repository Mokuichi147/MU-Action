using System;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;


public class TableTennisAgent : Agent
{
    public GameObject ball;
    public GameObject opponent;
    public bool useVecObs;
    
    Vector3 self_pos;
    Quaternion self_rot;
    ActionAgent opponent_aa;
    EnvironmentParameters npc_reset_params;

    public override void Initialize()
    {
        self_pos = this.transform.position;
        self_rot = this.transform.rotation;
        opponent_aa = opponent.GetComponent<ActionAgent>();
        npc_reset_params = Academy.Instance.EnvironmentParameters;
        AgentReset();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (useVecObs)
        {
            sensor.AddObservation(this.transform.position);
            sensor.AddObservation(ball.transform.position - this.transform.position);
            sensor.AddObservation(opponent.transform.position - this.transform.position);
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var scale = 0.005f;
        var continuousActions = actionBuffers.ContinuousActions;
        var moveX = Mathf.Clamp(continuousActions[0], -1f, 1f) * scale;
        var moveZ = Mathf.Clamp(continuousActions[1], -1f, 1f) * scale;
        var moveY = Mathf.Clamp(continuousActions[2], -1f, 1f) * scale;
        
        var new_pos = this.transform.position + new Vector3(moveX, moveY, moveZ);
        var new_pos_sa = new_pos - this.transform.parent.gameObject.transform.position;
        if (new_pos_sa.z < 1.5 && new_pos_sa.y < 0.85 && Mathf.Abs(new_pos_sa.x) < 1f)
        {
            new_pos -= new Vector3(moveX, moveY, moveZ);
            SetReward(-1f);
        }
        else if (new_pos_sa.z < 0.15)
        {
            new_pos.z -= moveZ;
            SetReward(-0.5f);
        }
        else
        {
            SetReward(0.1f);
        }
        this.transform.position = new_pos;
    }

    public override void OnEpisodeBegin()
    {
        //this.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        //this.transform.Rotate(new Vector3(0, 1, 0), Random.Range(-10f, 10f));
        AgentReset();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        var key_w = Input.GetKey(KeyCode.W) ? 1f : 0f;
        var key_s = Input.GetKey(KeyCode.S) ? -1f : 0f;
        continuousActionsOut[0] = key_w + key_s;
        var key_a = Input.GetKey(KeyCode.A) ? 1f : 0f;
        var key_d = Input.GetKey(KeyCode.D) ? -1f : 0f;
        continuousActionsOut[1] = key_a + key_d;
        var key_e = Input.GetKey(KeyCode.E) ? 1f : 0f;
        var key_q = Input.GetKey(KeyCode.Q) ? -1f : 0f;
        continuousActionsOut[2] = key_e + key_q;
    }

    public void AgentSet()
    {
        //var scale = npc_reset_params.GetWithDefault("scale", 1.0f);
        //this.transform.localScale =  new Vector3(scale, scale, scale);
        this.transform.position = self_pos;
        this.transform.rotation = self_rot;
    }

    public void AgentReset()
    {
        AgentSet();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("wall"))
        {
            SetReward(-1);
            opponent_aa.SetReward(1);
            EndEpisode();
            opponent_aa.EndEpisode();
        }
    }
}
