using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    public float rotation_Speed;
    float rot;
    // Start is called before the first frame update
    void Start()
    {
        rot = 0; 
    }

    // Update is called once per frame
    void Update()
    {
        rot += Time.deltaTime * rotation_Speed;

        var angle = new Vector3(0, rot, 0);
        transform.localEulerAngles = angle;
    }
}
