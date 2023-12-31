using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoulGuageScript : MonoBehaviour
{
    public int guageCap;
    public int currentFill = 0;

    //Image guageImage;

    public GameObject maxGuage;
    public float maxGuageAnimSpeedMultiplier;

    public Image treeLighting1;
    public RawImage fog1, fog2;

    public Gradient fogGradient, lightingGradient;

    Animator maxGuageAnim;

    // Start is called before the first frame update
    void Start()
    {
        //guageImage = GetComponent<Image>();
        //guageImage.fillAmount = 0;

        maxGuageAnim = maxGuage.GetComponent<Animator>();

        maxGuage.SetActive(false);

        fog1.color = fogGradient.Evaluate(0.0f);
        treeLighting1.color = lightingGradient.Evaluate(0.0f);

        GameManager.gm.soulGuageButton.interactable = false;
    }

    public void fillGuage(int amount)
    {
        currentFill += amount;
        //guageImage.fillAmount = (float)currentFill / (float)guageCap;
        fog1.color = fogGradient.Evaluate((float)currentFill / (float)guageCap);
        treeLighting1.color = lightingGradient.Evaluate((float)currentFill / (float)guageCap);

        if (currentFill >= guageCap)
            MaxGuage();
    }

    void MaxGuage()
    {
        maxGuage.SetActive(true);
        maxGuageAnim.speed *= maxGuageAnimSpeedMultiplier;
        GameManager.gm.soulGuageButton.interactable = true;
    }

    public void EmptyGuage()
    {
        currentFill = 0;
        fog1.color = fogGradient.Evaluate(0.0f);
        treeLighting1.color = lightingGradient.Evaluate(0.0f);
        maxGuage.SetActive(false);

        GameManager.gm.soulGuageButton.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
