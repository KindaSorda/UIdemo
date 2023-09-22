using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterUIControlScript : MonoBehaviour
{
    [HideInInspector] public Transform target;
    RectTransform rt;

    Animator anim;

    public RawImage[] bubbleImages;
    [Range(0.0f,1.0f)]public float onHoverAlpha, offHoverAlpha;

    [Header("The following variables are for eas of assigning to BattleCharacter upon Instantiation")]
    public TextMeshProUGUI healthText;
    public Image healthBar, healthBarFill;
    public RawImage healthBarBubbleTop, healthBarBubbleBottom;
    public GameObject healthBarParent;
    public GameObject breathsUIcontainer;
    public List<StatusEffectIconControl> buffIcons = new List<StatusEffectIconControl>();
    public List<StatusEffectIconControl> debuffIcons = new List<StatusEffectIconControl>();
    public float breathInstantiationRotOffset;

    [HideInInspector] public BattleCharacter myCharacter;

    // Start is called before the first frame update
    void Start()
    {
        rt = GetComponent<RectTransform>();
        anim = GetComponent<Animator>();

        for (int i = 0; i < bubbleImages.Length; i++)
        {
            bubbleImages[i].color = new Color(bubbleImages[i].color.r, bubbleImages[i].color.g, bubbleImages[i].color.b, offHoverAlpha);
        }
    }

    void FollowTarget()
    {
        rt.position = Camera.main.WorldToScreenPoint(target.position);
    }

    public void ScaleOnHover(bool hover)
    {
        /*if (myCharacter.isMyTurn == false)
        {
            SetScale(hover);
        }*/
        SetScale(hover);
    }

    public void SetScale(bool state)
    {
        anim.SetBool("isHover", state);
        healthBarParent.GetComponent<Animator>().SetBool("isHover", state);
        for (int i = 0; i < bubbleImages.Length; i++)
        {
            bubbleImages[i].color = new Color(bubbleImages[i].color.r, bubbleImages[i].color.g, bubbleImages[i].color.b, state == true ? onHoverAlpha : offHoverAlpha);
        }
    }

    public void FlipUI()
    {
        RectTransform healthBarRT = healthBarParent.GetComponent<RectTransform>();
        healthBarRT.localScale = new Vector3(healthBarRT.localScale.x * -1.0f, healthBarRT.localScale.y, healthBarRT.localScale.z);
    }

    // Update is called once per frame
    void Update()
    {
        //ScaleUpOnHover();
        FollowTarget();
    }
}
