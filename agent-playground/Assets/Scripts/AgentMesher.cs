using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MarchingCubes;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;

public class AgentMesher : MonoBehaviour
{
    [Header("Referenced Elements")]
    public StigmergyManager stigmergy;
   

    [Header("Meshing Settings")]
    public int _triangleBudget = 65536;
    public float _targetValue = 0;

    [Header("Compute")]
    public ComputeShader _builderCompute;

    private ComputeBuffer _voxelBuffer;
    private MeshBuilder _builder;

    private bool initialized;



    // Start is called before the first frame update
    void Start()
    {
        initialized = false;
       
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time < 0.5f) return;
        if (!initialized)
        {
            _builder = new MeshBuilder(stigmergy.res_x, stigmergy.res_y, stigmergy.res_z, _triangleBudget, _builderCompute);
            _voxelBuffer = new ComputeBuffer(stigmergy.num_cells, sizeof(float));

            
            initialized = true;

        }

        // Isosurface reconstruction

        _voxelBuffer.SetData(stigmergy.job_flipped_scalar_field);
        _builder.BuildIsosurface(_voxelBuffer, _targetValue, stigmergy.cell_size);
        GetComponent<MeshFilter>().sharedMesh = _builder.Mesh;
    }

    private void OnDestroy()
    {
        _builder.Dispose();
        _voxelBuffer.Dispose();
    }
}


