using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathUINode : MonoBehaviour
{
    public bool activeBreath = true;
    public GameObject icon;
    Animator anim;
    public bool flashing = false;
    
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Flash()
    {
        anim.SetBool("Flashing", flashing);
    }

    // Update is called once per frame
    void Update()
    {
        Flash();
        icon.SetActive(activeBreath);
    }
}
