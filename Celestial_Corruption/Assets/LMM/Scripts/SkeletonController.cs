using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Quat.QuatFuncs;

public class SkeletonController : MonoBehaviour
{
    // Features to be used by LMM
    public Vector3 leftFootVelocity;
    public Vector3 rightFootVelocity;
    public Vector3 hipsVelocity;
    public Vector3 positionIn20Frames;
    public Vector3 positionIn40Frames;
    public Vector3 positionIn60Frames;
    public Vector3 directionIn20Frames;
    public Vector3 directionIn40Frames;
    public Vector3 directionIn60Frames;

    // For obtaining input
    [SerializeField] 
    private GameInput gameInput;
    [SerializeField] 
    private Transform cameraTransform;
    [SerializeField]
    private Rigidbody playerBody;

    // Joint information
    private Transform positionRootJoint;
    private Transform rotationRootJoint;
    private Transform hipsJoint;
    private Transform leftFootJoint;
    private Transform rightFootJoint;

    // Previous joint positions (for velocity calculation)
    private Vector3 prevHipsPosition;
    private Vector3 prevLeftFootPosition;
    private Vector3 prevRightFootPosition;

    private List<Vector3> jointRestPositions;
    private List<Quaternion> jointRestRotations;

    void Start()
    {

        jointRestPositions = new List<Vector3>{};
        jointRestRotations = new List<Quaternion>{};

        // Get transforms of important joints in skeleton
        Transform modelHipsTransform = FindDeepChild(transform, "Model:Hips");
        if (modelHipsTransform != null)
        {
            GetJointTransforms(modelHipsTransform);
        }

        UpdatePrevious();
    }

    void Update()
    {
        GetTrajectories();
    }

    [HideInInspector]
    public int jointsCount = 0;

    private void GetJointTransforms(Transform currentJoint)
    {
        jointsCount++;

        jointRestPositions.Add(currentJoint.localPosition);
        jointRestRotations.Add(currentJoint.localRotation);
        
        if (currentJoint.name.Contains("Spine2"))
        {
         
            positionRootJoint = currentJoint;
        }
        if (currentJoint.name.Contains("Hip"))
        {
            hipsJoint = currentJoint;
            rotationRootJoint = currentJoint;
        }
        if (currentJoint.name.Contains("LeftFoot"))
        {
            leftFootJoint = currentJoint;
        }
        if (currentJoint.name.Contains("RightFoot"))
        {
            rightFootJoint = currentJoint;
        }

        // Traverse child joints recursively to find positions of each
        for (int i = 0; i < currentJoint.childCount; i++)
        {
            Transform childJoint = currentJoint.GetChild(i);
            if (JointIsValid(childJoint))
            {
                GetJointTransforms(childJoint);
            }
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = new Color(1.0f, 1.0f, 0.0f, 1f);
        UnityEditor.Handles.color = Color.red;

        Gizmos.DrawWireSphere(positionIn20Frames + positionRootJoint.position, 0.1f);
        Gizmos.DrawWireSphere(positionIn40Frames + positionRootJoint.position, 0.1f);
        Gizmos.DrawWireSphere(positionIn60Frames + positionRootJoint.position, 0.1f);

        Gizmos.DrawWireSphere(hipsJoint.position, 0.1f);
        UnityEditor.Handles.ArrowHandleCap(0, playerBody.position, playerBody.rotation, 5f, EventType.Repaint);
        Gizmos.DrawWireSphere(leftFootJoint.position, 0.1f);
        Gizmos.DrawWireSphere(rightFootJoint.position, 0.1f);
    
    }

    public void GetCurrentPose()
    {   
        GetTrajectories();
        CalculateVelocities();
    }

    private void GetTrajectories()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        // Convert the input vector to a movement vector relative to the camera orientation
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // Adjusted movement vector
        Vector3 direction = (forward * inputVector.y + right * inputVector.x).normalized;
   
        // Predict player position 20, 40, 60 frames into the future
        positionIn20Frames = direction * (1.0f / 60.0f) * (1.0f/3.0f) * 10;
        positionIn40Frames = direction * (1.0f / 60.0f) * (2.0f/3.0f) * 10;
        positionIn60Frames = direction * (1.0f / 60.0f) * (3.0f/3.0f) * 10;

        // Predict player direction 20, 40, 60 frames into the future
        float rotationSpeed = 1f;
        Quaternion toRotation = Quaternion.LookRotation(direction);
        directionIn20Frames = playerBody.rotation * Vector3.forward;
        directionIn40Frames = playerBody.rotation * Vector3.forward;
        directionIn60Frames = playerBody.rotation * Vector3.forward;
    }

    private Vector3 GetLocalSpace(Transform joint)
    {
        // Should be character space
        return joint.position - positionRootJoint.position;
    }

    private void CalculateVelocities()
    {
        // Speed = distance / time = distance / (1/60) = distance * 60

        hipsVelocity = (GetLocalSpace(hipsJoint) - prevHipsPosition) * 60.0f;
        leftFootVelocity = (GetLocalSpace(leftFootJoint) - prevLeftFootPosition) * 60.0f;
        rightFootVelocity = (GetLocalSpace(rightFootJoint) - prevRightFootPosition) * 60.0f;
    }

    public Vector3 GetLeftFootPosition(){ return leftFootJoint.localPosition; }
    public Vector3 GetRightFootPosition(){ return rightFootJoint.localPosition; }
    public Vector3 GetRootPosition(){ return positionRootJoint.position; }
    public Quaternion GetRootRotation() { return rotationRootJoint.rotation; }
    public Quaternion GetCurrentDirection() { return playerBody.rotation;}

    // For updating joints pose
    [SerializeField]
    private List<Vector3> newPositions;
    [SerializeField]
    private List<Quaternion> newRotations;
    private int jointIndex;

    public void UpdatePose(List<Vector3> newPostions, List<Quaternion> newRotations)
    {
        this.newPositions = newPostions;
        this.newRotations = newRotations;

        UpdatePrevious();

        jointIndex = -1;
        MoveJoints(hipsJoint);
    }

    private void MoveJoints(Transform currentJoint)
    {
        jointIndex++;
        
        // Position relative to Spine2
        // Rotation relative to hips

        // currentJoint.localPosition = newPositions[jointIndex];
        // currentJoint.localRotation = newRotations[jointIndex];
        
        if (jointIndex == 0)
        {
            currentJoint.localPosition = new Vector3(newPositions[0].x, 0.439f, newPositions[0].z); //+ positionRootJoint.position;
            currentJoint.localRotation = newRotations[0];// * rotationRootJoint.rotation;

        } else {
            currentJoint.localPosition = newPositions[jointIndex];
            currentJoint.localRotation = newRotations[jointIndex];
        }

        // Traverse child joints recursively to move each
        for (int i = 0; i < currentJoint.childCount; i++)
        {   
            Transform childJoint = currentJoint.GetChild(i);

            // Exclude 
            if (JointIsValid(childJoint))
            {
                MoveJoints(childJoint);
            }
        }
    }

    private bool JointIsValid(Transform joint) 
    { 
        return !joint.gameObject.name.Contains("ToeEnd") && 
                !joint.gameObject.name.Contains("InHand") && 
                !joint.gameObject.name.Contains("Thumb");
    }

    private void UpdatePrevious()
    {
        prevHipsPosition = GetLocalSpace(hipsJoint);
        prevLeftFootPosition = GetLocalSpace(leftFootJoint);
        prevRightFootPosition = GetLocalSpace(rightFootJoint);
    }
}

