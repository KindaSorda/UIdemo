using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealOnHover_Canvas : MonoBehaviour
{
    public GameObject revealTarget;

    public void Reveal(bool state)
    {
        if(gameObject.GetComponent<StatusEffectIconControl>().hoverable == true)
            revealTarget.SetActive(state);
    }
}
