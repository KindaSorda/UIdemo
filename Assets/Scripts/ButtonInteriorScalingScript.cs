using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonInteriorScalingScript : MonoBehaviour
{
    public GameObject interiorObject;
    public float scaleModifier;

    public GameObject glowEffect;

    RectTransform rt;
    Vector2 startingScale, targetScale;

    Button myButton;

    // Start is called before the first frame update
    void Start()
    {
        rt = interiorObject.GetComponent<RectTransform>();
        myButton = GetComponent<Button>();

        startingScale = rt.sizeDelta;
        targetScale = startingScale * scaleModifier;

        glowEffect.SetActive(false);
    }

    public void SetGlowEffect(bool hover)
    {
        glowEffect.SetActive(hover);
    }

    public void InteriorScaleOnHover(bool hover)
    {
        rt.sizeDelta = hover ? targetScale : startingScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (myButton.interactable == false)
            glowEffect.SetActive(false);
    }
}
