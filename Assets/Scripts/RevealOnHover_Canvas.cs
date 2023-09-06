using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RevealOnHover_Canvas : MonoBehaviour
{
    public TextMeshProUGUI getTextForCursorElement;
    public string addTextForCursorElement;

    public GameObject revealTarget;

    public bool revealCursorElement;

    GameObject cursorElement;
    TextMeshProUGUI cursorText;

    private void Start()
    {
        cursorElement = GameManager.gm.cursorRevealObject;
        cursorText = cursorElement.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
    }

    public void Reveal(bool state)
    {
        if (gameObject.GetComponent<StatusEffectIconControl>().hoverable == true)
        {
            if (!revealCursorElement)
                revealTarget.SetActive(state);
            else
            {
                cursorElement.SetActive(state);
                cursorText.text = addTextForCursorElement;
            }
        }
    }

    public void RevealHealthText(bool state)
    {
        float startingHealthValue = transform.parent.parent.parent.GetComponent<CharacterUIControlScript>().target.GetComponent<BattleCharacter>().startingHealth;
        float healthValue = transform.parent.parent.parent.GetComponent<CharacterUIControlScript>().target.GetComponent<BattleCharacter>().health;

        cursorText.text = transform.parent.parent.parent.GetComponent<CharacterUIControlScript>().target.GetComponent<BattleCharacter>().gameObject.name + ": " + healthValue + " / " + startingHealthValue;
        cursorElement.SetActive(state);
    }

    private void Update()
    {
        if (GameManager.gm.mouseOver == gameObject)
            Debug.Log("Mouse Over " + gameObject.name);
    }
}
