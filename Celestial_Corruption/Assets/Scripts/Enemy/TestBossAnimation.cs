using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBossAnimation : MonoBehaviour
{
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        bool stomp = Input.GetKey("y");
        bool aim = Input.GetKey("u");
        bool crouch = Input.GetKey("i");
        bool spin = Input.GetKey("o");
        bool summon = Input.GetKey("p");
        if (stomp)
        {
            animator.SetBool("IsStomping", true);
        }else
        {
            animator.SetBool("IsStomping", false);
        }

        if (aim)
        {
            animator.SetBool("IsAiming", true);
        }
        else
        {
            animator.SetBool("IsAiming", false);
        }

        if (crouch)
        {
            animator.SetBool("IsCrouching", true);
        }
        else
        {
            animator.SetBool("IsCrouching", false);
        }

        if (spin)
        {
            animator.SetBool("IsSpinning", true);
        }
        else
        {
            animator.SetBool("IsSpinning", false);
        }

        if (summon)
        {
            animator.SetBool("IsSummoning", true);
        }
        else
        {
            animator.SetBool("IsSummoning", false);
        }
    }
}
