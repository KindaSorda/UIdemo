using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackButtonScript : MonoBehaviour
{
    public int assignedAttackBreathCost;
    public string assignedAttackDescription;

    Vector3 targetPos, startingPos;
    Vector3 targetScale, startingScale;
    Transform onHoverParent, offHoverParent;
    public float verticalShiftOffset;
    public float scaleUpOffset;

    public List<BreathUINode> targetBreathNodes = new List<BreathUINode>();
    public BattleCharacter assignedCharacter;

    RectTransform rt;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rt = GetComponent<RectTransform>();
        anim = GetComponent<Animator>();

        startingPos = rt.localPosition;
        targetPos = new Vector2(rt.localPosition.x, rt.localPosition.y + verticalShiftOffset);
        startingScale = transform.parent.localScale;
        targetScale = startingScale * scaleUpOffset;
        offHoverParent = transform.parent.parent;
        onHoverParent = offHoverParent.parent;
        //Debug.Log(targetBreathNodes.Count);
    }

    public void OnHover(bool state)
    {
        Debug.Log("Hovering over Attack Button of " + assignedCharacter.name);

        anim.SetBool("isHover", state);

        if(state == true)
        {
            FlashBreathsOnHover(state);
            ScaleOnHover(state);
            //SetParentOnHover(state);
            //ShiftAllButtonsAbove(state);
        }
        else
        {
            FlashBreathsOnHover(state);
            //SetParentOnHover(state);
            ScaleOnHover(state);
            //ShiftAllButtonsAbove(state);
        }
    }

    void FlashBreathsOnHover(bool flashing)
    {
        Debug.Log("Trigger FlashBreaths");

        for(int i = 0; i < targetBreathNodes.Count; i++)
        {
            if (i < (assignedAttackBreathCost + assignedCharacter.breathsSpentThisTurn))
                targetBreathNodes[i].Flash(flashing);
        }
    }

    void ScaleOnHover(bool hover)
    {
        transform.parent.localScale = hover == true ? targetScale : startingScale;
    }

    void SetParentOnHover(bool hover)
    {
        transform.parent.SetParent(hover == true ? onHoverParent : offHoverParent, true);
    }

    void ShiftAllButtonsAbove(bool hover)
    {
        AttackButtonScript me;
        int myPositionInArray = 0;

        for(int i = 0; i < assignedCharacter.myAttackButtons.Count; i++)
        {
            if (assignedCharacter.myAttackButtons[i] == this)
                myPositionInArray = i;
        }

        for (int i = 0; i < assignedCharacter.myAttackButtons.Count; i++)
        {
            if (i > myPositionInArray)
            {
                assignedCharacter.myAttackButtons[i].ShiftVerticalOnHover(hover);
            }
        }
    }

    void ShiftVerticalOnHover(bool hover)
    {
        rt.localPosition = hover == true ? targetPos : startingPos;
    }

// Update is called once per frames
    void Update()
    {
        //transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime);
    }
}
