using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartyBasicAttack : MonoBehaviour
{
    List<GameObject> attackButtons = new List<GameObject>();
    bool targetingMode = false;
    bool isBasicAttacking = false;
    public float basicAttackAnimSpeed;
    public float basicAttackAdvanceDelay, basicAttackReturnDelay;
    BattleCharacter thisCharacter;
    public int basicAttackBreathCost;

    Vector3 basicAttackTargetPos;

    // Start is called before the first frame update
    void Start()
    {
        thisCharacter = gameObject.GetComponent<BattleCharacter>();
        StartCoroutine(AssignToButton());
    }

    IEnumerator AssignToButton()
    {
        yield return new WaitForSeconds(0.1f);

        attackButtons = GetComponent<InstantiateAttackButtons>().attackButtons;

        for (int i = 0; i < attackButtons.Count; i++)
        {
            //attackButtons[i].GetComponent<Button>().onClick.AddListener(() => PrepareAttack(i));
            attackButtons[i].GetComponent<AttackButtonScript>().assignedAttackBreathCost = basicAttackBreathCost;
        }
    }

    void PrepareAttack(int num)
    {
        Debug.Log("Initiate Attack " + num);

        targetingMode = true;
        GameManager.gm.SetTargetingReticle(true);
    }

    void EngageAttack()
    {
        if (Input.GetMouseButton(0))
        {
            if(GameManager.gm.mouseOver != null && GameManager.gm.mouseOver.tag == "Enemy")
            {
                //Debug.Log("Party -> " + GameManager.gm.mouseOver.name);
                StartCoroutine(ExeceuteAttack(GameManager.gm.mouseOver.transform));
                targetingMode = false;
                GameManager.gm.SetTargetingReticle(false);
                //GameManager.gm.EndTurn();
            }
        }
        else if (Input.GetMouseButton(1))
        {
            targetingMode = false;
            GameManager.gm.SetTargetingReticle(false);
        }
    }

    IEnumerator ExeceuteAttack(Transform target)
    {
        //thisCharacter.uiParent.SetActive(false);

        Vector3 firstPos = transform.position;

        thisCharacter.StartCoroutine(thisCharacter.SpendBreaths(basicAttackBreathCost, basicAttackAdvanceDelay + basicAttackReturnDelay));

        basicAttackTargetPos = target.position;
        isBasicAttacking = true;
        yield return new WaitForSeconds(basicAttackAdvanceDelay);
        basicAttackTargetPos = firstPos;
        yield return new WaitForSeconds(basicAttackReturnDelay);
        isBasicAttacking = false;
        transform.position = firstPos;

        thisCharacter.uiParent.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (targetingMode)
            EngageAttack();

        if (isBasicAttacking == true)
            transform.position = Vector3.Lerp(transform.position, basicAttackTargetPos, Time.deltaTime * basicAttackAnimSpeed);
    }
}
