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
    public List<TargetingLineScript> lineScripts;
    public float currentTurnIndicatorWorldSpaceMovementSpeed;
    public Color enemyTurnColor, partyTurnColor;
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
    public SoulGuageScript soulGuage;
    public float turnIndicatorUpdateSpeed;
    public float turnIndicatorPosMultiplier;
    public float currentTurnIndicatorX;
    public float turnIndicatorScaleByProgressOffset;

    public BattleCharacter currentTurnCharacter;

    LayerMask combatInteractable;

    [HideInInspector]public GameObject mouseOver;

    public GameObject targetingMouseReticle;

    public float variableSetDelay;

    public Transform InstantiateAttackButtonsPos;

    public ActionWheelScript actionWheel;
    public TextMeshProUGUI attackDescriptionText;
    [HideInInspector] public string currentAttackDescription;

    Animator mainCameraAnim;

    private void Awake()
    {
        gm = this;

        StartCoroutine(EndTurn(0.0f));
    }

    // Start is called before the first frame update
    void Start()
    {
        nextTurnButton.onClick.AddListener(() => StartCoroutine(EndTurn(0.0f)));

        AddCharactersToList("Party");
        AddCharactersToList("Enemy");

        /*for (int i = 0; i < turnIndicators.Count; i++)
        {
            if (turnIndicators[i].overrideSprite == null)
                turnIndicators[i].gameObject.SetActive(false);
        }*/

        combatInteractable = LayerMask.GetMask("CombatCharacter");

        targetingMouseReticle.GetComponent<Image>().enabled = false;
        cursorRevealObject.SetActive(false);

        rotForEnemy = currentTurnIndicatorWorldSpace.transform.rotation;
        rotForParty = rotForEnemy;
        rotForEnemy.eulerAngles = new Vector3(rotForParty.eulerAngles.x, rotForParty.eulerAngles.y + 180.0f, rotForParty.eulerAngles.z);

        mainCameraAnim = Camera.main.gameObject.GetComponent<Animator>();

        //StartCoroutine(EndTurn(0.0f));
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

            if (thisCharacter.gameObject.GetComponent<EnemyInfo>() != null)
                thisCharacter.myTurnIndicator.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = thisCharacter.gameObject.GetComponent<EnemyInfo>().enemyNumber.ToString();
            else
                thisCharacter.myTurnIndicator.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
        }
    }

    public IEnumerator EndTurn(float delay)
    {
        yield return new WaitForSeconds(delay);

        //Debug.Log("Called EndTurn");

        if (currentTurnCharacter != null)
        {
            currentTurnCharacter.isMyTurn = false;
            //currentTurnCharacter.uiParentScript.SetScale(false);
            currentTurnCharacter.turnIndicatorTargetX = 0.0f;
            StartCoroutine(currentTurnCharacter.SetCurrentTurnThumbnail(false, 0.5f));
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
        //currentTurnCharacter.uiParentScript.SetScale(true);
        currentTurnCharacter.turnValue = 0.0f;
        currentTurnCharacter.RefillBreaths();
        StartCoroutine(currentTurnCharacter.SetCurrentTurnThumbnail(true, 0.0f));
        if (currentTurnCharacter.tag == "Party")
        {
            currentTurnCharacter.EnableAttackButtons(true);
            actionWheel.transform.parent.gameObject.SetActive(true);
            actionWheel.ResetWheel();
            actionWheel.SetDescriptionText(currentTurnCharacter.myAttackButtons[0].assignedAttackDescription);
        }
        else
        {
            actionWheel.transform.parent.gameObject.SetActive(false);
            actionWheel.SetDescriptionText("");
        }
        SetCurrentTurnIndicatorWorldPos(currentTurnCharacter.transform);

        /*for(int i = 0; i < characters.Count; i++)
        {
            characters[i].GetComponent<BattleCharacter>().ApplyStatusEffects();
        }*/

        nextTurnButton.interactable = false;
        nextTurnButton.interactable = true;
    }

    void SetCurrentTurnIndicatorWorldPos(Transform target)
    {
        BattleCharacter targetBC = target.gameObject.GetComponent<BattleCharacter>();

        currentTurnIndicatorWorldSpaceTargetPos = new Vector3(target.transform.position.x, currentTurnIndicatorWorldSpace.transform.position.y, target.transform.position.z);
        currentTurnIndicatorWorldSpace.transform.GetChild(0).gameObject.GetComponent<Animator>().SetTrigger("Pulse");

        if (target.gameObject.tag == "Party")
        {
            currentTurnIndicatorWorldSpace.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = partyTurnColor;
            currentTurnIndicatorWorldSpaceTargetRot = rotForParty;
        }
        else if (target.gameObject.tag == "Enemy")
        {
            currentTurnIndicatorWorldSpace.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = enemyTurnColor;
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
            mouseOver = objectOver.transform.root.gameObject;
            Debug.Log("From " + objectOver.name);
        }
        else
            mouseOver = null;

        if(mouseOver != null)
            Debug.Log("GM_GetMouseOver " + mouseOver.name);
    }

    public void SetTargetingReticle(bool state)
    {
        targetingMouseReticle.GetComponent<Image>().enabled = state;
    }

    public void SetUIState(bool state)
    {
        mainCombatUI.GetComponent<Canvas>().enabled = state;
        //currentTurnIndicatorWorldSpace.SetActive(state);
    }

    public void DamageCurrentTurnCharacter(float damage)
    {
        currentTurnCharacter.TakeDamage(damage);
    }

    public void CameraShakeEnemy()
    {
        mainCameraAnim.SetTrigger("EnemyHit");
    }

    public void CameraShakePlayer()
    {
        mainCameraAnim.SetTrigger("PlayerHit");
    }

    public void SetTargetingLineToTarget(int lineNum, Transform origin, Transform target)
    {
        lineScripts[lineNum].EnableSegmentsToTargetPosition(origin.position, target.position);
        lineScripts[lineNum].SetRotTowardTarget(target.position);
    }

    public void DisableTargetingLine(int lineNum)
    {
        lineScripts[lineNum].DisableAllSegments();
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
