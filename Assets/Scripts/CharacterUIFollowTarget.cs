using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUIFollowTarget : MonoBehaviour
{
    [HideInInspector] public Transform target;
    RectTransform rt;

    // Start is called before the first frame update
    void Start()
    {
        rt = GetComponent<RectTransform>();
    }

    void FollowTarget()
    {
        rt.position = Camera.main.WorldToScreenPoint(target.position);
    }

    // Update is called once per frame
    void Update()
    {
        FollowTarget();
    }
}
