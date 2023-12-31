using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealTargetByMouseDistanceToThis : MonoBehaviour
{
    public bool inDistance = false;

    public string textForReveal;

    public bool viable = true;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.gm.GetComponent<RevealOnHover_Canvas>().mouseDistanceObjects.Add(gameObject);
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
