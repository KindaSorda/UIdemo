using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionWheelScript : MonoBehaviour
{
    public Button rotateUp, rotateDown;
    public float speed;

    Quaternion startingRot, targetRot;

    // Start is called before the first frame update
    void Start()
    {
        startingRot = transform.rotation;
        targetRot = startingRot;

        rotateUp.onClick.AddListener(() => TurnWheel(-30.0f));
        rotateDown.onClick.AddListener(() => TurnWheel(30.0f));
    }

    public void TurnWheel(float angle)
    {
        startingRot = targetRot;
        targetRot.eulerAngles = new Vector3(targetRot.eulerAngles.x, targetRot.eulerAngles.y, targetRot.eulerAngles.z + angle);
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, Time.deltaTime * speed);
    }
}
