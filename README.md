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
