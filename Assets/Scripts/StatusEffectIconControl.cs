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
    public bool active = false;

    public float mouseDistanceThreshold;
    CharacterUIControlScript myUiParent;
    RevealOnHover_Canvas revealScript;
    RectTransform rt;

    // Start is called before the first frame update
    void Start()
    {
        displayIcon = GetComponent<Image>();
        stackDisplay.text = "";

        myUiParent = transform.parent.parent.parent.gameObject.GetComponent<CharacterUIControlScript>();
        revealScript = GetComponent<RevealOnHover_Canvas>();
        rt = GetComponent<RectTransform>();
    }

    public void AssignEffect(SO_StatusEffect effect)
    {
        active = true;
        assigned = effect;
        displayIcon.sprite = effect.thumbnailSprite;
        descriptionTextBox.text = effect.description;

        if (assigned.stacks < 2)
            stackDisplay.text = "";
        else
            stackDisplay.text = assigned.stacks.ToString();
    }
    
    public void EmptyOnStart()
    {
        //Debug.Log("Called EmptyOnStart()");
        GetComponent<Image>().sprite = Resources.Load<Sprite>("Empty");
        active = false;
    }

    public void Empty()
    {
        Debug.Log("Called Empty()");
        displayIcon.sprite = Resources.Load<Sprite>("Empty");
        active = false;
    }

    void MouseDistanceCheck()
    {
        float distance;
        distance = Vector2.Distance(rt.position, Input.mousePosition);

        if (distance <= mouseDistanceThreshold)
        {
            Debug.Log("Triggered Mouse Distance Check On Status Effect");
            Debug.Log("Target to scale: " + myUiParent.name);
            myUiParent.scaleUpFromStatusIcon = true;
            revealScript.addTextForCursorElement = assigned.description;
        }
        else
            myUiParent.scaleUpFromStatusIcon = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
            MouseDistanceCheck();

        /*if (assigned != null) {
            if (assigned.stacks < 2)
                stackDisplay.text = "";
            else
                stackDisplay.text = assigned.stacks.ToString();
        }*/
    }
}
