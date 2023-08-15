using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUIFollowTarget : MonoBehaviour
{
    [HideInInspector] public Transform target;
    RectTransform rt;

    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rt = GetComponent<RectTransform>();
        anim = GetComponent<Animator>();
    }

    void FollowTarget()
    {
        rt.position = Camera.main.WorldToScreenPoint(target.position);
    }

    public void ScaleOnHover(bool hover)
    {
        anim.SetBool("isHover", hover);
    }

    public void ScaleUpOnHover()
    {
        if(GameManager.gm.mouseOver == target.gameObject)
        {
            //Debug.Log("Triggered Scale Up");
            anim.SetBool("isHover", true);
        }
    }

    public void ScaleDownOnHover()
    {
        anim.SetBool("isHover", false);
    }

    // Update is called once per frame
    void Update()
    {
        //ScaleUpOnHover();
        FollowTarget();
    }
}
