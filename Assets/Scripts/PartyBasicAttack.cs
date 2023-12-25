using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartyBasicAttack : MonoBehaviour
{

    bool isBasicAttacking = false;

    public string attackName;
    [TextArea(3, 5)]public string description;
    public float basicAttackAnimSpeed;
    public float basicAttackAdvanceDelay, basicAttackReturnDelay;
    BattleCharacter thisCharacter;
    public int basicAttackBreathCost;

    Vector3 basicAttackTargetPos;

    public int assignedButton;

    bool thisAttackActive = false;

    // Start is called before the first frame update
    void Start()
    {
        thisCharacter = gameObject.GetComponent<BattleCharacter>();
        StartCoroutine(AssignToButton(assignedButton));
    }

    IEnumerator AssignToButton(int assignToButton)
    {
        yield return new WaitForSeconds(0.1f);
        Button targetButton = thisCharacter.myAttackButtons[assignToButton].gameObject.GetComponent<Button>();

        targetButton.onClick.AddListener(() => PrepareAttack());
        targetButton.GetComponent<AttackButtonScript>().assignedAttackBreathCost = basicAttackBreathCost;
        targetButton.GetComponent<AttackButtonScript>().assignedAttackDescription = description;
        targetButton.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = attackName;

        //Debug.Log("Assigned basic attack for " + gameObject.name + " to Button " + targetButton.transform.parent.parent.parent.name);
    }

    void PrepareAttack()
    {
        Debug.Log("Prepared Basic Attack From " + gameObject.name);

        GameManager.gm.targetingModeSingleEnemy = true;
        thisAttackActive = true;
        GameManager.gm.SetTargetingReticle(true);
    }

    void EngageAttack()
    {
        Debug.Log("Engaged Basic Attack From " + gameObject.name);

        if (Input.GetMouseButton(0))
        {
            if(GameManager.gm.mouseOver != null && GameManager.gm.mouseOver.tag == "Enemy")
            {
                //Debug.Log("Party -> " + GameManager.gm.mouseOver.name);
                StartCoroutine(ExeceuteAttack(GameManager.gm.mouseOver.transform));
                GameManager.gm.targetingModeSingleEnemy = false;
                thisAttackActive = false;
                GameManager.gm.SetTargetingReticle(false);
                //GameManager.gm.EndTurn();
            }
        }
        else if (Input.GetMouseButton(1))
        {
            GameManager.gm.targetingModeSingleEnemy = false;
            thisAttackActive = false;
            GameManager.gm.SetTargetingReticle(false);
        }
    }

    IEnumerator ExeceuteAttack(Transform target)
    {
        Debug.Log("Execute Basic Attack From " + gameObject.name);

        Vector3 firstPos = transform.position;

        //GameManager.gm.SetUIState(false);

        basicAttackTargetPos = new Vector3(target.position.x, transform.position.y, target.position.z);
        isBasicAttacking = true;
        yield return new WaitForSeconds(basicAttackAdvanceDelay);
        basicAttackTargetPos = firstPos;
        GameManager.gm.CameraShakePlayer();
        yield return new WaitForSeconds(basicAttackReturnDelay);
        isBasicAttacking = false;
        transform.position = firstPos;

        //GameManager.gm.SetUIState(true);

        thisCharacter.StartCoroutine(thisCharacter.SpendBreaths(basicAttackBreathCost, 0.1f));
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.gm.targetingModeSingleEnemy && thisAttackActive && GameManager.gm.currentTurnCharacter == thisCharacter)
        {
            EngageAttack();
        }

        if (isBasicAttacking == true)
            transform.position = Vector3.Lerp(transform.position, basicAttackTargetPos, Time.deltaTime * basicAttackAnimSpeed);
    }
}
