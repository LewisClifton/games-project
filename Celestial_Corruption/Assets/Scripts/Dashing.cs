using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dashing : MonoBehaviour
{

    [Header("References")]
    public Transform orientation;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Rigidbody playerBody;
    private Player pm;

    [Header("Dashing")]
    public float dashForce;
    public float dashUpwardForce;
    public float dashDuration;

    [Header("Cooldown")]
    public float dashCd;
    private float dashCdTimer;

    [Header("Input")]
    [SerializeField] private GameInput gameInput;
    // Start is called before the first frame update
    void Start()
    {
        playerBody=GetComponent<Rigidbody>();
        pm=GetComponent<Player>();
    }
    private void Dash()
    {
        Vector3 forceToApply = playerBody.transform.forward * dashForce + playerBody.transform.up * dashUpwardForce;

        playerBody.AddForce(forceToApply,ForceMode.Impulse);

        Invoke(nameof(ResetDash), dashDuration);
    }

    private void ResetDash()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (gameInput.IsDashPressed())
        {
            Dash();
        }
    }
}