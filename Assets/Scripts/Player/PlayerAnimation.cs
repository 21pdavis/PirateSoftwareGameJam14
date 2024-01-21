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
}
