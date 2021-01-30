using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Narrative : MonoBehaviour
{
    public static bool InPoem = false;
    public Poem IntroPoem;
    public Poem Emptyhanded;
    public Poem FirstItem;
    public Poem SecondItem;
    public TextMeshProUGUI NarrativeText;
    public float TimePerCharacter = 0.1f;
    public float TimePerSentence = 2f;

    public GameObject NarrativeCanvas;

    private Poem currentPoem;
    private int currentLine = 0;
    private float lineTimer = 0f;
    private bool fullLine = false;
    private int itemsBrought = 0;

    private void Start() {
        PlayPoem(IntroPoem);
    }

    private void Update() {
        if (!InPoem)
            return;

        if (Input.GetMouseButtonDown(0)) {
            if (lineTimer <= currentPoem.Lines[currentLine].Length * TimePerCharacter) {
                lineTimer = currentPoem.Lines[currentLine].Length * TimePerCharacter;
                NarrativeText.text = currentPoem.Lines[currentLine];
            }
            else
                NextLine();
        }

        lineTimer += Time.deltaTime;
        if(lineTimer / TimePerCharacter <= currentPoem.Lines[currentLine].Length)
            NarrativeText.text = currentPoem.Lines[currentLine].Substring(0, (int)(lineTimer / TimePerCharacter));
        else {
            if(lineTimer - currentPoem.Lines[currentLine].Length * TimePerCharacter > TimePerSentence) {
                NextLine();
            }
        }
    }

    private void NextLine() {
        NarrativeText.text = "";
        lineTimer = 0f;
        if(currentLine < currentPoem.Lines.Count - 1)
            currentLine++;
        else {
            NarrativeCanvas.SetActive(false);
            InPoem = false;
        }
    }

    private void PlayPoem(Poem poem) {
        //Time.timeScale = 0f;
        NarrativeCanvas.SetActive(true);
        currentPoem = poem;
        currentLine = 0;
        InPoem = true;
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player") || InPoem)
            return;

        Crow c = other.GetComponent<Crow>();
        if (c == null)
            return;

        if(c.ObjectHeld == null) {
            PlayPoem(Emptyhanded);
        }
        else {
            c.ObjectHeld.tag = "Untagged";
            c.DropObject();

            itemsBrought++;
            if(itemsBrought == 1) {
                PlayPoem(FirstItem);
            }
            else if(itemsBrought == 2) {
                PlayPoem(SecondItem);
            }
        }
    }
}


