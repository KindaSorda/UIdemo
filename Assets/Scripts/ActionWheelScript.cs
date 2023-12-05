using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionWheelScript : MonoBehaviour
{
    public Button rotateUp, rotateDown;
    public float speed;

    public bool invertRot;

    Quaternion startingRot, targetRot;

    int numClicksDown = 0;

    // Start is called before the first frame update
    void Start()
    {
        startingRot = transform.rotation;
        targetRot = startingRot;

        float angle = invertRot ? 30.0f : -30.0f;

        rotateUp.onClick.AddListener(() => TurnWheel(angle, -1));
        rotateDown.onClick.AddListener(() => TurnWheel(-angle, 1));

        Debug.Log("Action Wheel Starting Rot: " + startingRot.eulerAngles);
    }

    public void TurnWheel(float angle, int click)
    {
        startingRot = targetRot;
        targetRot.eulerAngles = new Vector3(targetRot.eulerAngles.x, targetRot.eulerAngles.y, targetRot.eulerAngles.z + angle);

        numClicksDown += click;
        SetDescriptionText(GameManager.gm.currentTurnCharacter.myAttackButtons[numClicksDown].assignedAttackDescription);

        rotateUp.interactable = false;
        rotateDown.interactable = false;
        rotateUp.interactable = true;
        rotateDown.interactable = true;
    }

    public void SetDescriptionText(string text)
    {
        Debug.Log("Set Description Text to " + text);
        GameManager.gm.attackDescriptionText.text = text;
    }

   public IEnumerator ResetWheel()
    {
        yield return new WaitForSeconds(0.1f);
        Debug.Log("Reset Wheel");
        targetRot.eulerAngles = new Vector3(0.0f, 0.0f, 357.11f);
        numClicksDown = 0;
        SetDescriptionText(GameManager.gm.currentTurnCharacter.myAttackButtons[numClicksDown].assignedAttackDescription);
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, Time.deltaTime * speed);

        if (numClicksDown == 0)
            rotateUp.interactable = false;
        else
            rotateUp.interactable = true;

        if (GameManager.gm.currentTurnCharacter != null) {
            if (numClicksDown == GameManager.gm.currentTurnCharacter.gameObject.GetComponent<InstantiateAttackButtons>().numAttacks - 1)
                rotateDown.interactable = false;
            else
                rotateDown.interactable = true;
        }
    }
}
