using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraLockOnRotation : MonoBehaviour
{
    public Transform player;
    public Transform boss;
    public CinemachineVirtualCamera lockOnCamera;
    private CinemachineTransposer transposer;

    void Start()
    {
        transposer = lockOnCamera.GetCinemachineComponent<CinemachineTransposer>();
    }

    void Update()
    {
        AdjustCameraBasedOnPlayerPosition();
    }

    void AdjustCameraBasedOnPlayerPosition()
    {
        Vector3 playerToBoss = boss.position - player.position;
        float angle = Vector3.SignedAngle(playerToBoss, transform.forward, Vector3.up);
        playerToBoss.Normalize();
        playerToBoss.y = 0;
        playerToBoss = playerToBoss * 7;
        playerToBoss.y += 1;
        transposer.m_FollowOffset = new Vector3(-playerToBoss.x, playerToBoss.y, -playerToBoss.z);
    }
}
