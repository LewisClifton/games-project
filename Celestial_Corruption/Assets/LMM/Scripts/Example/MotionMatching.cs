using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Barracuda;

namespace MM
{
	[System.Serializable]
	public class MotionMatching
	{
		#region VARIABLES
		
		[System.Serializable]
		public class ClipInfo
		{
			public int frames;
			public float seconds;
			public float frameRate;

			/// <summary>
			/// Stored animation-curves info<br/>
			/// Key: bone name<br/>
			/// Value: animation curves about position/rotation/scale
			/// </summary>
			public Dictionary<string, CurvesInfo> curvesInfo;

			/// <summary>
			/// Features of each frame <br/>
			/// Key:	Frame of the pose <br/>
			/// Value: 	Features of the pose
			/// </summary>
			public Dictionary<int, Features> features;

			/// <summary>
			/// Reconstructed poses from animation-curves and stored in game at 'Poses Database'<br/>
			/// Key:		Frame of the pose<br/>
			/// Dict Key: 	Frame of the pose<br/>
			/// Dict Value: Reconstructed pose
			/// </summary>
			public Dictionary<int, Dictionary<string, GameObject>> reconstructedPose;
		}

		[System.Serializable]
		public class CurvesInfo
		{
			public string boneParent;
			public Transform boneRelative;

			public AnimationCurve positionX;
			public AnimationCurve positionY;
			public AnimationCurve positionZ;

			public AnimationCurve rotationX;
			public AnimationCurve rotationY;
			public AnimationCurve rotationZ;
			public AnimationCurve rotationW;

			public AnimationCurve scaleX;
			public AnimationCurve scaleY;
			public AnimationCurve scaleZ;
		}

		[System.Serializable]
		public class TargetBones
		{
			public Transform character;
			public Transform root;
			public Transform hips;
			public Transform rFoot;
			public Transform lFoot;
		}

		[System.Serializable]
		public class Features
		{
			/// <summary> Nexts 3 trajectory points in the future </summary>
			public Vector3[] trajectory;

            /// <summary> Bones position/rotation </summary>
            public Dictionary<string, Vector3> locBonesT;
            public Dictionary<string, Vector3> chrBonesT;
			public Dictionary<string, Quaternion> locBonesR;
            public Dictionary<string, Quaternion> chrBonesR;

			/// <summary> Local bones speed </summary>
			public Dictionary<string, Vector3> locBonesTVel;
            public Dictionary<string, Vector3> chrBonesTVel;
			public Dictionary<string, Vector3> locBonesRVel;
            public Dictionary<string, Vector3> chrBonesRVel;
		}

		[System.NonSerialized] public Animator animator;
        [System.NonSerialized] public Dictionary<string, ClipInfo> clipInfo = new Dictionary<string, ClipInfo>();

		public AnimationClip idleClip;
		public TargetBones targetBones;

		AnimationClip[] animClips;

        List<float> nnInput = new List<float>();
        public List<List<List<float>>> xData;
        List<List<List<float>>> yData;
        List<List<List<float>>> qData;
        List<List<List<float>>> zData;
        List<List<List<float>>> xVelData = new List<List<List<float>>>();
        List<List<List<float>>> zVelData = new List<List<List<float>>>();

		public NNModel decompressorParams;
		public NNModel stepperParams;
		public NNModel projectorParams;
		
		Model decompressorModel;
		Model stepperModel;
		Model projectorModel;

		IWorker decompressorWorker; 
		IWorker stepperWorker;
		IWorker projectorWorker;

        Tensor decompressorInput;
		Tensor decompressorOutput;

        Tensor projectorInput;
        Tensor projectorOutput;

		Tensor stepperInput;
        Tensor stepperOutput;

		int qLength = 0;
        int njoints = 0;
        int jointInfo = 0;
        [NonSerialized] public bool enableDecompressor = true;
        [NonSerialized] public bool enableStepper = true;
        [NonSerialized] public bool enableProjector = true;
        [NonSerialized] public float projectorFreq = 20;
		public GameObject prefab;
		string folder;

        #endregion

        MotionMatching() {}

		public void Build(GameObject go)
		{    
			animator = go.GetComponent<Animator>();
			animClips = AnimationUtility.GetAnimationClips(go);

            folder = AssetDatabase.GetAssetPath(prefab);
			Debug.Log(folder);
            folder = folder.Substring(0, folder.LastIndexOf('/'));

			LoadRigInfo();
			LoadData("XData", ref xData);
            LoadData("ZData", ref zData);
			LoadData("YtxyData", ref yData);
			LoadData("YData", ref qData);

            jointInfo = yData[0][0].Count / njoints;
            qLength = qData[0][0].Count;
			Debug.Log(yData[0][0].Count + " " + qData[0][0].Count + " " + njoints + " " + jointInfo);

			NormalizeData(ref xData, ref meansX, ref stdsX);
			NormalizeData(ref yData, ref meansY, ref stdsY);
            NormalizeData(ref zData, ref meansZ, ref stdsZ);

            float YPos_scale = ScaleData(ref yData, 0, 3, jointInfo);
            float YRot_scale = ScaleData(ref yData, 3, 9, jointInfo);
            float YVel_scale = ScaleData(ref yData, 9, 12, jointInfo);
            float YAng_scale = ScaleData(ref yData, 12, 15, jointInfo);

			for (int i = 0; i < njoints; i++)
			{
				scalesY.AddRange(Enumerable.Repeat(YPos_scale, 3).ToList());
				scalesY.AddRange(Enumerable.Repeat(YRot_scale, 6).ToList());
				scalesY.AddRange(Enumerable.Repeat(YVel_scale, 3).ToList());
            	scalesY.AddRange(Enumerable.Repeat(YAng_scale, 3).ToList());
			}

            scalesX.AddRange(Enumerable.Repeat(ScaleData(ref xData, 0, 24, 24), 24).ToList());
            scalesZ.AddRange(Enumerable.Repeat(ScaleData(ref zData, 0, 32, 32), 32).ToList());

			for (int i = 0; i < xData.Count; i++)
			{
				List<List<float>> clip = new List<List<float>>();

				for (int j = 1; j < xData[i].Count; j++)
				{
					List<float> row = new List<float>();

					for (int k = 0; k < xData[i][j].Count; k++)
						row.Add((xData[i][j][k] - xData[i][j - 1][k]) / 0.0167f);
					
					clip.Add(row);
				}

				xVelData.Add(clip);
			}

            for (int i = 0; i < zData.Count; i++)
            {
                List<List<float>> clip = new List<List<float>>();

                for (int j = 1; j < zData[i].Count; j++)
                {
                    List<float> row = new List<float>();

                    for (int k = 0; k < zData[i][j].Count; k++)
                        row.Add((zData[i][j][k] - zData[i][j - 1][k]) / 0.0167f);

                    clip.Add(row);
                }

                zVelData.Add(clip);
            }

            NormalizeData(ref xVelData, ref meansXVel, ref stdsXVel);
            NormalizeData(ref zVelData, ref meansZVel, ref stdsZVel);

			decompressorInput  	= new Tensor(1, 1, 56, 1);
			decompressorOutput 	= new Tensor(1, 1, qLength, 1);

			projectorInput 		= new Tensor(1, 1, 24, 1);
			projectorOutput 	= new Tensor(1, 1, 56, 1);

			stepperInput 		= new Tensor(1, 1, 56, 1);
            stepperOutput 		= new Tensor(1, 1, 56, 1);

			decompressorModel = ModelLoader.Load(decompressorParams);
			decompressorWorker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, decompressorModel);
			
			stepperModel = ModelLoader.Load(stepperParams);
			stepperWorker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, stepperModel);

			projectorModel = ModelLoader.Load(projectorParams);
			projectorWorker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, projectorModel);
		}

        /// <summary> Exports dataset to .txt files </summary>
        public void ExtractData(GameObject go) 
		{
            animator = go.GetComponent<Animator>();
            animClips = AnimationUtility.GetAnimationClips(go);

            GameObject prefabParent = PrefabUtility.GetCorrespondingObjectFromSource(go);
            folder = AssetDatabase.GetAssetPath(prefabParent);
            folder = folder.Substring(0, folder.LastIndexOf('/'));

            LoadRigInfo();
            GenerateFeatures();
            ExportData();
        }

        /// <summary> Load character rig and animation-clip data </summary>
        void LoadRigInfo()
		{
			if (clipInfo.Count > 0) return;

            for (int i = 0; i < animClips.Length; i++)
            {
                clipInfo.Add(animClips[i].name, new ClipInfo());
				var clip = clipInfo[animClips[i].name];

                clip.curvesInfo = new Dictionary<string, CurvesInfo>();

                clip.frames = Mathf.FloorToInt(animClips[i].length * animClips[i].frameRate);
                clip.seconds = animClips[i].length;
                clip.frameRate = animClips[i].frameRate;

                // acess animation-curves
                foreach (EditorCurveBinding binding in AnimationUtility.GetCurveBindings(animClips[i]))
                {
                    string[] path = binding.path.Split('/');
                    string curBone = path.Last();

                    // build hierarchy
                    if (curBone == "") curBone = targetBones.character.name;

                    if (!clip.curvesInfo.ContainsKey(curBone))
                    {
                        clip.curvesInfo.Add(curBone, new CurvesInfo());

                        if (path.Length == 1 && curBone != targetBones.character.name)
                        {
                            clip.curvesInfo[curBone].boneParent = targetBones.character.name;
                        }
                        else if (path.Length > 1)
                        {
                            clip.curvesInfo[curBone].boneParent = path[path.Length - 2];
                        }
                    }

                    clip.curvesInfo[curBone].boneRelative = GameObject.Find(targetBones.character.name + '/' + binding.path).transform;

                    // store animation-curves
					if (curBone == targetBones.character.name)
					{
                        // root
                        if (binding.propertyName == "RootT.x")
                            clip.curvesInfo[curBone].positionX = AnimationUtility.GetEditorCurve(animClips[i], binding);

                        if (binding.propertyName ==  "RootT.y")
                            clip.curvesInfo[curBone].positionY = AnimationUtility.GetEditorCurve(animClips[i], binding);

                        if (binding.propertyName ==  "RootT.z")
                            clip.curvesInfo[curBone].positionZ = AnimationUtility.GetEditorCurve(animClips[i], binding);

                        if (binding.propertyName ==  "RootQ.x")
                            clip.curvesInfo[curBone].rotationX = AnimationUtility.GetEditorCurve(animClips[i], binding);

                        if (binding.propertyName ==  "RootQ.y")
                            clip.curvesInfo[curBone].rotationY = AnimationUtility.GetEditorCurve(animClips[i], binding);

                        if (binding.propertyName ==  "RootQ.z")
                            clip.curvesInfo[curBone].rotationZ = AnimationUtility.GetEditorCurve(animClips[i], binding);

                        if (binding.propertyName ==  "RootQ.w")
                            clip.curvesInfo[curBone].rotationW = AnimationUtility.GetEditorCurve(animClips[i], binding);
					}
					else 
					{
                        // position
                        if (binding.propertyName ==  "m_LocalPosition.x")
                            clip.curvesInfo[curBone].positionX = AnimationUtility.GetEditorCurve(animClips[i], binding);

                        if (binding.propertyName ==  "m_LocalPosition.y")
                            clip.curvesInfo[curBone].positionY = AnimationUtility.GetEditorCurve(animClips[i], binding);

                        if (binding.propertyName ==  "m_LocalPosition.z")
                            clip.curvesInfo[curBone].positionZ = AnimationUtility.GetEditorCurve(animClips[i], binding);

                        // rotation
                        if (binding.propertyName ==  "m_LocalRotation.x")
                            clip.curvesInfo[curBone].rotationX = AnimationUtility.GetEditorCurve(animClips[i], binding);

                        if (binding.propertyName ==  "m_LocalRotation.y")
                            clip.curvesInfo[curBone].rotationY = AnimationUtility.GetEditorCurve(animClips[i], binding);

                        if (binding.propertyName ==  "m_LocalRotation.z")
                            clip.curvesInfo[curBone].rotationZ = AnimationUtility.GetEditorCurve(animClips[i], binding);

                        if (binding.propertyName ==  "m_LocalRotation.w")
                            clip.curvesInfo[curBone].rotationW = AnimationUtility.GetEditorCurve(animClips[i], binding);

                        // scale
                        if (binding.propertyName ==  "m_LocalScale.x")
                            clip.curvesInfo[curBone].scaleX = AnimationUtility.GetEditorCurve(animClips[i], binding);

                        if (binding.propertyName ==  "m_LocalScale.y")
                            clip.curvesInfo[curBone].scaleY = AnimationUtility.GetEditorCurve(animClips[i], binding);

                        if (binding.propertyName ==  "m_LocalScale.z")
                            clip.curvesInfo[curBone].scaleZ = AnimationUtility.GetEditorCurve(animClips[i], binding);
                    }
                }

				njoints = clip.curvesInfo.Count;
			}
        }

		/// <summary> Generate features of each animation frame </summary>
		void GenerateFeatures()
		{
            GameObject poseHolder = new GameObject();
			poseHolder.name = "Pose database";

            foreach (var clip in clipInfo.Values)
			{
				clip.features = new Dictionary<int, Features>();
				clip.reconstructedPose = new Dictionary<int, Dictionary<string, GameObject>>();

				// store all features but trajectories
				for (int j = 0; j < clip.frames; j++)
				{
					clip.features.Add(j, new Features());
					clip.reconstructedPose.Add(j, new Dictionary<string, GameObject>());

					float ftos = j / clip.frameRate;
					
					Vector3 posInfo;
					Quaternion rotInfo;
					
					Vector3 locTVelInfo;
                    Vector3 chrTVelInfo;

					Vector3 locRVelInfo;
                    Vector3 chrRVelInfo;

					// pose bones loop
					foreach (var boneName in clip.curvesInfo.Keys)
					{
						if (boneName == clip.curvesInfo.Keys.First())
						{
							clip.features[j].locBonesT = new Dictionary<string, Vector3>();
                            clip.features[j].chrBonesT = new Dictionary<string, Vector3>();

							clip.features[j].locBonesR = new Dictionary<string, Quaternion>();
                            clip.features[j].chrBonesR = new Dictionary<string, Quaternion>();

							clip.features[j].locBonesTVel = new Dictionary<string, Vector3>();
                            clip.features[j].chrBonesTVel = new Dictionary<string, Vector3>();

							clip.features[j].locBonesRVel = new Dictionary<string, Vector3>();
                            clip.features[j].chrBonesRVel = new Dictionary<string, Vector3>();
						}

						GameObject go = new GameObject();

						// if (boneName == targetBones.character.name) go.name = "Frame " + j;
						go.name = boneName;

						if (clip.curvesInfo[boneName].boneParent != null) 
						{
							go.transform.parent = clip.reconstructedPose[j][clip.curvesInfo[boneName].boneParent].transform;
						}
						else 
						{
							go.transform.parent = poseHolder.transform;
						}

						// store positions and rotations
						if (boneName != targetBones.root.name)
						{
							posInfo = new Vector3(	clip.curvesInfo[boneName].positionX.Evaluate(ftos),
													clip.curvesInfo[boneName].positionY.Evaluate(ftos),
													clip.curvesInfo[boneName].positionZ.Evaluate(ftos));

							rotInfo = new Quaternion(clip.curvesInfo[boneName].rotationX.Evaluate(ftos),
													clip.curvesInfo[boneName].rotationY.Evaluate(ftos),
													clip.curvesInfo[boneName].rotationZ.Evaluate(ftos),
													clip.curvesInfo[boneName].rotationW.Evaluate(ftos));
                            if (boneName != targetBones.character.name)
                            {
                            go.transform.localScale = new Vector3(	clip.curvesInfo[boneName].scaleX.Evaluate(ftos),
                            										clip.curvesInfo[boneName].scaleY.Evaluate(ftos),
                            										clip.curvesInfo[boneName].scaleZ.Evaluate(ftos));
                            }
                            else go.transform.localScale = Vector3.one;
                            // go.transform.localScale = Vector3.one;
						}
						else
						{
							posInfo = targetBones.root.localPosition;
							rotInfo = targetBones.root.localRotation;
                            go.transform.localScale = targetBones.root.localScale;
						}

						go.transform.localPosition = posInfo;
						go.transform.localRotation = rotInfo;

						clip.reconstructedPose[j].Add(boneName, go);

						clip.features[j].locBonesT.Add(boneName, go.transform.localPosition);
                        clip.features[j].chrBonesT.Add(boneName, clip.reconstructedPose[j][targetBones.character.name].transform.InverseTransformPoint(go.transform.position));

						// Vector3 quatToEuler1 = go.transform.localEulerAngles * Mathf.Deg2Rad;
                        // Vector3 quatToEuler2 = go.transform.eulerAngles      * Mathf.Deg2Rad;
                        
						clip.features[j].locBonesR.Add(boneName, go.transform.localRotation);
                        clip.features[j].chrBonesR.Add(boneName, go.transform.rotation);

                        if (j == 0) continue;

						// calculate velocities
						locTVelInfo = clip.features[j].locBonesT[boneName] - clip.features[j - 1].locBonesT[boneName];
						locTVelInfo /= 0.017f;

                        chrTVelInfo = clip.features[j].chrBonesT[boneName] - clip.features[j - 1].chrBonesT[boneName];
                        chrTVelInfo /= 0.017f;

						locRVelInfo = clip.features[j].locBonesR[boneName].eulerAngles - clip.features[j - 1].locBonesR[boneName].eulerAngles;
						locRVelInfo /= 0.017f;

                        chrRVelInfo = clip.features[j].chrBonesR[boneName].eulerAngles - clip.features[j - 1].chrBonesR[boneName].eulerAngles;
                        chrRVelInfo /= 0.017f;

						clip.features[j].locBonesTVel.Add(boneName, locTVelInfo);
                        clip.features[j].chrBonesTVel.Add(boneName, chrTVelInfo);

						clip.features[j].locBonesRVel.Add(boneName, locRVelInfo);
                        clip.features[j].chrBonesRVel.Add(boneName, chrRVelInfo);

						if (j == 1)
						{
                            clip.features[0].locBonesTVel.Add(boneName, clip.features[1].locBonesTVel[boneName]);
                            clip.features[0].chrBonesTVel.Add(boneName, clip.features[1].chrBonesTVel[boneName]);

							clip.features[0].locBonesRVel.Add(boneName, clip.features[1].locBonesRVel[boneName]);
                            clip.features[0].chrBonesRVel.Add(boneName, clip.features[1].chrBonesRVel[boneName]);
						}
					}
				}

				// store trajectories features
				for (int j = 0; j < clip.frames; j++)
				{
					clip.features[j].trajectory = new Vector3[3];

					if (j < clip.frames - 60)
					{
						clip.features[j].trajectory[0] = clip.reconstructedPose[j + 20][targetBones.character.name].transform.position - clip.reconstructedPose[j][targetBones.character.name].transform.position;
						clip.features[j].trajectory[1] = clip.reconstructedPose[j + 40][targetBones.character.name].transform.position - clip.reconstructedPose[j][targetBones.character.name].transform.position;
						clip.features[j].trajectory[2] = clip.reconstructedPose[j + 60][targetBones.character.name].transform.position - clip.reconstructedPose[j][targetBones.character.name].transform.position;
					}
					else
					{
						clip.features[j].trajectory[0] = clip.features[j - 1].trajectory[0];
						clip.features[j].trajectory[1] = clip.features[j - 1].trajectory[1];
						clip.features[j].trajectory[2] = clip.features[j - 1].trajectory[2];
					}
				}
				
				foreach (var dict in clip.reconstructedPose.Values)
				{
					foreach (var obj in dict.Values)
						MonoBehaviour.DestroyImmediate(obj);
				}
			}
			
			MonoBehaviour.DestroyImmediate(poseHolder);
		}

		List<float> meansX = new List<float>();
		List<float> stdsX  = new List<float>();
        List<float> scalesX = new List<float>();

        List<float> meansY = new List<float>();
        List<float> stdsY = new List<float>();
        List<float> scalesY = new List<float>();

        List<float> meansQ = new List<float>();
        List<float> stdsQ = new List<float>();
        List<float> scalesQ = new List<float>();

        List<float> meansZ = new List<float>();
        List<float> stdsZ = new List<float>();
        List<float> scalesZ = new List<float>();

        List<float> meansXVel = new List<float>();
        List<float> stdsXVel = new List<float>();

        List<float> meansZVel = new List<float>();
        List<float> stdsZVel = new List<float>();

		void NormalizeData(ref List<List<List<float>>> data, ref List<float> means, ref List<float> stds)
		{
            for (int k = 0; k < data[0][0].Count; k++)
			{
				double mean = 0;
				double std  = 0;
				int n = 0;

                for (int i = 0; i < clipInfo.Count; i++)
				{
					for (int j = 0; j < data[i].Count; j++)
					{
						mean += data[i][j][k];
						n++;
					}
				}
				
				mean /= n;
				
				for (int i = 0; i < clipInfo.Count; i++)
				{
					for (int j = 0; j < data[i].Count; j++)
						std += Mathf.Pow(data[i][j][k] - (float)mean, 2f);
				}
				
				std = Mathf.Sqrt((float)std / n) + 0.001f;

				means.Add((float)mean);
				stds.Add((float)std);
			}
		}

		void NormalizeSample(ref List<float> data, ref List<float> means, ref List<float> stds)
		{
			for (int i = 0; i < data.Count; i++)
            	data[i] = ((float) (data[i] - means[i])) / ((float) stds[i]);
        }

		float ScaleData(ref List<List<List<float>>> data, int start, int stop, int step)
		{
			double mean = 0f;
			double std = 0f;
			int n = 0;
			
			for (int p = 0; p < data[0][0].Count; p += step)
			{
				for (int c = 0; c < data.Count; c++)
					for (int r = 0; r < data[c].Count; r++)
                        for (int f = p + start; f < p + stop; f++)
						{
							mean += data[c][r][f];
							n++;
						}
			}

			mean /= n;

            for (int p = 0; p < data[0][0].Count; p += step)
            {
                for (int c = 0; c < data.Count; c++)
                    for (int r = 0; r < data[c].Count; r++)
                        for (int f = p + start; f < p + stop; f++)
                        {
        					std += Mathf.Pow(data[c][r][f] - (float)mean, 2);
						}
			}

        	std = Mathf.Sqrt((float)std / (n - 1));
			return (float)std;
		}

		void GatherXData(string clipName, int frame, ref List<float> list)
		{
			list = new List<float>() { 
							// TRAJECTORY
							clipInfo[clipName].features[frame].trajectory[0].x,
							clipInfo[clipName].features[frame].trajectory[0].y,
							clipInfo[clipName].features[frame].trajectory[0].z,
							clipInfo[clipName].features[frame].trajectory[1].x,
							clipInfo[clipName].features[frame].trajectory[1].y,
							clipInfo[clipName].features[frame].trajectory[1].z,
							clipInfo[clipName].features[frame].trajectory[2].x,
							clipInfo[clipName].features[frame].trajectory[2].y,
							clipInfo[clipName].features[frame].trajectory[2].z,

							// HIPS VEL
							clipInfo[clipName].features[frame].chrBonesTVel[targetBones.hips.name].x,
							clipInfo[clipName].features[frame].chrBonesTVel[targetBones.hips.name].y,
							clipInfo[clipName].features[frame].chrBonesTVel[targetBones.hips.name].z,

							// RIGHT FOOT
							clipInfo[clipName].features[frame].chrBonesT[targetBones.rFoot.name].x,
                            clipInfo[clipName].features[frame].chrBonesT[targetBones.rFoot.name].y,
                            clipInfo[clipName].features[frame].chrBonesT[targetBones.rFoot.name].z,

                            clipInfo[clipName].features[frame].chrBonesTVel[targetBones.rFoot.name].x,
                            clipInfo[clipName].features[frame].chrBonesTVel[targetBones.rFoot.name].y,
                            clipInfo[clipName].features[frame].chrBonesTVel[targetBones.rFoot.name].z,

							// LEFT FOOT
							clipInfo[clipName].features[frame].chrBonesT[targetBones.lFoot.name].x,
                            clipInfo[clipName].features[frame].chrBonesT[targetBones.lFoot.name].y,
                            clipInfo[clipName].features[frame].chrBonesT[targetBones.lFoot.name].z,

                            clipInfo[clipName].features[frame].chrBonesTVel[targetBones.lFoot.name].x,
                            clipInfo[clipName].features[frame].chrBonesTVel[targetBones.lFoot.name].y,
                            clipInfo[clipName].features[frame].chrBonesTVel[targetBones.lFoot.name].z
						};
		}

        void GatherYData(string clipName, int frame, ref List<float> list)
        {
            list = new List<float>();

            foreach (var boneName in clipInfo[clipName].curvesInfo.Keys)
            {
                // if root
                if (boneName == targetBones.character.name)
                {
                    list.Add(clipInfo[clipName].features[frame].locBonesTVel[boneName].x);
                    list.Add(clipInfo[clipName].features[frame].locBonesTVel[boneName].y);
                    list.Add(clipInfo[clipName].features[frame].locBonesTVel[boneName].z);
                }
                else
                {
                    // translation
                    list.Add(clipInfo[clipName].features[frame].locBonesT[boneName].x);
                    list.Add(clipInfo[clipName].features[frame].locBonesT[boneName].y);
                    list.Add(clipInfo[clipName].features[frame].locBonesT[boneName].z);
                }

                // rotation
                list.Add(clipInfo[clipName].features[frame].locBonesR[boneName].w);
                list.Add(clipInfo[clipName].features[frame].locBonesR[boneName].x);
                list.Add(clipInfo[clipName].features[frame].locBonesR[boneName].y);
                list.Add(clipInfo[clipName].features[frame].locBonesR[boneName].z);

                // translation speed
                list.Add(clipInfo[clipName].features[frame].locBonesTVel[boneName].x);
                list.Add(clipInfo[clipName].features[frame].locBonesTVel[boneName].y);
                list.Add(clipInfo[clipName].features[frame].locBonesTVel[boneName].z);

                // rotation speed
                list.Add(clipInfo[clipName].features[frame].locBonesRVel[boneName].x);
                list.Add(clipInfo[clipName].features[frame].locBonesRVel[boneName].y);
                list.Add(clipInfo[clipName].features[frame].locBonesRVel[boneName].z);
            }
        }

        void GatherHierarchyData(string clipName, ref List<float> list)
        {
            list = new List<float>();
            Dictionary<string, int> boneID = new Dictionary<string, int>();
            int i = 0;

            foreach (var boneName in clipInfo[clipName].curvesInfo.Keys)
            {
                if (i == 0)
                {
                    list.Add(i);
                    boneID.Add(boneName, i++);
                    continue;
                }

                list.Add(boneID[clipInfo[clipName].curvesInfo[boneName].boneParent]);
                boneID.Add(boneName, i++);
            }
        }

        void LoadData(string filename, ref List<List<List<float>>> data)
        {
            TextReader file = File.OpenText(Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/') + 1) + folder + "/Database/" + filename + ".txt");

            data = new List<List<List<float>>>();
            List<List<float>> clip = new List<List<float>>();

            string line = file.ReadLine();

            while (line != null)
            {
                List<float> lineItems = new List<float>();

                if (line.Replace(".", ",").Replace("E", "e").Split(' ')[0] == "")
                {
                    data.Add(new List<List<float>>(clip));
					clip.Clear();
                    line = file.ReadLine();
                    continue;
                }

                foreach (var _item in line.Replace(".", ",").Replace("E", "e").Split(' ').ToList())
                    lineItems.Add(float.Parse(_item));

                clip.Add(lineItems);
                line = file.ReadLine();
            }
        }

		void ExportData()
		{
			string pathDatabase  = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/') + 1) + folder + "/Database";

            int yDataLen = (clipInfo[idleClip.name].curvesInfo.Count * 13);
			int poseLen = (clipInfo[idleClip.name].curvesInfo.Count * 7);

			Debug.Log("Y Length:    " + yDataLen);
            Debug.Log("Clips count: " + clipInfo.Count);
			Debug.Log("Bones count: " + clipInfo[idleClip.name].curvesInfo.Count);
			foreach(var clip in clipInfo)
				Debug.Log(clip.Key + ": " + clip.Value.frames + " frames");

			bool generateDatabase = true;

			if (generateDatabase)
			{
                List<float> input  = new List<float>();
                List<float> output = new List<float>();

                string path1;
				string content;

				#region Features Database
				path1 = pathDatabase + "/XData.txt";
				File.WriteAllText(path1, "");

				foreach (var clip in clipInfo)
				{
					for (int i = 0; i < clip.Value.frames; i++)
					{
						content = "";

						// STORING DATA
						GatherXData(clip.Key, i, ref input);

						// WRITTING DATA TO FILE
						for (int j = 0; j < input.Count - 1; j++)
							content += input[j].ToString().Replace(",", ".") + " ";
						content += input[input.Count - 1].ToString().Replace(",", ".");

						content += "\n";
						File.AppendAllText(path1, content);
					}

					File.AppendAllText(path1, "\n");
				}
                #endregion

                #region Compressor Database
                path1 = pathDatabase + "/YData.txt";
                File.WriteAllText(path1, "");

				foreach (var clip in clipInfo)
				{
					for (int i = 0; i < clip.Value.frames; i++)
					{
						content = "";

						// STORING DATA
						GatherYData(clip.Key, i, ref input);

						// WRITTING DATA TO FILE
						for (int j = 0; j < input.Count - 1; j++)
							content += input[j].ToString().Replace(",", ".") + " ";
						content += input[input.Count - 1].ToString().Replace(",", ".");

						content += "\n";
						File.AppendAllText(path1, content);
					}

					File.AppendAllText(path1, "\n");
				}
                #endregion

                #region Hierarchy Database
                path1 = pathDatabase + "/HierarchyData.txt";
				File.WriteAllText(path1, "");
				content = "";

				// STORING DATA
				GatherHierarchyData(idleClip.name, ref input);

				// WRITTING DATA TO FILE
				for (int j = 0; j < input.Count; j++)
					content += input[j].ToString() + "\n";

				File.AppendAllText(path1, content);

				#endregion
			}
        }

		float updater = -1;
        int frame = 0;
		int choosenClip = 0;

		Vector3 currPos;
		Vector3 currVel;
		Vector3 prevHips;
        Vector3 prevRFoot;
        Vector3 prevLFoot;
		Vector3 hips;
        Vector3 RFoot;
		Vector3 LFoot;
		float frameRateCount;

		Vector3 _fast_cross(Vector3 a, Vector3 b)
		{
			return new Vector3(
				a.y*b.z - a.z*b.y,
				a.z*b.x - a.x*b.z,
				a.x*b.y - a.y*b.x);
		}

		public void GetUserQuery(ref MMInput.MMInput input)
		{
            nnInput.Clear();

            hips  = targetBones.character.InverseTransformPoint(targetBones.hips.position);
            RFoot = targetBones.character.InverseTransformPoint(targetBones.rFoot.position);
            LFoot = targetBones.character.InverseTransformPoint(targetBones.lFoot.position);
			float dt = 1f / 60f;

            if (updater == 0)
            {
            	prevHips  = hips;
            	prevRFoot = RFoot;
            	prevLFoot = LFoot;
            }

            nnInput.AddRange(new List<float> () {
            	// TRAJECTORY
            	input.trajectory[0].x,
            	input.trajectory[0].y,
            	input.trajectory[0].z,
            	input.trajectory[1].x,
            	input.trajectory[1].y,
            	input.trajectory[1].z,
            	input.trajectory[2].x,
            	input.trajectory[2].y,
            	input.trajectory[2].z,

            	// HIPS VEL
            	(hips.x - prevHips.x) / dt,
            	(hips.y - prevHips.y) / dt,
            	(hips.z - prevHips.z) / dt,

            	// RIGHT FOOT
            	RFoot.x,
            	RFoot.y,
            	RFoot.z,

            	(RFoot.x - prevRFoot.x) / dt,
            	(RFoot.y - prevRFoot.y) / dt,
            	(RFoot.z - prevRFoot.z) / dt,

            	// LEFT FOOT
            	LFoot.x,
            	LFoot.y,
            	LFoot.z,

            	(LFoot.x - prevLFoot.x) / dt,
            	(LFoot.y - prevLFoot.y) / dt,
            	(LFoot.z - prevLFoot.z) / dt
            });
		}

		public void Matching(ref MMInput.MMInput input)
		{
            // frameRateCount += Time.deltaTime;

			// if (frameRateCount < 0.017f) return;

			// frameRateCount = 0;
            updater++;

			#region DEBUG
			if (Input.GetKeyDown(KeyCode.K)) choosenClip++;

			if (choosenClip >= clipInfo.Count) choosenClip = 0;
			#endregion

			if (updater == 0)
				currPos = targetBones.character.transform.position;

			frame = (int) updater % xData[choosenClip].Count;

            #region MOTION MATCH
            if (updater % projectorFreq == 0)
			{
            	#region DECOMPRESSOR INPUT (DEBUG ONLY)
				if (!enableProjector)
				{
					nnInput = new List<float>(xData[choosenClip][frame]);
                    nnInput.AddRange(zData[choosenClip][frame]);
				}
                #endregion
                #region PROJECTOR
				else
				{
					if (enableStepper)
					{
                		GetUserQuery(ref input);
					}
					else
					{
						nnInput = new List<float>(xData[choosenClip][frame]);
                        // NormalizeSample(ref nnInput, ref meansX, ref stdsX);
                    }

                	for (int w = 0; w < 24; w++)
                    	projectorInput[0, 0, w, 0] = (nnInput[w] - meansX[w]) / scalesX[w];

					projectorWorker.Execute(projectorInput);
					projectorOutput = projectorWorker.PeekOutput("y");

                    nnInput = Enumerable.Repeat(0f, 56).ToList();

                    for (int i = 0; i < nnInput.Count; i++)
                        if (i < 24) nnInput[i] = projectorOutput[i] * stdsX[i] + meansX[i];
                        else 		nnInput[i] = projectorOutput[i] * stdsZ[i - 24] + meansZ[i - 24];
				}
                #endregion

                // stepper <- new user query
                for (int i = 0; i < nnInput.Count; i++)
					stepperInput[i] = nnInput[i];
            }
            #endregion

            #region STEPPER
            if (enableStepper)
            {
				float dt = 1f / 60f;

                for (int i = 0; i < nnInput.Count; i++)
                    if (i < 24) stepperInput[i] = (nnInput[i] - meansX[i]) / scalesX[i];
                    else 		stepperInput[i] = (nnInput[i] - meansZ[i - 24]) / scalesZ[i - 24];

                stepperWorker.Execute(stepperInput);
                stepperOutput = stepperWorker.PeekOutput("y");

				for (int i = 0; i < nnInput.Count; i++)
                    if (i < 24) nnInput[i] += ((stepperOutput[i] * stdsXVel[i]) + meansXVel[i]) * dt;
					else 		nnInput[i] += ((stepperOutput[i] * stdsZVel[i - 24]) + meansZVel[i - 24]) * dt;
            }
			#endregion

            #region PREVIOUS FRAME RELEVANT FEATURES
            if (updater % projectorFreq == projectorFreq - 1)
            {
                prevHips  = targetBones.character.InverseTransformPoint(targetBones.hips.position);
                prevRFoot = targetBones.character.InverseTransformPoint(targetBones.rFoot.position);
                prevLFoot = targetBones.character.InverseTransformPoint(targetBones.lFoot.position);
            }
            #endregion

			if (enableDecompressor)
			{
				#region DECOMPRESSOR
				for (int k = 0; k < 56; k++)
					decompressorInput[0, 0, k, 0] = nnInput[k];

				decompressorWorker.Execute(decompressorInput);
				var decompressorOutput = decompressorWorker.PeekOutput("y");

				for (int k = 0; k < njoints * 15; k++)
					decompressorOutput[k] = (decompressorOutput[k] * stdsY[k]) + meansY[k];
				#endregion

				#region RECONSTRUCT POSE (PREDICTED)
				int j = 0;
				foreach (var bone in clipInfo[idleClip.name].curvesInfo.Values)
				{
					if (bone == clipInfo[idleClip.name].curvesInfo.Values.First()) 
					{
                        currVel = new Vector3(decompressorOutput[j + 0], 0, decompressorOutput[j + 2]);
						bone.boneRelative.localPosition += currVel * 0.017f * 1.00f;
					}
					else bone.boneRelative.localPosition = new Vector3(decompressorOutput[j + 0], decompressorOutput[j + 1], decompressorOutput[j + 2]);

                    Vector3 va = new Vector3(decompressorOutput[j + 3], decompressorOutput[j + 5], decompressorOutput[j + 7]);
                    Vector3 vb = new Vector3(decompressorOutput[j + 4], decompressorOutput[j + 6], decompressorOutput[j + 8]);

                    Vector3 c0, c1, c2;

                    c2 = _fast_cross(va, vb);
                    c2 = c2 / Mathf.Sqrt(c2.x * c2.x + c2.y * c2.y + c2.z * c2.z);
                    c1 = _fast_cross(c2, va);
                    c1 = c1 / Mathf.Sqrt(c1.x * c1.x + c1.y * c1.y + c1.z * c1.z);
                    c0 = va;

                    Quaternion q = new Quaternion();
                    float[,] ts = new float[,] { { c0.x, c1.x, c2.x }, { c0.y, c1.y, c2.y }, { c0.z, c1.z, c2.z } };

                    if (ts[2, 2] < 0)
                    {
                        if (ts[0, 0] > ts[1, 1])
                        {
                            q.w = ts[2, 1] - ts[1, 2];
                            q.x = 1f + ts[0, 0] - ts[1, 1] - ts[2, 2];
                            q.y = ts[1, 0] + ts[0, 1];
                            q.z = ts[0, 2] + ts[2, 0];
                        }
                        else
                        {
                            q.w = ts[0, 2] - ts[2, 0];
                            q.x = ts[1, 0] + ts[0, 1];
                            q.y = 1f - ts[0, 0] + ts[1, 1] - ts[2, 2];
                            q.z = ts[2, 1] + ts[1, 2];
                        }
                    }
                    else
                    {
                        if (ts[0, 0] < -ts[1, 1])
                        {
                            q.w = ts[1, 0] - ts[0, 1];
                            q.x = ts[0, 2] + ts[2, 0];
                            q.y = ts[2, 1] + ts[1, 2];
                            q.z = 1f - ts[0, 0] - ts[1, 1] + ts[2, 2];
                        }
                        else
                        {
                            q.w = 1f + ts[0, 0] + ts[1, 1] + ts[2, 2];
                            q.x = ts[2, 1] - ts[1, 2];
                            q.y = ts[0, 2] - ts[2, 0];
                            q.z = ts[1, 0] - ts[0, 1];
                        }
                    }

                    float norm = Mathf.Sqrt(q.w * q.w + q.x * q.x + q.y * q.y + q.z * q.z) + 1E-8f;
                    q.w /= norm;
                    q.x /= norm;
                    q.y /= norm;
                    q.z /= norm;

                    bone.boneRelative.localRotation = q;
                    j += 15;
				}
				#endregion
			}
			else
			{
                #region RECONSTRUCT POSE (GROUND TRUTH)
                int j = 0;
                foreach (var bone in clipInfo[idleClip.name].curvesInfo.Values)
                {
                    if (bone == clipInfo[idleClip.name].curvesInfo.Values.First())
                    {
                        currVel = new Vector3(yData[choosenClip][frame][j + 0], yData[choosenClip][frame][j + 1], yData[choosenClip][frame][j + 2]);
                        bone.boneRelative.localPosition += currVel * 0.017f * 1.00f;
                    }
                    else bone.boneRelative.localPosition = new Vector3(yData[choosenClip][frame][j + 0], yData[choosenClip][frame][j + 1], yData[choosenClip][frame][j + 2]);

                    Vector3 va = new Vector3(yData[choosenClip][frame][j + 3], yData[choosenClip][frame][j + 5], yData[choosenClip][frame][j + 7]);
                    Vector3 vb = new Vector3(yData[choosenClip][frame][j + 4], yData[choosenClip][frame][j + 6], yData[choosenClip][frame][j + 8]);

                	Vector3 c0, c1, c2;

                	c2 = _fast_cross(va, vb);
                	c2 = c2 / Mathf.Sqrt(c2.x*c2.x + c2.y*c2.y + c2.z*c2.z);
                	c1 = _fast_cross(c2, va);
                	c1 = c1 / Mathf.Sqrt(c1.x*c1.x + c1.y*c1.y + c1.z*c1.z);
                	c0 = va;

                	Quaternion q = new Quaternion();
                    float[,] ts = new float[,] {{c0.x, c1.x, c2.x}, { c0.y, c1.y, c2.y }, { c0.z, c1.z, c2.z }};
                    // float[,] ts = new float[,] {
                	// 	{ yData[choosenClip][frame][j + 3], yData[choosenClip][frame][j + 4], yData[choosenClip][frame][j + 5] }, 
                	// 	{ yData[choosenClip][frame][j + 6], yData[choosenClip][frame][j + 7], yData[choosenClip][frame][j + 8] }, 
                	// 	{ yData[choosenClip][frame][j + 9], yData[choosenClip][frame][j + 10], yData[choosenClip][frame][j + 11] }
                	// };

                    if (ts[2,2] < 0)
                	{
                		if (ts[0,0] > ts[1,1])
                		{
                			q.w = ts[2,1] - ts[1,2];
                			q.x = 1f + ts[0,0] - ts[1,1] - ts[2,2];
                			q.y = ts[1,0] + ts[0,1];
                            q.z = ts[0,2] + ts[2,0];
                		}
                		else
                		{
                			q.w = ts[0,2] - ts[2,0];
                			q.x = ts[1,0] + ts[0,1];
                			q.y = 1f - ts[0,0] + ts[1,1] - ts[2,2];
                            q.z = ts[2,1] + ts[1,2];
                		}
                	}
                	else
                	{
                		if (ts[0,0] < -ts[1,1])
                		{
                			q.w = ts[1,0] - ts[0,1];
                			q.x = ts[0,2] + ts[2,0];
                			q.y = ts[2,1] + ts[1,2];
                            q.z = 1f - ts[0,0] - ts[1,1] + ts[2,2];
                		}
                		else
                		{
                			q.w = 1f + ts[0,0] + ts[1,1] + ts[2,2];
                			q.x = ts[2,1] - ts[1,2];
                			q.y = ts[0,2] - ts[2,0];
                            q.z = ts[1,0] - ts[0,1];						
                		}
                	}

                	float norm = Mathf.Sqrt(q.w*q.w + q.x*q.x + q.y*q.y + q.z*q.z) + 1E-8f;
                	q.w /= norm;
                	q.x /= norm;
                	q.y /= norm;
                    q.z /= norm;

                	// if (frame == 0 && j == 15 * 10)
                	// {
                	// 	Debug.Log(ts[0,0] + " " + ts[0,1] + " " + ts[0,2]);
                	// 	Debug.Log(ts[1,0] + " " + ts[1,1] + " " + ts[1,2]);
                	// 	Debug.Log(ts[2,0] + " " + ts[2,1] + " " + ts[2,2]);

                	// 	Debug.Log(q.w + " " + q.x + " " + q.y + " " + q.z);
                	// }

                    bone.boneRelative.localRotation = q;
                    // bone.boneRelative.localRotation = new Quaternion(qData[choosenClip][frame][j + 4], qData[choosenClip][frame][j + 5], qData[choosenClip][frame][j + 6], qData[choosenClip][frame][j + 3]);
                	j += 15;
                }
                // foreach (var bone in clipInfo[idleClip.name].curvesInfo.Values)
                // {
                //     if (bone == clipInfo[idleClip.name].curvesInfo.Values.First())
                //     {
                //         currVel = new Vector3(qData[choosenClip][frame][j + 0], qData[choosenClip][frame][j + 1], qData[choosenClip][frame][j + 2]);
                //         bone.boneRelative.localPosition += currVel * 0.017f * 1.00f;
                //     }
                //     else bone.boneRelative.localPosition = new Vector3(qData[choosenClip][frame][j + 0], qData[choosenClip][frame][j + 1], qData[choosenClip][frame][j + 2]);

                //     bone.boneRelative.localRotation = new Quaternion(qData[choosenClip][frame][j + 4], qData[choosenClip][frame][j + 5], qData[choosenClip][frame][j + 6], qData[choosenClip][frame][j + 3]);
                // 	j += 13;
                // }
                #endregion
            }
        }

		public void DisposeTensors()
		{
            decompressorInput.Dispose();
            decompressorOutput.Dispose();
            decompressorWorker.Dispose();

            projectorInput.Dispose();
            projectorOutput.Dispose();
            projectorWorker.Dispose();

            stepperInput.Dispose();
            stepperOutput.Dispose();
            stepperWorker.Dispose();
        }
	}
}
