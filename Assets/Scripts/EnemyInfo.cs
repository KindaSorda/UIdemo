using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EnemyInfo : MonoBehaviour
{
    public bool hasNumber;
    public int enemyNumber;
    public GameObject enemyNumText;

    // Start is called before the first frame update
    void Start()
    {
        enemyNumText.GetComponent<TextMeshProUGUI>().text = enemyNumber.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
