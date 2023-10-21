using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverRipplesEffectScript : MonoBehaviour
{
    public float speed;
    Vector3 startPos, endPos;
    
    List<GameObject> riverTiles = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < gameObject.transform.childCount; i++)
        {
            riverTiles.Add(gameObject.transform.GetChild(i).gameObject);
        }

        endPos = riverTiles[0].transform.localPosition;
        startPos = riverTiles[transform.childCount - 1].transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < riverTiles.Count; i++)
        {
            riverTiles[i].transform.Translate(Vector3.up * Time.deltaTime * speed);

            if (riverTiles[i].transform.localPosition.y >= endPos.y)
                riverTiles[i].transform.localPosition = startPos;
        }
    }
}
