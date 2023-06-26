using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusEffectIconControl : MonoBehaviour
{
    public TextMeshProUGUI stackDisplay;
    [HideInInspector] public Image displayIcon;
    [HideInInspector] public SO_StatusEffect assigned;
    public TextMeshProUGUI descriptionTextBox;
    RevealOnHover myHoverScript;

    // Start is called before the first frame update
    void Start()
    {
        displayIcon = GetComponent<Image>();
        stackDisplay.text = "";

        myHoverScript = GetComponent<RevealOnHover>();
    }

    public void AssignEffect(SO_StatusEffect effect)
    {
        myHoverScript.enabled = true;
        assigned = effect;
        displayIcon.sprite = effect.thumbnailSprite;
        descriptionTextBox.text = effect.description;

        if (assigned.stacks < 2)
            stackDisplay.text = "";
        else
            stackDisplay.text = assigned.stacks.ToString();
    }
    
    public void Empty()
    {
        displayIcon.sprite = Resources.Load<Sprite>("Empty");
        myHoverScript.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        /*if (assigned != null) {
            if (assigned.stacks < 2)
                stackDisplay.text = "";
            else
                stackDisplay.text = assigned.stacks.ToString();
        }*/
    }
}
