using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AttackScript_Hellfire : MonoBehaviour
{
    public string attackName;
    [TextArea(3, 5)] public string description;

    BattleCharacter thisCharacter;
    public float damage;
    public int breathCost;

    public float animTime;
    public float effectDelay;

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
        //Debug.Log("Assigne Overload to Button");

        yield return new WaitForSeconds(0.1f);
        Button targetButton = GameManager.gm.soulGuageButton;

        targetButton.onClick.AddListener(() => PrepareAttack());
        //targetButton.GetComponent<AttackButtonScript>().assignedAttackBreathCost = breathCost;
        //targetButton.GetComponent<AttackButtonScript>().assignedAttackDescription = description;
        //targetButton.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = attackName;
    }

    void PrepareAttack()
    {
        Debug.Log("Preparing Hellfire");
        GameManager.gm.targetingModeAllEnemies = true;
        thisAttackActive = true;
        GameManager.gm.SetTargetingReticle(true);
    }

    void EngageAttack()
    {
        if (Input.GetMouseButton(0))
        {
            if (GameManager.gm.mouseOver != null && GameManager.gm.mouseOver.tag == "Enemy")
            {
                //Debug.Log("Party -> " + GameManager.gm.mouseOver.name);
                Debug.Log("Successful Click on Hellfire");
                StartCoroutine(ExecuteAttack());
                GameManager.gm.targetingModeAllEnemies = false;
                thisAttackActive = false;

                GameManager.gm.DisableTargetingLine(0);
                GameManager.gm.DisableTargetingLine(1);
                GameManager.gm.DisableTargetingLine(2);

                GameManager.gm.SetTargetingReticle(false);
                //GameManager.gm.EndTurn();
            }
        }
        else if (Input.GetMouseButton(1))
        {
            GameManager.gm.targetingModeAllEnemies = false;
            thisAttackActive = false;
            GameManager.gm.SetTargetingReticle(false);
        }
    }

    IEnumerator ExecuteAttack()
    {
        Debug.Log("Execute Attack Hellfire");

        GameObject attackAnim = Instantiate(Resources.Load<GameObject>("Prefabs/AttackAnims/AttackAnim_Hellfire"), GameManager.gm.comicBordersUI.transform);
        attackAnim.transform.SetAsLastSibling();

        yield return new WaitForSeconds(animTime);
        Destroy(attackAnim);

        yield return new WaitForSeconds(effectDelay);
        GameManager.gm.CameraShakePlayer();
        for (int i = 0; i < GameManager.gm.enemies.Count; i++)
        {
            StartCoroutine(GameManager.gm.enemies[i].TakeDamage(damage));

            SO_StatusEffect newEffect = Instantiate(Resources.Load("StatusEffects/Deccelerate") as SO_StatusEffect);
            GameManager.gm.enemies[i].InflictStatusEffect(newEffect);
        }

        thisCharacter.StartCoroutine(thisCharacter.SpendBreaths(breathCost, 0.1f));
        GameManager.gm.soulGuage.EmptyGuage();

        yield return new WaitForSeconds(1.0f);
        GameManager.gm.VictoryState();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.gm.targetingModeAllEnemies && thisAttackActive && GameManager.gm.currentTurnCharacter == thisCharacter)
        {
            EngageAttack();
        }
    }
}
