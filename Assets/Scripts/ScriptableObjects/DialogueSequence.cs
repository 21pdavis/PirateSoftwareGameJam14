using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="DialogueSequence", menuName="ScriptableObjects/DialogueSequence", order=1)]
public class DialogueSequence : ScriptableObject
{
    public List<DialogueLine> dialogueLines = new();

    public enum SpeakMode
    {
        Left,
        Right,
        Neither
    }

    [Serializable]
    public struct DialogueLine
    {
        public SpeakMode Speaking;

        [TextArea(1, 1)]
        public string Speaker;
        public Sprite LeftImage;
        public Sprite RightImage;

        [TextArea(3, 1)]
        public string Text;

        public AudioClip TalkingClip;
    }
}
