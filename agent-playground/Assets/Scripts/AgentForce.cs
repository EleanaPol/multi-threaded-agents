using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;

public class AgentForce : MonoBehaviour
{
    [HideInInspector] public int num_agents;
    public NativeArray<Agent> job_agents;
    public NativeArray<float3> job_agent_force;
    [HideInInspector] public AgentPopulation population;

    [HideInInspector] public float strength;

    [HideInInspector] public bool force_initialized;

    private void Awake()
    {
        force_initialized = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region Override methods

    public void InitForce()
    {
        AllocateJobMemory();
        force_initialized = true;
    }

    public virtual void CalculateForce()
    {
        
    }

    private void OnDestroy()
    {
        ClearJobMemory();
    }

    #endregion

    #region Jobs

    private void AllocateJobMemory()
    {
        job_agent_force = new NativeArray<float3>(num_agents, Allocator.Persistent);
    }

    private void ClearJobMemory()
    {
        if (job_agent_force.IsCreated) job_agent_force.Dispose();
    }

    public void ScheduleForceClearJob()
    {
        var clearForce = new ClearForcesJob
        {
            forces = job_agent_force
        };
        clearForce.Schedule(num_agents, 128).Complete();
    }

    #endregion
}
