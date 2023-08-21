using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUIFollowTarget : MonoBehaviour
{
    [HideInInspector] public Transform target;
    RectTransform rt;

    Animator anim;

    public RawImage[] bubbleImages;
    [Range(0.0f,1.0f)]public float onHoverAlpha, offHoverAlpha;

    // Start is called before the first frame update
    void Start()
    {
        rt = GetComponent<RectTransform>();
        anim = GetComponent<Animator>();

        for (int i = 0; i < bubbleImages.Length; i++)
        {
            bubbleImages[i].color = new Color(bubbleImages[i].color.r, bubbleImages[i].color.g, bubbleImages[i].color.b, offHoverAlpha);
        }
    }

    void FollowTarget()
    {
        rt.position = Camera.main.WorldToScreenPoint(target.position);
    }

    public void ScaleOnHover(bool hover)
    {
        anim.SetBool("isHover", hover);
        for (int i = 0; i < bubbleImages.Length; i++)
        {
            bubbleImages[i].color = new Color(bubbleImages[i].color.r, bubbleImages[i].color.g, bubbleImages[i].color.b, hover == true ? onHoverAlpha : offHoverAlpha);
        }
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
