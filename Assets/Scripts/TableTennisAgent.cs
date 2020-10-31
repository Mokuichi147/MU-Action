using System;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;


public class TableTennisAgent : Agent
{
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
            sensor.AddObservation(opponent.transform.position - this.transform.position);
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var actionF = 50f * Mathf.Clamp(actionBuffers.ContinuousActions[0], 0f, 1f);
        var actionR = 2f * Mathf.Clamp(actionBuffers.ContinuousActions[1], -1f, 1f);

        if (this.transform.position.y < 0f)
        {
            SetReward(-1f);
        }
        else if (opponent.transform.position.y < 0f)
        {
            SetReward(1f);
        }
        else
        {
            SetReward(0.1f);
        }
    }

    public override void OnEpisodeBegin()
    {
        this.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        this.transform.Rotate(new Vector3(0, 1, 0), Random.Range(-10f, 10f));
        AgentReset();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = -Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
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
