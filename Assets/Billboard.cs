using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera targetCam;

    // Start is called before the first frame update
    void Start()
    {
        targetCam = Camera.main;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.rotation = targetCam.transform.rotation;
    }
}
