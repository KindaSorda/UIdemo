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

    public string attackName;
    [TextArea(3, 5)]public string description;
    public float basicAttackAnimSpeed;
    public float basicAttackAdvanceDelay, basicAttackReturnDelay;
    BattleCharacter thisCharacter;
    public int basicAttackBreathCost;

    Vector3 basicAttackTargetPos;

    public int assignedButton;

    // Start is called before the first frame update
    void Start()
    {
        thisCharacter = gameObject.GetComponent<BattleCharacter>();
        StartCoroutine(AssignToButton(assignedButton));
    }

    IEnumerator AssignToButton(int assignToButton)
    {
        yield return new WaitForSeconds(0.1f);
        Button targetButton = gameObject.GetComponent<BattleCharacter>().myAttackButtons[assignToButton].gameObject.GetComponent<Button>();

        targetButton.onClick.AddListener(() => PrepareAttack());
        targetButton.GetComponent<AttackButtonScript>().assignedAttackBreathCost = basicAttackBreathCost;
        targetButton.GetComponent<AttackButtonScript>().assignedAttackDescription = description;
        targetButton.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = attackName;
    }

    void PrepareAttack()
    {
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
        if (targetingMode)
        {
            EngageAttack();

            if (GameManager.gm.mouseOver != null && GameManager.gm.mouseOver.tag == "Enemy")
            {
                GameManager.gm.SetTargetingLineToTarget(0, thisCharacter.transform, GameManager.gm.mouseOver.transform);
            }
            else
                GameManager.gm.DisableTargetingLine(0);
        }

        if (isBasicAttacking == true)
            transform.position = Vector3.Lerp(transform.position, basicAttackTargetPos, Time.deltaTime * basicAttackAnimSpeed);
    }
}
