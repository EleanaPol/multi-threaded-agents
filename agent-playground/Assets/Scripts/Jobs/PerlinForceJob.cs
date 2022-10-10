using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;

[BurstCompile]
public struct PerlinForceJob : IJobParallelFor
{
    public NativeArray<Agent> agents;
    public NativeArray<float3> agent_forces;

    public float n_scale;
    public float n_speed;
    public float time;


    public void Execute(int index)
    {       
        var pos = agents[index].position;
        float3 gr = float3.zero;
        float3 sampler = new float3(pos.x * n_scale + time * n_speed + pos.y * n_scale + time * n_speed + pos.z * n_scale + time * n_speed);
        noise.snoise(sampler, out gr);
        //Debug.Log(gr);

        agent_forces[index] = gr;
    }
}
