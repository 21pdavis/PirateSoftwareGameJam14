using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    internal Animator animator;
    internal string currentAnim;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        ChangeAnimState("idle_gun");
    }

    internal void ChangeAnimState(string animName)
    {
        if (currentAnim == animName)
            return;

        //Debug.Log($"Changing to {animName}");
        animator.Play(animName);
        currentAnim = animName;
    }

    internal void CrossfadeAnimState(string animName, float normalizedTransitionDuration)
    {
        if (currentAnim == animName)
            return;

        animator.CrossFade(animName, normalizedTransitionDuration);
        // TODO: may need a delay here, but likely not
        currentAnim = animName;
    }
}
