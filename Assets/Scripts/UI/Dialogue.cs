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
    [SerializeField] private Image leftMask;
    [SerializeField] private GameObject speakerContainer;
    [SerializeField] private Image rightImage;
    [SerializeField] private Image rightMask;
    [SerializeField] private GameObject dialogueContainer;
    [SerializeField] private RectTransform promptArrow;

    [Header("UI Paramters")]
    [SerializeField] private float maskOpacity = 0.95f;
    [SerializeField] private float promptBounceSpeed = 1.0f; // deg per second iterating for bounce
    [SerializeField] private float promptBounceMultiplier = 1.0f; // amplitude of sine wave

    private List<DialogueSequence.DialogueLine>.Enumerator dialogueIterator;
    private TextMeshProUGUI speakerText;
    private TextMeshProUGUI dialogueText;
    private float initialPromptYPos;
    private float sinDegCount = 0;
    // TODO: progressive text animation

    private void Start()
    {
        dialogueIterator = sequence.dialogueLines.GetEnumerator();

        speakerText = speakerContainer.transform.Find("Text").GetComponent<TextMeshProUGUI>();
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
        if (currLine.LeftImage)
        {
            leftImage.sprite = currLine.LeftImage;
        }
        else
        {
            leftImage.sprite = null;
            leftImage.color = new Color(0f, 0f, 0f);
        }

        if (currLine.RightImage)
        {
            rightImage.sprite = currLine.RightImage;
        }
        else
        {
            rightImage.sprite = null;
            rightImage.color = new Color(0f, 0f, 0f);
        }

        leftImage.sprite = currLine.LeftImage;
        rightImage.sprite = currLine.RightImage;

        // update masks
        Color tmp;
        if (currLine.LeftSpeaking)
        {
            tmp = rightMask.color;
            tmp.a = maskOpacity;
            rightMask.color = tmp;

            tmp = leftMask.color;
            tmp.a = 0f;
            leftMask.color = tmp;
        }
        else
        {
            tmp = leftMask.color;
            tmp.a = maskOpacity;
            leftMask.color = tmp;

            tmp = rightMask.color;
            tmp.a = 0f;
            rightMask.color = tmp;
        }

        // update speaker
        if (!currLine.Speaker.Equals(""))
        {
            speakerContainer.SetActive(true);
            speakerText.text = currLine.Speaker;
        }
        else
        {
            speakerContainer.SetActive(false);
        }

        // update dialogue text
        dialogueText.text = currLine.Text;
    }

    private IEnumerator AnimateText()
    {
        yield return null;
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
