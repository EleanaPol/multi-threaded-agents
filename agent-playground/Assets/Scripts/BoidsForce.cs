using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidsForce : AgentForce
{
    [Header("Force Strengths")]
    public float cohesion_strength;
    public float separation_strength;
    public float alignment_strength;

    [Header("Force Thresholds")]
    public float cohesion_threshold;
    public float separation_threshold;
    public float alignment_threshold;

    [Header("Force Speeds")]
    public float cohesion_speed;
    public float separation_speed;
    public float alignment_speed;


    
}
