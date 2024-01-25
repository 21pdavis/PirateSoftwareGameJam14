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
        [TextArea(1, 1)]
        public string Speaker;

        [TextArea(3, 1)]
        public string Text;

        public Sprite LeftImage;
        public Sprite RightImage;
    }
}
