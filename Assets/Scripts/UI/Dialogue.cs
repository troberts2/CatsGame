using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.Tilemaps;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;

    private int index;
    private BoxCollider2D box;
    private GameObject panel;
    private PlayerMovement pm;
    private levelTimer lt;

    // Start is called before the first frame update
    void Start()
    {
        textComponent.text = string.Empty;
        box = GetComponent<BoxCollider2D>();
        panel = gameObject.transform.GetChild(0).gameObject;
        pm  = FindObjectOfType<PlayerMovement>();
        lt = FindObjectOfType<levelTimer>();
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
            }
        }
    }

    public void StartDialogue()
    {
        index = 0;
        panel.SetActive(true);
        pm.enabled = false;
        lt.ToggleTimer();
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            pm.enabled = true;
            lt.ToggleTimer();
            gameObject.SetActive(false);
        }
    }
}