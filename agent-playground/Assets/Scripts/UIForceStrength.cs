using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIForceStrength : MonoBehaviour
{
    public Slider slider;
    public TMP_Text title;
    private AgentForce a_force;

    // Start is called before the first frame update
    void Start()
    {
        a_force = transform.GetComponent<AgentForce>();
        InitSlider();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitSlider()
    {
        slider.value = a_force.OVERALL_STRENGTH;
        title.text = a_force.name;
    }

    public void AssignForceStrength(float strength)
    {
        a_force.OVERALL_STRENGTH = strength;
    }
    
}
