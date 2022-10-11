using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;


[BurstCompile]
public struct BakeMeshForceJob : IJobParallelFor
{
    public NativeArray<float3> grid_points;
    public NativeArray<float3> field_points;
    [NativeDisableParallelForRestriction] public NativeArray<Vector3> mesh_vertices;

    public int vertex_count;
    public float cell_size;


    public void Execute(int index)
    {
        var pos = grid_points[index] + (-new float3(cell_size * 0.5f));
        float3 vector = float3.zero;
        float shortest_dist = math.INFINITY;

        for(int i=0; i<vertex_count; i++)
        {
            var vert = mesh_vertices[i];
            var diff = (float3)vert - pos;
            var dist = math.length(diff);

            if (dist < shortest_dist)
            {
                vector = diff;
                shortest_dist = dist;
            }
        }

        field_points[index] = math.length(vector) > 0 ? math.normalize(vector) : float3.zero;
    }
}
