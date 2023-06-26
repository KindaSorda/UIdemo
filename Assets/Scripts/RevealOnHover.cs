using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealOnHover : MonoBehaviour
{
    public GameObject revealTarget;

    LayerMask combatInteractable;

    // Start is called before the first frame update
    void Start()
    {
        revealTarget.SetActive(false);

        combatInteractable = LayerMask.GetMask("AdditionalUI");
    }

    void GetMouseOver()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, combatInteractable))
        {
            GameObject objectOver = hit.collider.gameObject;
            if (objectOver == gameObject)
                revealTarget.SetActive(true);
        }
        else
            revealTarget.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        GetMouseOver();
    }
}
