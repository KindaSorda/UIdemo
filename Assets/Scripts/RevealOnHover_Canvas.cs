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

    public List<GameObject> mouseDistanceObjects = new List<GameObject>();

    public float mouseDistanceThresholdMod;

    private void Start()
    {
        cursorElement = GameManager.gm.cursorRevealObject;
        cursorText = cursorElement.transform.GetChild(1).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
    }

    public void Reveal(bool state)
    {
        //if (gameObject.GetComponent<StatusEffectIconControl>().active == true)
        //{
        Debug.Log("Set Reveal state " + state);

            if (!revealCursorElement)
                revealTarget.SetActive(state);
            else
            {
                cursorElement.SetActive(state);
                cursorText.text = addTextForCursorElement;
            }
        //}
    }
    void MouseDistanceCheck()
    {
        bool withinADistance = false;
        string text = "";

        for(int i = 0; i < mouseDistanceObjects.Count; i++)
        {
            if (mouseDistanceObjects[i].GetComponent<RevealTargetByMouseDistanceToThis>().viable)
            {
                float distance;
                distance = Vector2.Distance(mouseDistanceObjects[i].GetComponent<RectTransform>().position, Input.mousePosition);

                if (distance <= ((Screen.width - Screen.height) / mouseDistanceThresholdMod))
                {
                    mouseDistanceObjects[i].GetComponent<RevealTargetByMouseDistanceToThis>().inDistance = true;
                }
                else
                {
                    mouseDistanceObjects[i].GetComponent<RevealTargetByMouseDistanceToThis>().inDistance = false;
                }
            }
        }

        for (int i = 0; i < mouseDistanceObjects.Count; i++)
        {
            if (mouseDistanceObjects[i].GetComponent<RevealTargetByMouseDistanceToThis>().inDistance == true)
            {
                Debug.Log("Cursor within distance of " + mouseDistanceObjects[i].name);
                withinADistance = true;
                text = mouseDistanceObjects[i].GetComponent<RevealTargetByMouseDistanceToThis>().textForReveal;
            }
        }

        if (withinADistance)
        {
            addTextForCursorElement = text;
            Reveal(true);
        }
        else
            Reveal(false);
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
        MouseDistanceCheck();

        if (GameManager.gm.mouseOver == gameObject)
            Debug.Log("Mouse Over " + gameObject.name);
    }
}
