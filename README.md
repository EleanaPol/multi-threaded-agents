# multi-threaded-agents

## Description
This Unity project manages CPU multithreaded autonomous agents vis Unity's [Job System](https://docs.unity3d.com/Manual/JobSystem.html). The agent system incorporates a variety of forces, some of which are agent-centric and others environnment-centric. To optimize the overall performance, a simple spatial binning system has been developed and the agent entities are drawn directly in graphics, through GPU instancing. The agent environment is expressed as a behind the scenes 3D voxel grid of customizable resolution. The voxel grid enables the agents' stigmergic behaviours as well as the meshing of the agents' paths through a [Marching Cubes](https://en.wikipedia.org/wiki/Marching_cubes) algorithm. The meshed paths are also drawn directly in the GPU but there is the option to export selected mesh "frames" in .obj file format.  




https://user-images.githubusercontent.com/45208539/196006390-28015d17-689b-4a2a-8f50-63a02a5648f2.mp4


---

## Project Info

**Author:** 
[Eleana Polychronaki](https://github.com/EleanaPol)

**Development Platform:**
Unity 2021.3

**Target Platform:**
Windows Standalone/Mac

**Working Scene:**
Playground

## System Breakdown
### Environment
The system's environment is handled through the **AgentEnvironment** component and is responsible for generating the 3D voxel grid that will hold the information of the agents' movement in space. The component requires a Box Collider component reference to define the region within which the agents will move and be tracked. The cell size defines the resolution of the 3D voxel grid that will be generated within the agent region. The AgentEnvironment component also handles the binning system by defining the bin cell size and the maximum agents allowed per bin. Each agent system requires only  **one** environment.  

![Image](https://github.com/EleanaPol/multi-threaded-agents/blob/main/Documentation/Environment.PNG)

### Agent Forces
All Agent forces derive from the same base class and have an **OVERALL STRENGTH** variable that is responsible for defining the strength of each force at a specific given time. The project contains six different force components all of which can be used mulltiple times in the system based on the desired approach.
* **Boids Force**  
The boids force, handled through the **BoidsForce** component, is developed to incorporate the dynamic forces between the agents of the population. These forces are: cohesion, separation and alignmenet and are all dependent on the agents' proximity with each other and the corresponding thresholds of activation for each force. Apart from the thresholds, the strength of each of the three boids forces can be separately set through this component.  

![Image](https://github.com/EleanaPol/multi-threaded-agents/blob/main/Documentation/Boids.PNG)
* **Attraction Force**  
This force is handled through the **AtrractionForce** component and is responsible for attracting the agents to the closest GameObject from a list of attractors. By switching the OVERALL STRENGTH of this component to a negative value the attractors switch and become repellers instead. But using multiple instances of this force, unique force variation can be developed in the system.  

![Image](https://github.com/EleanaPol/multi-threaded-agents/blob/main/Documentation/Attraction.PNG)  
* **Uniform Force**  
The **UniformForce** component is responsible for attaching a force of a selected uniform direction to all the agents of the system. This force is useful if you want to make the agents move towards a specific global direction (upwards for example).  

![Image](https://github.com/EleanaPol/multi-threaded-agents/blob/main/Documentation/Uniform.PNG)
* **Perlin Force**  
This force, handled through the **PerlinForce** component, is responsible for generating a curl noise vectorfield on the positions of the 3D voxel gris and in turn based on the relative positions of the agents apply the corresponding force to their movement. The curl noise can be customized by playing with the noise scale and noise speed parameters. The scale defines the resolution of the turbulence in the vector field, whereas the speed defines the movement of the curl field in space.  

![Image](https://github.com/EleanaPol/multi-threaded-agents/blob/main/Documentation/Perlin.PNG)
* **Stigmergy Gradient Force**  
The **StigmergyGradientForce** takes into account the positions in the environment where the agents have already travelled. Based on the scalar field of tracked agents, the gradient is calculated which is trying to pull the population towards those preferred (already travelled through) paths. The component takes as input references the **AgentPopulation** and **StigmergyManager** components of the system.  

![Image](https://github.com/EleanaPol/multi-threaded-agents/blob/main/Documentation/Stigmergy.PNG)
* **Attract To Mesh Force**  
This force, handled through the **AttractToMeshComponent** is responsible for attracting the agents towards a defined mesh geometry. The component takes as reference the systems **AgentEnvironment** and the mesh geometry towards which the agents will be attracted. We can vary the strength of the attraction based on the distance of the agent from the mesh at any given time through the max and min strength parameters. The closer the agent goes to the mesh the stronger the attraction will be (max strenth). If you wish a uniform attraction independent of distance, then both min and max values should be set to the same number. The attraction forces are calculated from the environment's voxel grid positions towards the corresponding closest mesh vertex. As this is a computationally heavy calculation it is only performed **once** at the start of the game. That means that even if the mesh is moved during rutime, the attraction will be only towards the position it originally had in the start of the game.  

![Image](https://github.com/EleanaPol/multi-threaded-agents/blob/main/Documentation/MeshAttract.PNG)
### Agent Population
The **AgentPopulation** component is responsible for generating a defined number of agents in a set region, that can either be a box collider(Generation Region) or the surface of a mesh object (Mesh Region). Additionally the component requires a reference to the system's **AgentEnvironment**. The Agent Population is responsible for applying the forces to the system's agents through the list of Forces. Every force can be named and enabled/disabled. The forces also require a refernce to the actual forces components. This is where you can manage the totality of forces that will affect the agent system throughout the simulation, in order for the simulation to run the **PLAY AGENTS** boolean should be enabled. Finally this component is responsible for rendering the agents, you can change the agent entity geometry through the Instance Mesh parameter.  

![Image](https://github.com/EleanaPol/multi-threaded-agents/blob/main/Documentation/Population.PNG)  

**ATTENTION** If you are generating the agents from a mesh surface, make sure the mesh is fully enclosed in the region of the Agent Enviroment box collider otherwise you will get compilation errors.
### Stigmergy Manager
This component is responsible of actually writting a value on the corresponding grid voxel based on each agent's location. The value can be defined through the chemical value parameter and can either stay on the grid forever or it can fade in time. This fading is defined through the chemical decay parameter. The higher the decay the longer the chemical lives. The component also requires references to the system's AgentPopulation and AgentEnvironment components.  

![Image](https://github.com/EleanaPol/multi-threaded-agents/blob/main/Documentation/StgmergyManager.PNG)
### Agent Meshing
The **AgentMesher** component is responsible for meshing the paths where the agent's have already travelled based on the values that are written on the voxel grid through the stigmergy manager. The gameobject that this component is attached to, also requires a **MeshFilter** and a **MeshRenderer** component. The MeshFilter will be dynamically filled with the procedural mesh that get generated whereas the material of that mesh can be defined through the mterial on the MeshRenderer. This component also holds a reference to the **ObjExporter** component that enables it to save and export as .obj files stills of the mesh that gets generated.  

![Image](https://github.com/EleanaPol/multi-threaded-agents/blob/main/Documentation/AgentMesher.PNG)
