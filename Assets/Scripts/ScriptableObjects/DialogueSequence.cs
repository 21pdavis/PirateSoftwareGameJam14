using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="DialogueSequence", menuName="ScriptableObjects/DialogueSequence", order=1)]
public class DialogueSequence : ScriptableObject
{
    public List<DialogueLine> dialogueLines = new();

    [Serializable]
    public struct DialogueLine
    {
        public bool LeftSpeaking;

        [TextArea(1, 1)]
        public string Speaker;
        public Sprite LeftImage;
        public Sprite RightImage;

        [TextArea(3, 1)]
        public string Text;

        public AudioClip TalkingSound;
    }
}
