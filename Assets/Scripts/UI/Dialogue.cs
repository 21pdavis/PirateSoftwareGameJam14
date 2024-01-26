using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class Dialogue : MonoBehaviour
{
    [SerializeField] private DialogueSequence sequence;
    [Tooltip("The state that should be transitioned to after conclusion of dialogue.")]
    [SerializeField] private GameManager.GameState stateAfterDialogue;

    [Header("UI Elements")]
    [SerializeField] private Image leftImage;
    [SerializeField] private Image leftMask;
    [SerializeField] private GameObject speakerContainer;
    [SerializeField] private Image rightImage;
    [SerializeField] private Image rightMask;
    [SerializeField] private GameObject dialogueContainer;
    [SerializeField] private RectTransform promptArrow;

    [Header("UI Parameters")]
    [SerializeField] private float maskOpacity = 0.95f;
    [SerializeField] private float promptBounceSpeed = 1.0f; // deg per second iterating for bounce
    [SerializeField] private float promptBounceMultiplier = 1.0f; // amplitude of sine wave
    [Tooltip("Delay between writing of individual letters during dialogue")]
    [SerializeField] private float textWriteDelay = 0.1f;

    [Header("References")]
    [SerializeField] private AudioSource talkingSource;

    private List<DialogueSequence.DialogueLine>.Enumerator dialogueIterator;
    private TextMeshProUGUI speakerText;
    private TextMeshProUGUI dialogueText;
    private DialogueSequence.DialogueLine currLine;
    private float initialPromptYPos;
    private float sinDegCount = 0;
    private bool animatingText = false;
    private IEnumerator animateTextHandle = null;

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
        bool moved = dialogueIterator.MoveNext();

        if (!moved)
        {
            GameManager.Instance.TransitionTo(stateAfterDialogue);
            return;
        }

        currLine = dialogueIterator.Current;

        // update images
        if (currLine.LeftImage)
        {
            rightImage.color = new Color(1f, 1f, 1f);
            leftImage.sprite = currLine.LeftImage;
        }
        else
        {
            leftImage.sprite = null;
            leftImage.color = new Color(0f, 0f, 0f);
        }

        if (currLine.RightImage)
        {
            rightImage.color = new Color(1f, 1f, 1f);
            rightImage.sprite = currLine.RightImage;
        }
        else
        {
            rightImage.sprite = null;
            rightImage.color = new Color(0f, 0f, 0f);
        }

        // update masks
        // TODO: Could clean this up and put some of it into functions, but no time atm
        Color tmp;
        if (currLine.Speaking == DialogueSequence.SpeakMode.Neither)
        {
            Color leftTmp = leftMask.color;
            tmp = rightMask.color;
            tmp.a = maskOpacity;
            leftTmp.a = maskOpacity;
            leftMask.color = leftTmp;
            rightMask.color = tmp;
        }
        else if (currLine.Speaking == DialogueSequence.SpeakMode.Left)
        {
            tmp = rightMask.color;
            tmp.a = maskOpacity;
            rightMask.color = tmp;

            tmp = leftMask.color;
            tmp.a = 0f;
            leftMask.color = tmp;
        }
        else if (currLine.Speaking == DialogueSequence.SpeakMode.Right)
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
        animateTextHandle = AnimateText();
        talkingSource.clip = currLine.TalkingClip;
        talkingSource.Play();
        StartCoroutine(animateTextHandle);
    }

    private IEnumerator AnimateText()
    {
        animatingText = true;
        string currText = "";
        char[] letters = currLine.Text.ToCharArray();

        foreach (char c in letters)
        {
            currText += c;
            dialogueText.text = currText;
            yield return new WaitForSeconds(textWriteDelay);
        }

        ResetAudio();
    }

    private void AnimatePrompt()
    {
        sinDegCount += (Time.deltaTime * promptBounceSpeed) % 360;
        promptArrow.localPosition = new Vector3(promptArrow.localPosition.x, initialPromptYPos + promptBounceMultiplier * Mathf.Sin(sinDegCount), 0f);
    }

    public void Continue(InputAction.CallbackContext context)
    {
        if (!context.started)
            return;

        if (animatingText)
        {
            StopCoroutine(animateTextHandle);
            ResetAudio();
        }
        else
        {
            AdvanceDialogue();
        }
    }

    private void ResetAudio()
    {
        talkingSource.Stop();
        talkingSource.clip = null;

        dialogueText.text = currLine.Text;
        animatingText = false;
    }
}
