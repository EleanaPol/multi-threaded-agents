using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;

[BurstCompile]
public struct ExtractAgentPositionsJob : IJobParallelFor
{
    public NativeArray<Agent> agents;
    public NativeArray<float3> positions;

    public float3 root;

    public void Execute(int index)
    {
        var pos = agents[index].position + root;

        positions[index] = pos;// agents[index].position;
    }
}
