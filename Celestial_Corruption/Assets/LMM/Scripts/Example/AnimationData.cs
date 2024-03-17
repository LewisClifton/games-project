using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationData : EditorWindow
{
	private AnimationClip clip;

	Vector2 scrollPos;

	[MenuItem("Window/Clip Info")]
	static void Init()
	{
		GetWindow(typeof(AnimationData));
	}

	public void OnGUI()
	{
		clip = EditorGUILayout.ObjectField("Clip", clip, typeof(AnimationClip), false) as AnimationClip;

		scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(500), GUILayout.Height(600));
        
        EditorGUILayout.LabelField("Curves:");
        
		if (clip != null)
		{
            // string curBone = "";

			foreach (var binding in AnimationUtility.GetCurveBindings(clip))
			{
                // if (curBone != binding.path)
                // {
                    int sub = binding.path.LastIndexOf('/') >= 0 ? binding.path.LastIndexOf('/') + 1 : 0;

                    AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);
                	// ObjectReferenceKeyframe[] ork = AnimationUtility.(clip, binding);

                    EditorGUILayout.LabelField("Path: " + binding.path + "\t\tProperty: " + binding.propertyName);
					// AnimationUtility
					// curBone = binding.path;
					// curve.Evaluate(1);
                // }
			}

			// EditorCurveBinding[] binding = AnimationUtility.GetCurveBindings(clip);
			// AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding[0]);

			// EditorGUILayout.LabelField(binding.GetLength(0) + " ");
			// EditorGUILayout.LabelField(binding[0].path + "/" + binding[0].propertyName + ", Keys: " + curve.keys.Length);
		}

        EditorGUILayout.EndScrollView();
	}
}