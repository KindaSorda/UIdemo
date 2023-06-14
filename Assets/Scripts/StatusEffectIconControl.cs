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

    // Start is called before the first frame update
    void Start()
    {
        displayIcon = GetComponent<Image>();
        stackDisplay.text = "";
    }

    public void AssignEffect(SO_StatusEffect effect)
    {
        assigned = effect;
        displayIcon.sprite = effect.thumbnailSprite;

        if (assigned.stacks < 2)
            stackDisplay.text = "";
        else
            stackDisplay.text = assigned.stacks.ToString();
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
