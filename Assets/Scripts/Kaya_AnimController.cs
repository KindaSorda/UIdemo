using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kaya_AnimController : MonoBehaviour
{
    Animator anim;
    public Vector2 idleFlareTimerRange;
    float idleFlareTimer;

    // Start is called before the first frame update
    void Start()
    {
        Random.InitState(System.DateTime.Now.Millisecond);

        anim = GetComponent<Animator>();
        StartCoroutine(TriggerIdleFlare());
    }

    IEnumerator TriggerIdleFlare()
    {
        idleFlareTimer = Random.Range(idleFlareTimerRange.x, idleFlareTimerRange.y);

        yield return new WaitForSeconds(idleFlareTimer);
        anim.SetTrigger("Flare");
        StartCoroutine(TriggerIdleFlare());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
