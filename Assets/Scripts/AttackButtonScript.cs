using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackButtonScript : MonoBehaviour
{
    public float shiftOffset;
    public float scaleMultiplier;

    public bool isUp = false;
    public bool isDown = false;
    public bool isHover = false;

    Vector3 targetPos, startingPos;

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
        targetPos = startingPos;
    }

    public void ScaleOnHover(bool up)
    {
        if(up)
            transform.localScale *= scaleMultiplier;
        else
            transform.localScale /= scaleMultiplier;
    }

    void ShiftUp()
    {
        targetPos = startingPos * shiftOffset;
    }

    void ShiftDown()
    {
        targetPos = startingPos / shiftOffset;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime);
    }
}
