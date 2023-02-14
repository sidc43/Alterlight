using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public string[] dialogues;
    public Button button;
    public TextMeshProUGUI text;
    public float textWaitTime;
    private int index;
    public bool isActive;
    private bool reachedEnd;

    private void Start() 
    {
        text.text = string.Empty;
        StartDialogue();
    }
    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E))
        {
            if (text.text == dialogues[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                text.text = dialogues[index];
            }
            
        }

        if (Input.GetKeyDown(KeyCode.Escape) && this.gameObject.activeSelf || reachedEnd)
        {
            this.gameObject.SetActive(false);
            index = 0;
            reachedEnd = false;
        }
    }
    void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }
    IEnumerator TypeLine()
    {
        foreach (char c in dialogues[index].ToCharArray())
        {
            text.text += c;
            yield return new WaitForSeconds(textWaitTime);
        }
    }
    void NextLine()
    {
        if (index < dialogues.Length - 1)
        {
            index++;
            text.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            this.gameObject.SetActive(false);
            reachedEnd = true;
        }
    }
}
