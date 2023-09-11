using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager gm;

    public GameObject mainCombatUI;
    public GameObject cursorRevealObject;

    public GameObject currentTurnIndicatorWorldSpace;
    public float currentTurnIndicatorWorldSpaceMovementSpeed;
    Quaternion rotForParty, rotForEnemy;
    Vector3 currentTurnIndicatorWorldSpaceTargetPos;
    Quaternion currentTurnIndicatorWorldSpaceTargetRot;

    public int numPartyMembersInScene;

    public List<GameObject> characters = new List<GameObject>();
    public List<BattleCharacter> party = new List<BattleCharacter>();
    public List<BattleCharacter> enemies = new List<BattleCharacter>();
    public List<Image> turnIndicators = new List<Image>();

    public int originalUIOrder = -2;
    public int currentTurnUIOrder = 1;

    public Button nextTurnButton;
    public float turnIndicatorUpdateSpeed;
    public float turnIndicatorPosMultiplier;
    public float currentTurnIndicatorX;

    public BattleCharacter currentTurnCharacter;

    LayerMask combatInteractable;

    [HideInInspector]public GameObject mouseOver;

    public GameObject targetingMouseReticle;

    public float variableSetDelay;

    public Transform InstantiateAttackButtonsPos;

    private void Awake()
    {
        gm = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        nextTurnButton.onClick.AddListener(() => StartCoroutine(EndTurn(0.0f)));

        AddCharactersToList("Party");
        AddCharactersToList("Enemy");

        for (int i = 0; i < turnIndicators.Count; i++)
        {
            if (turnIndicators[i].overrideSprite == null)
                turnIndicators[i].gameObject.SetActive(false);
        }

        combatInteractable = LayerMask.GetMask("CombatCharacter");

        targetingMouseReticle.GetComponent<Image>().enabled = false;
        cursorRevealObject.SetActive(false);

        rotForEnemy = currentTurnIndicatorWorldSpace.transform.rotation;
        rotForParty = rotForEnemy;
        rotForParty.eulerAngles = new Vector3(rotForParty.eulerAngles.x, rotForParty.eulerAngles.y + 180.0f, rotForParty.eulerAngles.z);

        StartCoroutine(EndTurn(0.0f));
    }

    void AddCharactersToList(string tag)
    {
        int offset = tag == "Party" ? 0 : numPartyMembersInScene;

        for (int i = 0; i < GameObject.FindGameObjectsWithTag(tag).Length; i++)
        {
            characters.Add(GameObject.FindGameObjectsWithTag(tag)[i]);

            BattleCharacter thisCharacter = characters[i + offset].GetComponent<BattleCharacter>();

            if (tag == "Party")
                party.Add(thisCharacter);
            if (tag == "Enemy")
                enemies.Add(thisCharacter);

            //thisCharacter.turnValue = thisCharacter.speed;

            thisCharacter.myTurnIndicator = turnIndicators[i + offset];

            if (thisCharacter.gameObject.GetComponent<EnemyInfo>() != null)
                thisCharacter.myTurnIndicator.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = thisCharacter.gameObject.GetComponent<EnemyInfo>().enemyNumber.ToString();
            else
                thisCharacter.myTurnIndicator.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().enabled = false;

            thisCharacter.myTurnIndicator.overrideSprite = thisCharacter.thumbnail;
        }
    }

    public IEnumerator EndTurn(float delay)
    {
        yield return new WaitForSeconds(delay);

        Debug.Log("Called EndTurn");

        if (currentTurnCharacter != null)
        {
            currentTurnCharacter.isMyTurn = false;
            currentTurnCharacter.uiParentScript.SetScale(false);
            currentTurnCharacter.turnIndicatorTargetX = 0.0f;
            if(currentTurnCharacter.tag == "Party")
                currentTurnCharacter.EnableAttackButtons(false);
            //currentTurnCharacter.ApplyStatusEffects();
            //currentTurnCharacter.transform.GetChild(0).GetComponent<Canvas>().sortingOrder = originalUIOrder;
        }

        BattleCharacter highestSpeed = characters[0].GetComponent<BattleCharacter>();
        //highestSpeed.turnValue = 0.0f;

        for (int i = 0; i < characters.Count; i++)
        {
            BattleCharacter thisCharacter = characters[i].GetComponent<BattleCharacter>();

            if (thisCharacter.turnValue > highestSpeed.turnValue)
                highestSpeed = thisCharacter;
        }

        currentTurnCharacter = highestSpeed;
        currentTurnCharacter.turnIndicatorTargetX = currentTurnIndicatorX;
        currentTurnCharacter.isMyTurn = true;
        currentTurnCharacter.ApplyAllStatusEffects();

        for (int i = 0; i < characters.Count; i++)
        {
            BattleCharacter thisCharacter = characters[i].GetComponent<BattleCharacter>();

            if (thisCharacter != highestSpeed)
            {
                thisCharacter.turnValue += thisCharacter.speed;
                thisCharacter.turnIndicatorTargetX = thisCharacter.turnValue * -turnIndicatorPosMultiplier;
            }
        }

        currentTurnCharacter.isMyTurn = true;
        currentTurnCharacter.uiParentScript.SetScale(true);
        currentTurnCharacter.turnValue = 0.0f;
        currentTurnCharacter.RefillBreaths();
        if(currentTurnCharacter.tag == "Party")
            currentTurnCharacter.EnableAttackButtons(true);
        SetCurrentTurnIndicatorWorldPos(currentTurnCharacter.transform);

        /*for(int i = 0; i < characters.Count; i++)
        {
            characters[i].GetComponent<BattleCharacter>().ApplyStatusEffects();
        }*/
    }

    void SetCurrentTurnIndicatorWorldPos(Transform target)
    {
        BattleCharacter targetBC = target.gameObject.GetComponent<BattleCharacter>();

        currentTurnIndicatorWorldSpaceTargetPos = new Vector3(target.transform.position.x, currentTurnIndicatorWorldSpace.transform.position.y, target.transform.position.z);
        if (target.gameObject.tag == "Party")
        {
            currentTurnIndicatorWorldSpace.GetComponent<SpriteRenderer>().color = Color.green;
            currentTurnIndicatorWorldSpaceTargetRot = rotForParty;
        }
        else if (target.gameObject.tag == "Enemy")
        {
            currentTurnIndicatorWorldSpace.GetComponent<SpriteRenderer>().color = Color.red;
            currentTurnIndicatorWorldSpaceTargetRot = rotForEnemy;
        }
    }

    void GetMouseOver()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, combatInteractable))
        {
            GameObject objectOver = hit.collider.gameObject;
            mouseOver = objectOver;
            //Debug.Log(objectOver.name);
        }
        else
            mouseOver = null;

        if(mouseOver != null)
            Debug.Log(mouseOver.name);
    }

    public void SetTargetingReticle(bool state)
    {
        targetingMouseReticle.GetComponent<Image>().enabled = state;
    }

    public void SetUIState(bool state)
    {
        mainCombatUI.GetComponent<Canvas>().enabled = state;
        currentTurnIndicatorWorldSpace.SetActive(state);
    }

    public void DamageCurrentTurnCharacter(float damage)
    {
        currentTurnCharacter.TakeDamage(damage);
    }

    // Update is called once per frame
    void Update()
    {
        GetMouseOver();

        targetingMouseReticle.transform.position = Input.mousePosition;

        if (currentTurnCharacter != null && currentTurnCharacter.tag == "Party")
            nextTurnButton.interactable = true;
        else
            nextTurnButton.interactable = false;

        cursorRevealObject.transform.position = Input.mousePosition;

        currentTurnIndicatorWorldSpace.transform.position = Vector3.Lerp
            (currentTurnIndicatorWorldSpace.transform.position, currentTurnIndicatorWorldSpaceTargetPos, Time.deltaTime * currentTurnIndicatorWorldSpaceMovementSpeed);
        currentTurnIndicatorWorldSpace.transform.rotation = Quaternion.Lerp
            (currentTurnIndicatorWorldSpace.transform.rotation, currentTurnIndicatorWorldSpaceTargetRot, Time.deltaTime * currentTurnIndicatorWorldSpaceMovementSpeed);
    }
}
