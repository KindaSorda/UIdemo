using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingLineScript : MonoBehaviour
{
    public float flashDelay;
    int tick = 0;

    public List<GameObject> segments = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < transform.childCount; i ++)
        {
            segments.Add(transform.GetChild(i).gameObject);
            segments[i].GetComponent<SpriteRenderer>().enabled = false;
        }

        StartCoroutine(StartFlashAnim(tick));
    }

    IEnumerator StartFlashAnim(int segmentNum)
    {
        yield return new WaitForSeconds(flashDelay * segmentNum);

        Animator anim = segments[segmentNum].GetComponent<Animator>();
        anim.SetTrigger("StartFlash");

        if (tick < transform.childCount - 1)
        {
            tick++;
            StartCoroutine(StartFlashAnim(tick));
        }
    }

    public void EnableSegmentsToTargetPosition(Vector3 from, Vector3 to)
    {
        float distance = Vector3.Distance(from, to);

        for (int i = 0; i < segments.Count; i++)
        {
            if(Vector3.Distance(segments[i].transform.position, from) <= distance)
            {
                segments[i].GetComponent<SpriteRenderer>().enabled = true;
            }
        }
    }

    public void DisableAllSegments()
    {
        //Debug.Log("Called Disable All Segments in Line Script");

        for (int i = 0; i < segments.Count; i++)
        {
            segments[i].GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    public void SetRotTowardTarget(Vector3 targetPos)
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, Vector3.Angle(Vector3.forward, transform.position - targetPos) * -1.0f, transform.eulerAngles.z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
