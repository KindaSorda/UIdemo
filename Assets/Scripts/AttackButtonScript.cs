using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackButtonScript : MonoBehaviour
{
    public float shiftOffset;
    public float scaleMultiplier;

    public bool isUp = false;
    public bool isDown = false;
    public bool isHover = false;

    public int assignedAttackBreathCost;

    Vector3 targetPos, startingPos;

    public List<BreathUINode> targetBreathNodes = new List<BreathUINode>();
    public BattleCharacter assignedCharacter;

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
        targetPos = startingPos;

        Debug.Log(targetBreathNodes.Count);

        StartCoroutine(SetAssignedCharacter());
    }

    public void ScaleOnHover(bool up)
    {
        if (up)
            transform.localScale *= scaleMultiplier;
        else
            transform.localScale /= scaleMultiplier;
    }

    IEnumerator SetAssignedCharacter()
    {
        assignedCharacter = transform.parent.parent.GetComponent<CharacterUIFollowTarget>().target.gameObject.GetComponent<BattleCharacter>();
        //Debug.Log(assignedCharacter.gameObject.name);

        yield return new WaitForSeconds(GameManager.gm.variableSetDelay);
    }

    public void FlashBreathsOnHover(bool flashing)
    {
        for(int i = 0; i < targetBreathNodes.Count; i++)
    {
            if (i < (assignedAttackBreathCost + assignedCharacter.breathsSpentThisTurn))
                targetBreathNodes[i].Flash(flashing);
        }
    }

    void ShiftUp()
    {
        targetPos = startingPos * shiftOffset;
    }

    void ShiftDown()
    {
        targetPos = startingPos / shiftOffset;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime);
    }
}
