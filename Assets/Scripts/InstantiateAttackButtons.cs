using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InstantiateAttackButtons : MonoBehaviour
{
    [Range(0,6)]public int numAttacks;

    GameObject attackButtonSet;

    BattleCharacter myCharacter;

    // Start is called before the first frame update
    void Start()
    {
        myCharacter = gameObject.GetComponent<BattleCharacter>();
    }

    public void InstantiateButtons()
    {
        Debug.Log("Called Instantiate Buttons");

        attackButtonSet = Instantiate(Resources.Load<GameObject>("Prefabs/SetOfAttackButtons"), GameManager.gm.InstantiateAttackButtonsPos.position, GameManager.gm.InstantiateAttackButtonsPos.rotation, GameManager.gm.mainCombatUI.transform);
        myCharacter.attackButtonsParent = attackButtonSet;
        
        for(int i = 0; i < 6; i++)
        {
            myCharacter.myAttackButtons.Add(attackButtonSet.transform.GetChild(0).GetChild(i).GetChild(0).gameObject.GetComponent<AttackButtonScript>());
            Debug.Log("Added " + attackButtonSet.transform.GetChild(0).GetChild(i).GetChild(0).gameObject.name);

            myCharacter.myAttackButtons[i].assignedCharacter = myCharacter;
            myCharacter.myAttackButtons[i].targetBreathNodes = myCharacter.breathNodes;

            if (i > numAttacks - 1)
                myCharacter.myAttackButtons[i].gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
