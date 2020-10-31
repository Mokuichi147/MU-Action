﻿using System;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;


public class ActionAgent : Agent
{
    public GameObject enemy;
    public bool useVecObs;
    
    Rigidbody npc_rb;
    Rigidbody enemy_rb;
    EnvironmentParameters npc_reset_params;

    public override void Initialize()
    {
        npc_rb = this.GetComponent<Rigidbody>();
        enemy_rb = enemy.GetComponent<Rigidbody>();
        npc_reset_params = Academy.Instance.EnvironmentParameters;
        AgentReset();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (useVecObs)
        {
            sensor.AddObservation(this.transform.position.x);
            sensor.AddObservation(this.transform.position.z);
            sensor.AddObservation(enemy.transform.position - this.transform.position);
            sensor.AddObservation(npc_rb.velocity);
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var actionF = 100f * Mathf.Clamp(actionBuffers.ContinuousActions[0], -1f, 1f);
        var actionR = 2f * Mathf.Clamp(actionBuffers.ContinuousActions[1], -1f, 1f);

        npc_rb.transform.Rotate(new Vector3(0, 1, 0), actionR);
        npc_rb.AddForce(this.transform.forward * actionF);

        if (this.transform.position.y < 0f)
        {
            SetReward(-1f);
        }
        else if (enemy.transform.position.y < 0f)
        {
            SetReward(1f);
        }
        else
        {
            SetReward(-0.1f);
        }
    }

    public override void OnEpisodeBegin()
    {
        this.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        this.transform.Rotate(new Vector3(0, 1, 0), Random.Range(-10f, 10f));
        npc_rb.velocity = new Vector3(0f, 0f, 0f);
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
        npc_rb.mass = npc_reset_params.GetWithDefault("mass", 1.0f);
        var scale = npc_reset_params.GetWithDefault("scale", 1.0f);
        this.transform.localScale =  new Vector3(scale, scale, scale);
    }

    public void AgentReset()
    {
        AgentSet();
    }
}
