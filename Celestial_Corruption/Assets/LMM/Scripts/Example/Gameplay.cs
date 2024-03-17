using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using MM;

public class Gameplay : MonoBehaviour
{
    public MotionMatching mm;
    public MMInput.MMInput input;

    Vector3 ctrlSmooth = Vector3.zero;
    Vector3 ctrlRaw;

    float sensitivity;

    Vector3 originalPos;
    Vector3 climbHeight;

    bool DEBUG = false;

    void Awake()
    {
        mm.Build(gameObject);
    }

    void Update()
    {
        if (!DEBUG)
        {
            GetInput();
            mm.Matching(ref input);
        }
    }

    void GetInput()
    {
		// GET INPUT
		ctrlRaw.Set(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

		// INPUT DIRECTION
		if (ctrlRaw.x != 0 || ctrlRaw.z != 0) 	sensitivity = input.acc;
		else 									sensitivity = input.decc;

		if (ctrlRaw.magnitude > 1)
			ctrlRaw.Normalize();

		// FIT WALK LENGTH + RUN LENGTH
		ctrlRaw *= input.defaultLength;

		ctrlSmooth = Vector3.MoveTowards(ctrlSmooth, ctrlRaw, sensitivity * Time.deltaTime);

        // SPLIT INTO 3 POINTS
        input.trajectory[0] = ctrlSmooth * (1 / 3f);
        input.trajectory[1] = ctrlSmooth * (2 / 3f);
        input.trajectory[2] = ctrlSmooth * (3 / 3f);
    }

    void OnDrawGizmos()
    {
		// -- YELLOW
		Gizmos.color = new Color(1.0f, 1.0f, 0.0f, 1f);

		for (int i = 0; i < 3; i++)
		{
			// Vector3 springDamp0 = smoothCD(transform.position, transform.position + input.trajectory[2], Vector3.zero, 1f, i * 1f);
			Gizmos.DrawWireSphere(transform.position + input.trajectory[i], .05f);
		}
    }

	public void ExtractData() 			{ mm.ExtractData(gameObject); }
    public void ToggleDecompressor()   	{ mm.enableDecompressor = !mm.enableDecompressor; }
    public void ToggleStepper() 		{ mm.enableStepper 		= !mm.enableStepper; }
    public void ToggleProjector() 		{ mm.enableProjector 	= !mm.enableProjector; }
    public void UpdateFrequence(InputField field) { mm.projectorFreq = int.Parse(field.text); }

    void OnApplicationQuit()
    {
		mm.DisposeTensors();
    }
}