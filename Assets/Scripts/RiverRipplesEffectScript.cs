using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverRipplesEffectScript : MonoBehaviour
{
    public int x, y, z;

    public float speed;
    float instantiationDistance;
    Vector3 startPos, endPos;
    
    List<GameObject> riverTiles = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < gameObject.transform.childCount; i++)
        {
            riverTiles.Add(gameObject.transform.GetChild(i).gameObject);
        }

        instantiationDistance = Vector3.Distance(new Vector3(
            riverTiles[0].transform.localPosition.x * x, riverTiles[0].transform.localPosition.y * y, riverTiles[0].transform.localPosition.z * z), new Vector3(
            riverTiles[1].transform.localPosition.x * x, riverTiles[1].transform.localPosition.y * y, riverTiles[1].transform.localPosition.z * z));

        Debug.Log(gameObject.name + " Instantiation Distance: " + instantiationDistance);

        endPos = riverTiles[0].transform.localPosition;
        startPos = riverTiles[transform.childCount - 1].transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < riverTiles.Count; i++)
        {
            if (riverTiles[i].transform.localPosition.y >= endPos.y)
            {
                GameObject lastInLine = riverTiles[i == 0 ? riverTiles.Count - 1 : i - 1];

                riverTiles[i].transform.localPosition = new Vector3(
                    lastInLine.transform.localPosition.x + (-instantiationDistance * Mathf.Abs(x)),
                    lastInLine.transform.localPosition.y + (-instantiationDistance * Mathf.Abs(y)),
                    lastInLine.transform.localPosition.z + (-instantiationDistance * Mathf.Abs(z)));
            }

            riverTiles[i].transform.Translate(new Vector3(x, y, z) * Time.deltaTime * speed);
        }
    }
}
