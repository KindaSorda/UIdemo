using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreathUINode : MonoBehaviour
{
    public float breathInstantiationRotOffset;
    public float movementSpeed;
    public float distanceToFillGuage;

    public bool activeBreath = true;
    public bool spentBreath = false;
    public GameObject icon;

    Animator anim;

    GameObject soulGuage;
    GameObject breathIcon;

    RectTransform iconRT, soulGuageRT;
    Vector2 startingPos, targetPos;

    Transform newParent;

    // Start is called before the first frame update
    void Start()
    {
        soulGuage = GameObject.FindGameObjectWithTag("SoulGuage");
        breathIcon = transform.GetChild(1).gameObject;
        anim = breathIcon.GetComponent<Animator>();

        iconRT = breathIcon.GetComponent<RectTransform>();
        soulGuageRT = soulGuage.GetComponent<RectTransform>();

        startingPos = iconRT.localPosition;

        newParent = GameObject.Find("BreathIconHolderForMovement").transform;
    }

    public void SetRotAtStart()
    {
        icon.GetComponent<RectTransform>().transform.eulerAngles = Vector3.zero;
    }

    public void Flash(bool flashing)
    {
        anim.SetBool("Flashing", flashing);
    }

    public void SpendBreath()
    {
        //breathIcon.transform.SetParent(newParent);
        targetPos = soulGuageRT.position;
        spentBreath = true;
    }

    public void RefillBreath()
    {
        Debug.Log("Refilled Breath");
        spentBreath = false;
        breathIcon.transform.SetParent(transform);
        iconRT.localPosition = startingPos;
        anim.SetBool("Spent", false);
        activeBreath = true;
    }

    void fillGuageWithBreath()
    {
        soulGuage.GetComponent<SoulGuageScript>().fillGuage(1);
        anim.SetBool("Spent", true);
        activeBreath = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Flash();
        //breathIcon.GetComponent<Image>().enabled = activeBreath;

        if (spentBreath == true)
        {
            iconRT.position = Vector2.Lerp(iconRT.position, targetPos, Time.deltaTime * movementSpeed);
            if (Vector2.Distance(iconRT.position, soulGuageRT.position) <= distanceToFillGuage && activeBreath == true)
                fillGuageWithBreath();
        }
    }
}
