using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;
using System;
using static Quat.QuatFuncs;

public class LMMController : MonoBehaviour
{

    [Header("Model Debug")]
    [SerializeField]
    public bool stepperActive = true;
    [SerializeField]
    public bool lmmEnabled = false;

    [SerializeField]
    public List<float> modelOutput = new List<float>();

    // In the unity editor assign each .onnx model to these
    [Header("LMM Models")]
    [SerializeField]
    public NNModel projectorModelAsset;
    [SerializeField]
    public NNModel decompressorModelAsset;
    [SerializeField]
    public NNModel stepperModelAsset;
    
    // How many frames between each time LMM is done
    [SerializeField]
    public int frameInterval = 20;

    // Frames elapsed between each frameInterval
    private int steps;

    // In the unity editor assign corresponding text file to these
    [Header("Normalising data")]
    [SerializeField]
    public TextAsset projectorInputMean;
    [SerializeField]
    public TextAsset projectorInputStd;
    [SerializeField]
    public TextAsset projectorOutputMean;
    [SerializeField]
    public TextAsset projectorOutputStd;
    [SerializeField]
    public TextAsset stepperInputMean;
    [SerializeField]
    public TextAsset stepperInputStd;
    [SerializeField]
    public TextAsset stepperOutputMean;
    [SerializeField]
    public TextAsset stepperOutputStd;
    [SerializeField]
    public TextAsset decompressorInputMean;
    [SerializeField]
    public TextAsset decompressorInputStd;
    [SerializeField]
    public TextAsset decompressorOutputMean;
    [SerializeField]
    public TextAsset decompressorOutputStd;

    // Access script for model interaction
    LMM lmm = new LMM();

    // Access skeleton to be updated
    SkeletonController skeleton;

    // The feature vector retrieved from the skeleton controller
    public List<float> modelInput;

    // Model input and output tensors
    private Tensor decompressorInput = new Tensor(1, 1, 59, 1);
    private Tensor decompressorOutput = new Tensor(1, 1, 338, 1);
    private Tensor projectorInput = new Tensor(1, 1, 27, 1); 
    private Tensor projectorOutput = new Tensor(1, 1, 59, 1);
    private Tensor stepperInput = new Tensor(1, 1, 59, 1);
    private Tensor stepperOutput = new Tensor(1, 1, 59, 1);

    // For updating joints
    private bool updateSkeleton = true;

    void Start()
    {   
        skeleton = GetComponent<SkeletonController>();

        // At the start of the program we want to ensure the projector is used and not stepper
        steps = frameInterval;

        // Tell the LMM script load the models
        lmm.LoadModels(projectorModelAsset, decompressorModelAsset, stepperModelAsset);

        // Tell the LMM script to load the data used to normalise models' input/output
        lmm.LoadNormalData(projectorInputMean,
                            projectorInputStd,
                            projectorOutputMean,
                            projectorOutputStd,
                            stepperInputMean,
                            stepperInputStd,
                            stepperOutputMean,
                            stepperOutputStd,
                            decompressorInputMean,
                            decompressorInputStd,
                            decompressorOutputMean,
                            decompressorOutputStd);
    }

    void Update()
    {
        // Every n frames, get the skeleton state and perform LMM with projector+decompressor otherwise use stepper+decompressor
        if (steps < frameInterval)
        {
            if (stepperActive)
            {
                LMMStep();

                updateSkeleton = true;
            }
            steps++;

        } else {
            steps = 0;

            CreateInputVector();
            LMM();

            updateSkeleton = true;
        }
    }

    void LateUpdate()
    {
        // Can probably be moved to Update()
        if (updateSkeleton && lmmEnabled)
        {
            UpdateSkeleton();
            updateSkeleton = false;
        }     
    }

    void OnDestroy()
    {
        lmm.DisposeWorkers();

        decompressorInput.Dispose();
        decompressorOutput.Dispose();

        projectorInput.Dispose();
        projectorOutput.Dispose();

        stepperInput.Dispose();
        stepperOutput.Dispose();
    }

    private void CreateInputVector()
    {

        // Get model input from the skeleton
        modelInput = new List<float>() {

            skeleton.leftFootPosition.x,
            skeleton.leftFootPosition.y,
            skeleton.leftFootPosition.z,

            skeleton.rightFootPosition.x,
            skeleton.rightFootPosition.y,
            skeleton.rightFootPosition.z,

            skeleton.leftFootVelocity.x,
            skeleton.leftFootVelocity.y,
            skeleton.leftFootVelocity.z,

            skeleton.rightFootVelocity.x,
            skeleton.rightFootVelocity.y,
            skeleton.rightFootVelocity.z,

            skeleton.hipsVelocity.x,
            skeleton.hipsVelocity.y,
            skeleton.hipsVelocity.z,

            skeleton.positionIn20Frames.x,
            skeleton.positionIn20Frames.z,

            skeleton.positionIn40Frames.x,
            skeleton.positionIn40Frames.z,

            skeleton.positionIn60Frames.x,
            skeleton.positionIn60Frames.z,

            skeleton.directionIn20Frames.x,
            skeleton.directionIn20Frames.z,

            skeleton.directionIn40Frames.x,
            skeleton.directionIn40Frames.z,

            skeleton.directionIn60Frames.x,
            skeleton.directionIn60Frames.z,


        };

        // Convert to tensor
        for (int i = 0; i < projectorInput.length; i++)
            projectorInput[i] = modelInput[i];
    }

    private void LMM()
    {   
        // Execute projector
        lmm.ExecuteProjector(projectorInput, ref projectorOutput);

        // Execute decompressor
        decompressorInput = projectorOutput.DeepCopy();
        stepperInput = projectorOutput.DeepCopy();

        lmm.ExecuteDecompressor(decompressorInput, ref decompressorOutput);
    }

    private void LMMStep()
    {
        modelOutput = new List<float>();

        // Execute stepper
        lmm.ExecuteStepper(stepperInput, ref stepperOutput, 1.0f/60.0f);
        stepperInput = stepperOutput.DeepCopy();

        // Execute decompressor
        decompressorInput = stepperOutput.DeepCopy();
        lmm.ExecuteDecompressor(decompressorInput, ref decompressorOutput);
    }

    private void UpdateSkeleton()
    {
        List<Vector3> newPositions = new List<Vector3>(new Vector3[skeleton.jointsCount]);
        List<Quaternion> newRotations = new List<Quaternion>(new Quaternion[skeleton.jointsCount]);

        // Transform the model output to update the pose
        ReconstructPose(ref newPositions, ref newRotations);

        // Move the skeleton
        skeleton.UpdatePose(newPositions, newRotations);
    }

    private void ReconstructPose(ref List<Vector3>  newPositions, ref List<Quaternion> newRotations)
	{
        
        // List<Quaternion> newVelocities;
        // List<Quaternion> newAngularVelocities;

        // Extract bone positions
        int offset = 0;
        for (int i = 0; i < skeleton.jointsCount - 1; i++)
        {
            newPositions[i + 1] = new Vector3(
                decompressorOutput[offset + i * 3 + 0],
                decompressorOutput[offset + i * 3 + 1],
                decompressorOutput[offset + i * 3 + 2]);
        }
        offset += (skeleton.jointsCount - 1) * 3;

        // Extract bone rotations, convert from 2-axis representation
        for (int i = 0; i < skeleton.jointsCount - 1; i++)
        {
            newRotations[i + 1] = quat_from_xform_xy(
                new Vector3(decompressorOutput[offset + i * 6 + 0],
                     decompressorOutput[offset + i * 6 + 2],
                     decompressorOutput[offset + i * 6 + 4]),
                new Vector3(decompressorOutput[offset + i * 6 + 1],
                     decompressorOutput[offset + i * 6 + 3],
                     decompressorOutput[offset + i * 6 + 5]));
        }
        offset += (skeleton.jointsCount - 1) * 6;

        offset += (skeleton.jointsCount - 1) * 3;

        offset += (skeleton.jointsCount - 1) * 3;

        Vector3 root_velocity = quat_mul_vec3(skeleton.GetRootRotation(), new Vector3(
            decompressorOutput[offset + 0],
            decompressorOutput[offset + 1],
            decompressorOutput[offset + 2]));

        Vector3 root_angular_velocity = quat_mul_vec3(skeleton.GetRootRotation(), new Vector3(
            decompressorOutput[offset + 3],
            decompressorOutput[offset + 4],
            decompressorOutput[offset + 5]));

        offset += 6;

        // Find new root position/rotation/velocities etc.
        newPositions[0] = 0.0167f * root_velocity + skeleton.GetRootPosition();//root_position;
        newRotations[0] = quat_mul(quat_from_scaled_angle_axis(root_angular_velocity * 0.0167f), skeleton.GetRootRotation());//root_rotation);

    }
}
