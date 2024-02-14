using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform player;
    public Transform playerObject;
    public Rigidbody rb;

    public float rotationSpeed;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    void Update()
    {
        turnPlayer();
    }

    private void turnPlayer()
    {
        //takes vector between the camera and the player (ignoring the y axis)
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        //input.GetAxis("Horizontal") and vertical take the inputs wasd, NOT the mouse input. If the player presses 'a', horizontal returns -1, if 'd' then 1
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        //Debug.Log(horizontalInput + " " + verticalInput);

        //checks which way the character should turn
        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //Debug.Log(inputDir);
        if (inputDir != Vector3.zero)
        {
            //Debug.Log(inputDir);
            //slerp interpolates between the first argument transform to the second a the rate of the third argument. Allows for smooth turning of the player.
            playerObject.forward = Vector3.Slerp(playerObject.forward, inputDir.normalized, rotationSpeed * Time.deltaTime);
        }
    }
}
