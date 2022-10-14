using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageDetector : MonoBehaviour
{
    public GameObject augmented;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PrintMessage()
    {
        Debug.Log($"Augmented pos {augmented.transform.position}");
    }
}
