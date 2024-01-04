using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAbilitiesAndControl : MonoBehaviour
{
    BattleCharacter me;

    [Header("Claw Attack Variables")]
    public float animTime;
    public float endTurnDelay;
    public float damage;
    public bool isTargeted;
    public int setTarget;

    [Header("Basic Attack Variables")]
    bool isBasicAttacking = false;
    Vector3 basicAttackTargetPos;
    public float basicAttackAnimSpeed;
    public float startDelay, basicAttackAdvanceDelay, basicAttackReturnDelay;

    // Start is called before the first frame update
    void Start()
    {
        me = gameObject.GetComponent<BattleCharacter>();
    }

    IEnumerator BasicAttack()
    {
        me.isMyTurn = false;
        Vector3 firstPos = transform.position;

        Random.InitState((int)System.DateTime.Now.Ticks);
        int target = Random.Range(0, GameManager.gm.party.Count);

        Debug.Log("Enemy -> " + GameManager.gm.party[target].gameObject.name);

        yield return new WaitForSeconds(startDelay);

        basicAttackTargetPos = GameManager.gm.party[target].gameObject.transform.position;
        isBasicAttacking = true;
        yield return new WaitForSeconds(basicAttackAdvanceDelay);
        basicAttackTargetPos = firstPos;
        GameManager.gm.CameraShakeEnemy();
        yield return new WaitForSeconds(basicAttackReturnDelay);
        isBasicAttacking = false;
        transform.position = firstPos;

        StartCoroutine(GameManager.gm.EndTurn(endTurnDelay));
    }

    IEnumerator ClawAttack()
    {
        me.isMyTurn = false;

        Random.InitState((int)System.DateTime.Now.Ticks);

        BattleCharacter attackTarget;

        if(isTargeted)
        {
            attackTarget = GameManager.gm.party[setTarget];
        }
        else
        {
            int randTarget = Random.Range(0, GameManager.gm.party.Count);
            attackTarget = GameManager.gm.party[randTarget];
        }

        yield return new WaitForSeconds(startDelay);

        GameObject attackAnim = Instantiate(Resources.Load<GameObject>("Prefabs/AttackAnims/AttackAnim_Claw"), GameManager.gm.mainCombatUI.transform);
        attackAnim.transform.SetSiblingIndex(1);

        yield return new WaitForSeconds(animTime);

        Destroy(attackAnim);
        StartCoroutine(attackTarget.TakeDamage(damage));
        GameManager.gm.CameraShakePlayer();

        StartCoroutine(GameManager.gm.EndTurn(endTurnDelay));
    }

    // Update is called once per frame
    void Update()
    {
        if (me.isMyTurn == true)
            StartCoroutine(ClawAttack());

        if (isBasicAttacking == true)
            transform.position = Vector3.Lerp(transform.position, basicAttackTargetPos, Time.deltaTime * basicAttackAnimSpeed);
    }
}
