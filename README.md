# multi-threaded-agents

## Description
This Unity project manages CPU multithreaded autonomous agents vis Unity's [Job System](https://docs.unity3d.com/Manual/JobSystem.html). The agent system incorporates a variety of forces, some of which are agent-centric and others environnment-centric. To optimize the overall performance, a simple spatial binning system has been developed and the agent entities are drawn directly in graphics, through GPU instancing. The agent environment is expressed as a behind the scenes 3D voxel grid of customizable resolution. The voxel grid enables the agents' stigmergic behaviours as well as the meshing of the agents' paths through a [Marching Cubes](https://en.wikipedia.org/wiki/Marching_cubes) algorithm. The meshed paths are also drawn directly in the GPU but there is the option to export selected mesh "frames" in .obj file format. 

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
The **StigmergyGradientForce** takes into account the positions in the environment where the agents have already travelled. Based on the scalar field of tracked agents, the gradient is calculated which is trying to pull the population towards those preferred (already travelled through) paths. 
