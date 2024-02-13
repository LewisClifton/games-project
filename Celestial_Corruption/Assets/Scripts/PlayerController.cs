using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public enum PlayerState
{
    Normal,
    Gliding,
    Dashing
}

public class PlayerController : MonoBehaviour
{
    public PlayerState playerState;

    [SerializeField] private MonoBehaviour normalMovement;
    [SerializeField] private MonoBehaviour glidingMovement;
    [SerializeField] private MonoBehaviour dashingMovement;

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // dashingMovement.enabled = true;
        normalMovement.enabled = true;
    }   

    private void Start()
    {
        playerState = PlayerState.Normal;
    }

    private void Update()
    {

    }

    private void DeactivateAllMovements()
    {
        normalMovement.enabled = false;
        glidingMovement.enabled = false;
        // Dashing can be active all the time
        // dashingMovement.enenabled = false;
    }

    public void SetPlayerState(PlayerState state)
    {
        DeactivateAllMovements();
        switch (state)
        {
            case PlayerState.Normal:
                playerState = PlayerState.Normal;
                normalMovement.enabled = true;
                break;
            case PlayerState.Gliding:
                playerState = PlayerState.Gliding;
                glidingMovement.enabled = true;
                break;
            case PlayerState.Dashing:
                playerState = PlayerState.Dashing;
                dashingMovement.enabled = true;
                break;
        }
    }
}