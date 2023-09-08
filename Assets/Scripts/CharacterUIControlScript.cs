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
        anim.SetBool("isHover", hover);
        healthBarParent.GetComponent<Animator>().SetBool("isHover", hover);
        for (int i = 0; i < bubbleImages.Length; i++)
        {
            bubbleImages[i].color = new Color(bubbleImages[i].color.r, bubbleImages[i].color.g, bubbleImages[i].color.b, hover == true ? onHoverAlpha : offHoverAlpha);
        }
    }

    public void FlipUI()
    {
        RectTransform healthBarRT = healthBarParent.GetComponent<RectTransform>();
        healthBarRT.localScale = new Vector3(healthBarRT.localScale.x * -1.0f, healthBarRT.localScale.y * -1.0f, healthBarRT.localScale.z);
        //RectTransform fillRT = healthBarFill.gameObject.GetComponent<RectTransform>();
        //fillRT.localScale = new Vector3(fillRT.localScale.x * -1.0f, fillRT.localScale.y * -1.0f, fillRT.localScale.z);
        RectTransform bubblesTopRT = healthBarBubbleTop.gameObject.GetComponent<RectTransform>();
        bubblesTopRT.localScale = new Vector3(bubblesTopRT.localScale.x * -1.0f, bubblesTopRT.localScale.y * -1.0f, bubblesTopRT.localScale.z);
        RectTransform bubblesBottomRT = healthBarBubbleBottom.gameObject.GetComponent<RectTransform>();
        bubblesBottomRT.localScale = new Vector3(bubblesBottomRT.localScale.x * -1.0f, bubblesBottomRT.localScale.y * -1.0f, bubblesBottomRT.localScale.z);
    }

    // Update is called once per frame
    void Update()
    {
        //ScaleUpOnHover();
        FollowTarget();
    }
}
