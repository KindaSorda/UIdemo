using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleCharacter : MonoBehaviour
{
    Animator uIanim;

    public Sprite thumbnail;
    [HideInInspector]public Image myTurnIndicator;
    public float turnIndicatorTargetX;
    public Vector3 turnIndicatorCurrentPos;

    public bool isMyTurn = false;

    [Header("CharacterStats")]
    public float startingHealth;
    public float health;
    [Range(0.0f, 10.0f)] public float speed;
    float baseSpeed;
    public float turnValue;

    [Header("UI Objects")]
    public GameObject healthUI;
    public GameObject combatControls;
    public GameObject currentTurnIndicator;
    public List<Image> allUIObjects = new List<Image>();
    public TextMeshProUGUI healthText;
    public Image healthBar;

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
        uIanim = transform.GetChild(0).gameObject.GetComponent<Animator>();

        //healthUI.SetActive(false);
        currentTurnIndicator.transform.parent = null;
        currentTurnIndicator.SetActive(false);

        if(combatControls != null)
            combatControls.SetActive(false);

        health = startingHealth;
        baseSpeed = speed;
        breathRegen = startingBreaths;

        InstantiateBreaths();

        allUIObjects.Add(healthUI.GetComponent<Image>());
        allUIObjects.Add(combatControls.GetComponent<Image>());
        for(int i = 0; i < breathNodes.Count; i++)
        {
            allUIObjects.Add(breathNodes[i].gameObject.GetComponent<Image>());
        }

        for(int i = 0; i < buffIcons.Count; i++)
        {
            buffIcons[i].gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Empty");
            debuffIcons[i].gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Empty");
        }
    }

    void InstantiateBreaths()
    {
        breathsUIContainer = transform.GetChild(0).GetChild(2).gameObject;

        for(int i = 0; i < startingBreaths; i++)
        {
            GameObject newBreath = Instantiate<GameObject>(Resources.Load("Prefabs/BreathContainer") as GameObject, breathsUIContainer.transform);
            Vector3 startingRot = newBreath.GetComponent<Transform>().eulerAngles;
            newBreath.GetComponent<Transform>().eulerAngles = new Vector3(startingRot.x, startingRot.y, breathUiInstantiationRotOffset * i);
            breathNodes.Add(newBreath.GetComponent<BreathUINode>());
        }
    }

    void EnableingUI()
    {
        if(GameManager.gm.mouseOver == gameObject)
        {
            //healthUI.SetActive(true);
            //breathsUIContainer.SetActive(true);
            uIanim.SetBool("isHover", true);
        }
        else
        {
            //healthUI.SetActive(false);
            //breathsUIContainer.SetActive(false);
            uIanim.SetBool("isHover", false);
        }

        if (combatControls != null)
        {
            combatControls.SetActive(isMyTurn);
        }
    }

    public void SpendBreaths(int cost)
    {
        int up = breathsSpentThisTurn;
        int numNegativeBreaths = 0;

        for (int i = 0; i < cost; i++)
        {
            up++;
            int newI = breathsSpentThisTurn + i;

            if(newI < breathNodes.Count)
                breathNodes[newI + (breathNodes.Count - (newI + up))].activeBreath = false;
            else
            {
                numNegativeBreaths++;
                GameObject newNegativeBreath = Instantiate<GameObject>(Resources.Load("Prefabs/NegativeBreathContainer") as GameObject, breathsUIContainer.transform);
                Vector3 startingRot = newNegativeBreath.GetComponent<Transform>().eulerAngles;
                newNegativeBreath.GetComponent<Transform>().eulerAngles = new Vector3(startingRot.x, startingRot.y, -breathUiInstantiationRotOffset * numNegativeBreaths);
                negativeBreaths.Add(newNegativeBreath);
                isInNegativeBreaths = true;
                InflictStatusEffect(Resources.Load<SO_StatusEffect>("StatusEffects/OutOfBreath"));
            }
        }

        breathsSpentThisTurn += cost;
    }

    public void RefillBreaths()
    {
        for(int i = 0; i < breathRegen; i++)
        {
            if(breathNodes[i].activeBreath == false)
            {
                breathNodes[i].activeBreath = true;
            }
        }

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

    public void ApplyStatusEffects()
    {
        for(int i = 0; i < debuffs.Count; i++)
        {
            speed += speed * ((debuffs[i].percentSpeedEffect * debuffs[i].stacks) * 0.01f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        healthText.text = health.ToString();
        healthBar.fillAmount = (health * 0.5f) / startingHealth;

        //if(controlUI != null)
        //    controlUI.SetActive(isMyTurn);
        currentTurnIndicator.SetActive(isMyTurn);

        if(uIanim != null)
            EnableingUI();

        if(myTurnIndicator != null)
            myTurnIndicator.rectTransform.localPosition = Vector3.Lerp(myTurnIndicator.rectTransform.localPosition, new Vector3(turnIndicatorTargetX, 0.0f, 0.0f), Time.deltaTime * GameManager.gm.turnIndicatorUpdateSpeed);
    }
}
