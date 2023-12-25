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
    public GameObject turnIndicatorsParent;
    public List<TurnIndicatorScript> characterTurnIndicators = new List<TurnIndicatorScript>();
    public List<TargetingLineScript> lineScripts;
    public float currentTurnIndicatorWorldSpaceMovementSpeed;
    public Color enemyTurnColor, partyTurnColor;
    Quaternion rotForParty, rotForEnemy;
    Vector3 currentTurnIndicatorWorldSpaceTargetPos;
    Quaternion currentTurnIndicatorWorldSpaceTargetRot;

    public int numPartyMembersInScene;

    public List<BattleCharacter> characters = new List<BattleCharacter>();
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

    public GameObject mouseOver;

    public bool targetingModeAllEnemies = false;
    public bool targetingModeSingleEnemy = false;
    public bool targetingModeAllParty = false;
    public bool targetingModeSingleParty = false;

    public GameObject targetingMouseReticle;
    public float damageRedFlashTime;

    public float variableSetDelay;

    public Transform InstantiateAttackButtonsPos;

    public ActionWheelScript actionWheel;
    public TextMeshProUGUI attackDescriptionText;
    [HideInInspector] public string currentAttackDescription;

    Animator mainCameraAnim;

    public string firstTurnTextCauseImCheating;

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

        targetingMouseReticle.SetActive(false);
        cursorRevealObject.SetActive(false);

        rotForEnemy = currentTurnIndicatorWorldSpace.transform.rotation;
        rotForParty = rotForEnemy;
        rotForEnemy.eulerAngles = new Vector3(rotForParty.eulerAngles.x, rotForParty.eulerAngles.y + 180.0f, rotForParty.eulerAngles.z);

        mainCameraAnim = Camera.main.gameObject.GetComponent<Animator>();

        //attackDescriptionText.text = firstTurnTextCauseImCheating;
        //StartCoroutine(EndTurn(0.0f));
    }

    void AddCharactersToList(string tag)
    {
        int offset = tag == "Party" ? 0 : numPartyMembersInScene;

        for (int i = 0; i < GameObject.FindGameObjectsWithTag(tag).Length; i++)
        {
            characters.Add(GameObject.FindGameObjectsWithTag(tag)[i].GetComponent<BattleCharacter>());

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
            Debug.Log("Party Check");
            currentTurnCharacter.EnableAttackButtons(true);
            actionWheel.transform.parent.gameObject.SetActive(true);
            StartCoroutine(actionWheel.ResetWheel());
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

        SetTurnIndicatorSiblingOrder();

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
            if (targetingModeSingleEnemy || targetingModeAllEnemies)
            {
                if(mouseOver.tag == "Enemy")
                    SetTargetingReticleState(true);
            }
            else if (targetingModeSingleParty || targetingModeAllParty)
            {
                if(mouseOver.tag == "Party")
                    SetTargetingReticleState(true);
            }
            else
                SetTargetingReticleState(false);
            //Debug.Log("From " + objectOver.name);
        }
        else
        {
            mouseOver = null;
            SetTargetingReticleState(false);
        }

        //if(mouseOver != null)
            //Debug.Log("GM_GetMouseOver " + mouseOver.name);
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
    public void SetTargetingReticle(bool state)
    {
        targetingMouseReticle.SetActive(state);
    }

    public void SetTargetingReticleState(bool state)
    {
        Animator trAnim = targetingMouseReticle.GetComponent<Animator>();
        trAnim.SetBool("Expand", state);
    }

    void SetTurnIndicatorSiblingOrder()
    {
        for (int i = 0; i < characters.Count; i++)
        {
            characters[i].myTurnIndicator.GetComponent<TurnIndicatorScript>().turnValue = characters[i].turnValue;
        }

        for(int i = 0; i < characterTurnIndicators.Count; i++)
        {
            int numBefore = 0;
            for (int j = 0; j < characterTurnIndicators.Count; j++)
            {
                if (characterTurnIndicators[i].turnValue > characterTurnIndicators[j].turnValue)
                {
                    numBefore++;
                }
            }
            characterTurnIndicators[i].transform.SetSiblingIndex(numBefore);
        }
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


        if (targetingModeAllEnemies || targetingModeSingleEnemy || targetingModeAllParty || targetingModeSingleParty)
        {
            if (targetingModeAllEnemies)
            {
                if (mouseOver != null && mouseOver.tag == "Enemy")
                {
                    SetTargetingLineToTarget(0, currentTurnCharacter.transform, enemies[0].transform);
                    SetTargetingLineToTarget(1, currentTurnCharacter.transform, enemies[1].transform);
                    SetTargetingLineToTarget(2, currentTurnCharacter.transform, enemies[2].transform);

                    enemies[0].uiParentScript.scaleUpFromGameManager = true;
                    enemies[1].uiParentScript.scaleUpFromGameManager = true;
                    enemies[2].uiParentScript.scaleUpFromGameManager = true;
                }
                else
                {
                    DisableTargetingLine(0);
                    DisableTargetingLine(1);
                    DisableTargetingLine(2);

                    enemies[0].uiParentScript.scaleUpFromGameManager = false;
                    enemies[1].uiParentScript.scaleUpFromGameManager = false;
                    enemies[2].uiParentScript.scaleUpFromGameManager = false;
                }
            }

            if (targetingModeSingleEnemy)
            {
                if (mouseOver != null && mouseOver.tag == "Enemy")
                {
                    SetTargetingLineToTarget(0, currentTurnCharacter.transform, mouseOver.transform);
                }
                else
                {
                    DisableTargetingLine(0);
                }
            }

            if (targetingModeAllParty)
            {
                if (mouseOver != null && mouseOver.tag == "Party")
                {
                    //SetTargetingLineToTarget(0, currentTurnCharacter.transform, party[0].transform);
                    //SetTargetingLineToTarget(1, currentTurnCharacter.transform, party[1].transform);
                    //SetTargetingLineToTarget(2, currentTurnCharacter.transform, party[2].transform);

                    party[0].uiParentScript.scaleUpFromGameManager = true;
                    party[1].uiParentScript.scaleUpFromGameManager = true;
                    party[2].uiParentScript.scaleUpFromGameManager = true;
                }
                else
                {
                    //DisableTargetingLine(0);
                    //DisableTargetingLine(1);
                    //DisableTargetingLine(2);

                    party[0].uiParentScript.scaleUpFromGameManager = false;
                    party[1].uiParentScript.scaleUpFromGameManager = false;
                    party[2].uiParentScript.scaleUpFromGameManager = false;
                }
            }

            if (targetingModeSingleParty)
            {
                if (mouseOver != null && mouseOver.tag == "Party")
                {
                    SetTargetingLineToTarget(0, currentTurnCharacter.transform, mouseOver.transform);
                }
                else
                {
                    DisableTargetingLine(0);
                }
            }
        }
        else
        {
            DisableTargetingLine(0);
            DisableTargetingLine(1);
            DisableTargetingLine(2);

            enemies[0].uiParentScript.scaleUpFromGameManager = false;
            enemies[1].uiParentScript.scaleUpFromGameManager = false;
            enemies[2].uiParentScript.scaleUpFromGameManager = false;

            party[0].uiParentScript.scaleUpFromGameManager = false;
            party[1].uiParentScript.scaleUpFromGameManager = false;
            party[2].uiParentScript.scaleUpFromGameManager = false;
        }


        //SetTurnIndicatorSiblingOrder();
        //for (int i = 0; i < characterTurnIndicators.Count; i++)
        //    Debug.Log(characterTurnIndicators[i].transform.GetSiblingIndex());
    }
}
