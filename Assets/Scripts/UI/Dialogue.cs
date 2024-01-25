using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class Dialogue : MonoBehaviour
{
    [SerializeField] private DialogueSequence sequence;

    [Header("UI Elements")]
    [SerializeField] private Image leftImage;
    [SerializeField] private Image rightImage;
    [SerializeField] private GameObject leftSpeakerContainer;
    [SerializeField] private GameObject rightSpeakerContainer;
    [SerializeField] private GameObject dialogueContainer;
    [SerializeField] private RectTransform promptArrow;
    [SerializeField] private float promptBounceSpeed = 1.0f; // deg per second iterating for bounce
    [SerializeField] private float promptBounceMultiplier = 1.0f; // amplitude of sine wave

    private List<DialogueSequence.DialogueLine>.Enumerator dialogueIterator;
    private TextMeshProUGUI leftSpeakerText;
    private TextMeshProUGUI rightSpeakerText;
    private TextMeshProUGUI dialogueText;
    private float initialPromptYPos;
    private float sinDegCount = 0;

    private void Start()
    {
        dialogueIterator = sequence.dialogueLines.GetEnumerator();

        leftSpeakerText = leftSpeakerContainer.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        rightSpeakerText = rightSpeakerContainer.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        dialogueText = dialogueContainer.transform.Find("Text").GetComponent<TextMeshProUGUI>();

        initialPromptYPos = promptArrow.localPosition.y;

        // advance to first dialogue
        AdvanceDialogue();
    }

    private void Update()
    {
        AnimatePrompt();
    }

    private void AdvanceDialogue()
    {
        dialogueIterator.MoveNext();

        DialogueSequence.DialogueLine currLine = dialogueIterator.Current;

        // update images
        leftImage.sprite = currLine.LeftImage;
        rightImage.sprite = currLine.RightImage;

        // update speaker
        if (currLine.LeftSpeaking)
        {
            leftSpeakerContainer.SetActive(true);
            rightSpeakerContainer.SetActive(false);

            leftSpeakerText.text = currLine.Speaker;
        }
        else
        {
            leftSpeakerContainer.SetActive(false);
            rightSpeakerContainer.SetActive(true);

            rightSpeakerText.text = currLine.Speaker;
        }

        // update dialogue text
        dialogueText.text = currLine.Text;
    }

    private void AnimatePrompt()
    {
        sinDegCount += (Time.deltaTime * promptBounceSpeed) % 360;
        promptArrow.localPosition = new Vector3(promptArrow.localPosition.x, initialPromptYPos + Time.deltaTime * promptBounceMultiplier * Mathf.Sin(sinDegCount), 0f);
    }

    public void Continue(InputAction.CallbackContext context)
    {
        if (!context.started)
            return;

        AdvanceDialogue();
    }
}
