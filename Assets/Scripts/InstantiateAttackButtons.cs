using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InstantiateAttackButtons : MonoBehaviour
{
    public int numAttacks;
    public float attackButtonInstantiationXOffset, attackButtonInstantianRotationOffset;
    public List<GameObject> attackButtons = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {

    }

    public void InstantiateButtons()
    {
        GameObject attackButtonParent = gameObject.GetComponent<BattleCharacter>().uiParent.transform.GetChild(1).gameObject;

        attackButtonParent.transform.eulerAngles = new Vector3
            (attackButtonParent.transform.eulerAngles.x,
             attackButtonParent.transform.eulerAngles.y,
             attackButtonParent.transform.eulerAngles.z + attackButtonInstantianRotationOffset * ((numAttacks / 2.0f)));

        for (int i = 0; i < numAttacks; i++)
        {
            GameObject newButton = Instantiate(Resources.Load("Prefabs/AttackButton") as GameObject, attackButtonParent.transform);
            newButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ("Attack " + i);
            attackButtonParent.transform.eulerAngles = new Vector3(attackButtonParent.transform.eulerAngles.x, attackButtonParent.transform.eulerAngles.y, attackButtonParent.transform.eulerAngles.z - (attackButtonInstantianRotationOffset));
            newButton.transform.localPosition = new Vector3(newButton.transform.localPosition.x + (attackButtonInstantiationXOffset), newButton.transform.localPosition.y, newButton.transform.localPosition.z);
            newButton.transform.SetParent(transform.GetChild(0));
            newButton.transform.eulerAngles = Vector3.zero;
            newButton.GetComponent<AttackButtonScript>().targetBreathNodes = gameObject.GetComponent<BattleCharacter>().breathNodes;
            attackButtons.Add(newButton);
        }

        float firstY = attackButtons[attackButtons.Count - 1].transform.position.y;
        float lastY = attackButtons[0].transform.position.y;

        float lastYAltered = lastY - firstY;

        for (int i = 0; i < attackButtons.Count; i++)
        {
            int mult = attackButtons.Count - i - 1;

            attackButtons[i].transform.position = new Vector3(attackButtons[i].transform.position.x, ((lastYAltered / numAttacks) * (mult)) + firstY, attackButtons[i].transform.position.z);

            attackButtons[i].transform.SetParent(attackButtonParent.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
