using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;

public class AgentPopulation : MonoBehaviour
{
   
    [Header("Population Settings")]
    public int num_agents;

    [Header("Referenced Elements")]
    public AgentEnvironment a_environment;
    public GameObject agent_object;

    [Header("System Forces")]
    public SystemForce[] forces;
    

    // Native Arrays
    private NativeArray<Agent> job_agents;
    public NativeArray<int> job_bins;
    public NativeArray<int> job_bin_counters;
    private NativeArray<float3> job_agent_overall_forces;

    // private variables
    private int num_bins;
    private int num_cells;
    [HideInInspector] public int num_agents_per_bin;

    [HideInInspector] public int bin_x_res;
    [HideInInspector] public int bin_z_res;
    [HideInInspector] public int bin_y_res;
    [HideInInspector] public int bin_cell_size;
    private Vector3 axis_offset;

    private Vector3 min_pt;
    private Vector3 max_pt;

    private int num_forces;
    private Unity.Mathematics.Random random;


    #region MonoBehaviour

    // Start is called before the first frame update
    void Start()
    {
        Init();
        AllocateJobMemory();
        InitPopulation();

        InitializePopulationForces();

        CreateAgentObjects();
    }

    // Update is called once per frame
    void Update()
    {
        AssignToBins();
        UpdatePopulationForces();
        ClearAgentForces();

        UpdateAgentObjects();
    }

    private void OnDestroy()
    {
        ClearJobMemory();
    }

    #endregion

    #region Initialization

    private void Init()
    {
        num_cells = a_environment.grid_resolution;
        num_bins = a_environment.bin_resolution;
        num_agents_per_bin = a_environment.agents_per_bin;
        min_pt = a_environment.min_pt;
        max_pt = a_environment.max_pt;

        axis_offset = a_environment.axis_offset_raw;
        bin_cell_size = a_environment.bin_cell_size;
        bin_x_res = a_environment.x_bin_res;
        bin_z_res = a_environment.z_bin_res;
        bin_y_res = a_environment.y_bin_res;

        num_forces = forces.Length;
    }

    #endregion

    #region Population Management
    private void InitPopulation()
    {
        for(int i=0; i<num_agents; i++)
        {
            // create random position
            var x = UnityEngine.Random.Range(min_pt.x, max_pt.x)/5.0f;
            var y = UnityEngine.Random.Range(min_pt.y, max_pt.y) / 5.0f;
            var z = UnityEngine.Random.Range(min_pt.z, max_pt.z) / 5.0f;

            // init agent to random position
            var agent = job_agents[i];
            agent.id = i;
            agent.position = new float3(x, y, z);
            agent.prev_position = new float3(x, y, z);
            agent.velocity = float3.zero;
            agent.bin_id = 0;
            job_agents[i] = agent;
        }

        
    }

    private void InitializePopulationForces()
    {
        for (int i = 0; i < num_forces; i++)
        {
            if (forces[i].enabled)
            {                
                forces[i].agent_force.strength = forces[i].force_strength;
                forces[i].agent_force.num_agents = num_agents;
                forces[i].agent_force.population = this;               
                forces[i].agent_force.InitForce();
            }
        }
    }

    private void UpdatePopulationForces()
    {
        for (int i = 0; i < num_forces; i++)
        {
            if (forces[i].enabled)
            {
                forces[i].agent_force.strength = forces[i].force_strength;
                forces[i].agent_force.job_agents = job_agents;
                forces[i].agent_force.population = this;

                forces[i].agent_force.CalculateForce();

                ScheduleForceAdditionJob(forces[i].agent_force.job_agent_force, forces[i].force_strength);
            }
        }

        ScheduleAgentMove();
    }

    private void ClearAgentForces()
    {
        ScheduleClearAgentForce();
    }

    private void AssignToBins()
    {
        ScheduleBinCleaning();
        ScheduleBinAssignmentJob();
    }

    #endregion



    #region Jobs

    private void AllocateJobMemory()
    {
        job_agents = new NativeArray<Agent>(num_agents, Allocator.Persistent);
        job_bins = new NativeArray<int>(num_bins * num_agents_per_bin, Allocator.Persistent);
        job_bin_counters = new NativeArray<int>(num_bins, Allocator.Persistent);
        job_agent_overall_forces = new NativeArray<float3>(num_agents, Allocator.Persistent);
    }

    private void ClearJobMemory()
    {
        if (job_agents.IsCreated) job_agents.Dispose();
        if (job_bins.IsCreated) job_bins.Dispose();
        if (job_bin_counters.IsCreated) job_bin_counters.Dispose();
        if (job_agent_overall_forces.IsCreated) job_agent_overall_forces.Dispose();
    }

    private void ScheduleBinCleaning()
    {
        var BinCleanJob = new ClearBinsJob
        {
            bin_counters = job_bin_counters
        };

        BinCleanJob.Schedule(num_bins, 128).Complete();
    }

    private void ScheduleBinAssignmentJob()
    {
        var BinAssignJob = new AssignAgentsToBinsJob
        {
            agents = job_agents,
            bins = job_bins,
            bin_counters = job_bin_counters,
            bin_size = bin_cell_size,
            bin_x_res = bin_x_res,
            bin_z_res = bin_z_res,
            bin_capacity = num_agents_per_bin,
            offset = axis_offset
        };
        BinAssignJob.Schedule(num_agents, 128).Complete();
    }

    private void ScheduleForceAdditionJob(NativeArray<float3> forces, float strength)
    {
        var ForceAdditionJob = new CalculateAgentForcesJob
        {
            agent_forces = forces,
            overall_agent_forces = job_agent_overall_forces,

            speed = strength,
            delta_time = Time.deltaTime

        };

        ForceAdditionJob.Schedule(num_agents, 128).Complete();
    }

    private void ScheduleAgentMove()
    {
        random = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(1, 100));
        var AgentMoveJob = new MoveAgentsJob
        {
            agents = job_agents,
            overall_force = job_agent_overall_forces,

            min = min_pt,
            max = max_pt,
            random = random
        };
        AgentMoveJob.Schedule(num_agents, 128).Complete();
    }

    private void ScheduleClearAgentForce()
    {
        var clearForce = new ClearForcesJob
        {
            forces = job_agent_overall_forces
        };
        clearForce.Schedule(num_agents, 128).Complete();
    }


    #endregion

    #region Testing

    private void CreateAgentObjects()
    {
        for(int i=0; i<num_agents; i++)
        {
            //GameObject agent = GameObject.CreatePrimitive(PrimitiveType.Cube);
            GameObject agent = Instantiate(agent_object);

            agent.name = "agent_" + i.ToString();
            agent.transform.position = job_agents[i].position;
            //agent.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            agent.transform.SetParent(transform);
        }
    }

    private void UpdateAgentObjects()
    {
        for (int i = 0; i < num_agents; i++)
        {
            var agent = transform.GetChild(i);
            agent.transform.position = job_agents[i].position;
        }
    }

    #endregion
}
