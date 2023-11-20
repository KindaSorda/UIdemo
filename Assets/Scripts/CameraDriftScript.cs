using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDriftScript : MonoBehaviour
{
    Vector2 mousePos;
    Vector2 screenCenter;
    float distanceX, distanceY;
    public float multiplier;
    float startingXRot, startingYRot;

    // Start is called before the first frame update
    void Start()
    {
        screenCenter = new Vector2(Screen.width / 2.0f, Screen.height / 2.0f);
        startingXRot = gameObject.transform.localEulerAngles.x;
        startingYRot = gameObject.transform.localEulerAngles.y;
    }

    void CameraDrift()
    {
        gameObject.transform.localEulerAngles = new Vector3
            (startingXRot + (distanceY * multiplier),
             startingYRot - (distanceX * multiplier),
             gameObject.transform.localEulerAngles.z);
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = Input.mousePosition;
        distanceX = screenCenter.x - mousePos.x;
        distanceY = screenCenter.y - mousePos.y;

        if(mousePos.x >= 0 && mousePos.y >= 0 && mousePos.x <= Screen.width && mousePos.y <= Screen.height)
            CameraDrift();
    }
}
