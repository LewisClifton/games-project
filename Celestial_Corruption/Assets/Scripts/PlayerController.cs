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
    private Rigidbody playerBody;
    private PlayerState playerState;

    [SerializeField] private GameObject NormalMovement;
    [SerializeField] private GameObject GlidingMovement;
    [SerializeField] private GameObject DashingMovement;

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        DashingMovement.SetActive(true);
        NormalMovement.SetActive(true);
    }   

    private void Start()
    {
        playerState = PlayerState.Normal;
        playerBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        switch (playerState)
        {
            case PlayerState.Normal:
                NormalMovement.SetActive(true);
                break;
            case PlayerState.Gliding:
                GlidingMovement.SetActive(true);
                break;
            case PlayerState.Dashing:
                DashingMovement.SetActive(true);
                break;
        }
    }

    private void DeactivateAllMovements()
    {
        NormalMovement.SetActive(false);
        GlidingMovement.SetActive(false);
        // Dashing can be active all the time
        // DashingMovement.SetActive(false);
    }

    public void SetPlayerState(PlayerState state)
    {
        DeactivateAllMovements();
        playerState = state;
    }
}