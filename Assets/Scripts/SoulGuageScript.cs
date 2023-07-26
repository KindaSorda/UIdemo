using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoulGuageScript : MonoBehaviour
{
    public int guageCap;
    public int currentFill = 0;

    Image guageImage;

    // Start is called before the first frame update
    void Start()
    {
        guageImage = GetComponent<Image>();
        guageImage.fillAmount = 0;
    }

    public void fillGuage(int amount)
    {
        currentFill += amount;
        guageImage.fillAmount = (float)currentFill / (float)guageCap;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
