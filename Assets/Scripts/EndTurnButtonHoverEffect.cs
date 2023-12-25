using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndTurnButtonHoverEffect : MonoBehaviour
{
    public GameObject thumbnailObject;
    public Image glowEffect;
    Button myButton;
    Animator thumbnailAnim;

    // Start is called before the first frame update
    void Start()
    {
        myButton = GetComponent<Button>();
        thumbnailAnim = thumbnailObject.GetComponent<Animator>();

        glowEffect.enabled = false;
    }

    public void OnHover(bool state)
    {
        glowEffect.enabled = state;
        thumbnailAnim.SetBool("Hover", state);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
