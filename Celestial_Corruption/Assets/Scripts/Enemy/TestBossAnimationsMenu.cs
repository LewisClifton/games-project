using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAnimationLooper : MonoBehaviour
{
    private Animator animator;
    private List<string> animations = new List<string> { "IsStomping", "IsAiming", "IsCrouching", "IsSpinning", "IsSummoning" };
    
    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(AnimationLoop());
    }

    IEnumerator AnimationLoop()
    {
        while (true) // Infinite loop
        {
            // Wait for the current animation to finish
            yield return new WaitUntil(() => !IsAnyAnimationPlaying());

            // Randomly select and play a new animation
            string selectedAnimation = animations[Random.Range(0, animations.Count)];
            animator.SetBool(selectedAnimation, true);

            // Wait a bit before checking/resetting to give the animation time to start
            yield return new WaitForSeconds(0.1f);

            // Immediately reset the animator state to be ready for the next random animation
            animator.SetBool(selectedAnimation, false);
        }
    }

    private bool IsAnyAnimationPlaying()
    {
        foreach (var animation in animations)
        {
            if (animator.GetBool(animation)) return true;
        }
        return false;
    }
}
