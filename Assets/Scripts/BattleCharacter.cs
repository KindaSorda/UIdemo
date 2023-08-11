using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleCharacter : MonoBehaviour
{
    Animator uIanim;

    public Transform UIFollowsHere;

    public Sprite thumbnail;
    [HideInInspector]public Image myTurnIndicator;
    public float turnIndicatorTargetX;
    public Vector3 turnIndicatorCurrentPos;

    public bool isMyTurn = false;

    [Header("CharacterStats")]
    public float startingHealth;
    public float health;
    public float speed;
    [Range(0.0f, 10.0f)] public float baseSpeed;
    public float turnValue;

    [Header("UI Objects")]
    public GameObject uiParent;
    //public GameObject healthUI;
    public GameObject currentTurnIndicator;
    //public List<Image> allUIObjects = new List<Image>();
    public TextMeshProUGUI healthText;
    public Image healthBar;
    public GameObject combatControls;

    [Header("Breath and Soul Variables")]
    public int startingBreaths;
    [HideInInspector] public int breathRegen;
    public List<BreathUINode> breathNodes = new List<BreathUINode>();
    public float breathUiInstantiationRotOffset;
    GameObject breathsUIContainer;
    [HideInInspector]public int breathsSpentThisTurn = 0;
    public bool isInNegativeBreaths = false;
    List<GameObject> negativeBreaths = new List<GameObject>();

    [Header("Status Effect Variables")]
    public List<StatusEffectIconControl> buffIcons = new List<StatusEffectIconControl>();
    public List<StatusEffectIconControl> debuffIcons = new List<StatusEffectIconControl>();
    public List<SO_StatusEffect> buffs = new List<SO_StatusEffect>();
    public List<SO_StatusEffect> debuffs = new List<SO_StatusEffect>();

    // Start is called before the first frame update
    void Start()
    {
        uiParent = Instantiate(Resources.Load("Prefabs/NewCharacterUI") as GameObject, GameManager.gm.mainCombatUI.transform);
        uiParent.GetComponent<CharacterUIFollowTarget>().target = UIFollowsHere;
        uiParent.name = gameObject.name + " UI";

        healthText = uiParent.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        healthBar = uiParent.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<Image>();
        combatControls = uiParent.transform.GetChild(1).gameObject;
        breathsUIContainer = uiParent.transform.GetChild(2).gameObject;
        int numStatusIcons = uiParent.transform.GetChild(0).GetChild(3).childCount;
        for (int i = 0; i < numStatusIcons; i++)
        {
            buffIcons.Add(uiParent.transform.GetChild(0).GetChild(3).GetChild(i).GetComponent<StatusEffectIconControl>());
            debuffIcons.Add(uiParent.transform.GetChild(0).GetChild(4).GetChild(i).GetComponent<StatusEffectIconControl>());
        }
        if(gameObject.tag == "Party")
            GetComponent<InstantiateAttackButtons>().InstantiateButtons();
        // /\/\/\/\/\/\ This is all to set the needed variable since the character UI needs to be instantiated, the variables can't be assigned in inspector


        //uIanim = transform.GetChild(0).gameObject.GetComponent<Animator>();

        currentTurnIndicator.transform.parent = null;
        currentTurnIndicator.SetActive(false);

        if(combatControls != null)
            combatControls.SetActive(false);

        health = startingHealth;
        speed = baseSpeed;
        breathRegen = startingBreaths;

        InstantiateBreaths();

        for(int i = 0; i < buffIcons.Count; i++)
        {
            buffIcons[i].EmptyOnStart();
            debuffIcons[i].EmptyOnStart();
        }
    }

    void InstantiateBreaths()
    {
        for (int i = 0; i < startingBreaths; i++)
        {
            GameObject newBreath = Instantiate<GameObject>(Resources.Load("Prefabs/BreathContainer") as GameObject, breathsUIContainer.transform);
            Vector3 startingRot = newBreath.GetComponent<Transform>().eulerAngles;
            newBreath.GetComponent<Transform>().eulerAngles = new Vector3(startingRot.x, startingRot.y, breathUiInstantiationRotOffset * i);
            breathNodes.Add(newBreath.GetComponent<BreathUINode>());
        }
        Vector3 parentRot = breathsUIContainer.transform.localEulerAngles;
        breathsUIContainer.transform.localEulerAngles = new Vector3(parentRot.x, parentRot.y, parentRot.z + -((breathNodes.Count - 1) * breathUiInstantiationRotOffset));
    }

    void EnableingUI()
    {
        /*if(GameManager.gm.mouseOver == gameObject)
        {
            uIanim.SetBool("isHover", true);
        }
        else
        {
            uIanim.SetBool("isHover", false);
        }*/

        if (combatControls != null)
        {
            combatControls.SetActive(isMyTurn);
        }
    }

    public IEnumerator SpendBreaths(int cost, float delay)
    {
        yield return new WaitForSeconds(delay);

        int up = breathsSpentThisTurn;
        int numNegativeBreaths = 0;

        for (int i = 0; i < cost; i++)
        {
            up++;
            int newI = breathsSpentThisTurn + i;

            if (newI < breathNodes.Count)
            {
                //breathNodes[newI].activeBreath = false;
                breathNodes[newI].SpendBreath();
            }
            else
            {
                numNegativeBreaths++;
                GameObject newNegativeBreath = Instantiate(Resources.Load("Prefabs/NegativeBreathContainer") as GameObject, breathsUIContainer.transform);
                Vector3 startingRot = newNegativeBreath.GetComponent<Transform>().eulerAngles;
                newNegativeBreath.GetComponent<Transform>().eulerAngles = new Vector3(startingRot.x, startingRot.y, breathUiInstantiationRotOffset * numNegativeBreaths);
                negativeBreaths.Add(newNegativeBreath);
                isInNegativeBreaths = true;

                SO_StatusEffect newEffect = Instantiate(Resources.Load("StatusEffects/OutOfBreath") as SO_StatusEffect);
                InflictStatusEffect(newEffect);
                StartCoroutine(GameManager.gm.EndTurn(delay));
            }
        }

        breathsSpentThisTurn += cost;
    }

    public void RefillBreaths()
    {
        for(int i = 0; i < breathRegen; i++)
        {
            if(breathNodes[i].spentBreath == true)
            {
                //breathNodes[i].activeBreath = true;
                breathNodes[i].RefillBreath();
            }
        }

        for(int i = 0; i < negativeBreaths.Count; i++)
        {
            Destroy(negativeBreaths[i]);
        }
        negativeBreaths.Clear();


        breathsSpentThisTurn = 0;
    }

    public void InflictStatusEffect(SO_StatusEffect effect)
    {
        Debug.Log("Triggered Inflict of " + effect.name);
        if (effect.isBuff)
        {
            bool alreadyInflicted = false;

            for (int i = 0; i < buffs.Count; i++)
            {
                if (buffs[i].name == effect.name)
                {
                    alreadyInflicted = true;
                    buffs[i].stacks++;
                }
            }

            if (!alreadyInflicted)
                buffs.Add(effect);

            UpdateBuffIcons();
            Debug.Log("Inflicted " + effect.name);
        }
        else
        {
            bool alreadyInflicted = false;

            for (int i = 0; i < debuffs.Count; i++)
            {
                if(debuffs[i].name == effect.name)
                {
                    alreadyInflicted = true;
                    debuffs[i].stacks++;
                }
            }

            if (!alreadyInflicted)
                debuffs.Add(effect);

            UpdateDebuffIcons();
            ApplyStatusEffect(effect);
            Debug.Log("Inflicted " + effect.name);
        }
    }

    void UpdateBuffIcons()
    {
        for (int i = 0; i < buffs.Count; i++)
        {
            //buffIcons[i].gameObject.GetComponent<Image>().sprite = buffs[i].thumbnailSprite;
            if (buffs[i] != null)
                buffIcons[i].AssignEffect(buffs[i]);
        }
    }

    void UpdateDebuffIcons()
    {
        for(int i = 0; i < debuffs.Count; i++)
        {
            //debuffIcons[i].gameObject.GetComponent<Image>().sprite = debuffs[i].thumbnailSprite;
            if (debuffs[i] != null)
                debuffIcons[i].AssignEffect(debuffs[i]);
        }
    }

    public void ApplyAllStatusEffects()
    {
        for(int i = 0; i < debuffs.Count; i++)
        {
            debuffs[i].duration--;
            if (debuffs[i].duration <= 0)
            {
                debuffs.Remove(debuffs[i]);
                debuffIcons[i].Empty();
            }

            if (debuffs.Count > 0)
                speed = baseSpeed + (baseSpeed * ((debuffs[i].percentSpeedEffect * debuffs[i].stacks) * 0.01f));
            else
                speed = baseSpeed;
        }
    }

    public void ApplyStatusEffect(SO_StatusEffect effect)
    {
        for (int i = 0; i < debuffs.Count; i++)
        {
            speed = baseSpeed + (baseSpeed * ((debuffs[i].percentSpeedEffect * debuffs[i].stacks) * 0.01f));
        }
    }

    // Update is called once per frame
    void Update()
    {
        healthText.text = health.ToString();
        healthBar.fillAmount = health / startingHealth;

        //if(controlUI != null)
        //    controlUI.SetActive(isMyTurn);
        currentTurnIndicator.SetActive(isMyTurn);

        //(uIanim != null)
        EnableingUI();

        if(myTurnIndicator != null)
            myTurnIndicator.rectTransform.localPosition = Vector3.Lerp(myTurnIndicator.rectTransform.localPosition, new Vector3(turnIndicatorTargetX, 0.0f, 0.0f), Time.deltaTime * GameManager.gm.turnIndicatorUpdateSpeed);
    }
}
