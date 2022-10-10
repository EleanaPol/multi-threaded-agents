using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;

[BurstCompile]
public struct MoveAgentsJob : IJobParallelFor
{
    public NativeArray<Agent> agents;
    public NativeArray<float3> overall_force;

    public float3 min;
    public float3 max;

    public void Execute(int index)
    {
        var agent = agents[index];
        var force = overall_force[index];
        var pos = agent.position;

        var moved_pos = pos + force;
        if(moved_pos.x <= min.x || moved_pos.x >= max.x 
            || moved_pos.y <= min.y || moved_pos.y >= max.y 
            || moved_pos.z <= min.z || moved_pos.z >= max.z)
        {
            agent.position = pos;
            agent.velocity = float3.zero;

        }
        else
        {
            agent.position += force;
            agent.velocity = agent.position - agent.prev_position;
            agent.prev_position = agent.position;
        }      
        agents[index] = agent;
    }
}
