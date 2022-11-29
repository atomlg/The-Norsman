﻿#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HumanoidHandPoseHelper))]
public class HumanoidHandPoseHelperEditor : Editor {

    HumanoidHandPoseHelper humanoidHandPoseHelper;

    private void OnEnable()
    {
        humanoidHandPoseHelper = (HumanoidHandPoseHelper)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Export Left HandPose as animation clip"))
        {
            ExportHandPose(true, false, "_LeftHandPose.anim");
        }

        if (GUILayout.Button("Export Right HandPose as animation clip"))
        {
            ExportHandPose(false, true, "_RightHandPose.anim");
        }

        if (GUILayout.Button("Export Both HandPose as animation clip"))
        {
            ExportHandPose(true, true, "_BothHandPose.anim");
        }

        if (GUILayout.Button("Reset HandPose"))
        {
            List<string> keyList = new List<string>();
            var keys = humanoidHandPoseHelper.leftHandPoseValueMap.Keys;
            foreach (var key in keys)
            {
                keyList.Add(key);
            }

            foreach (var key in keyList)
            {
                humanoidHandPoseHelper.leftHandPoseValueMap[key] = 0f;
            }
            keyList.Clear();

            keys = humanoidHandPoseHelper.rightHandPoseValueMap.Keys;
            foreach (var key in keys)
            {
                keyList.Add(key);
            }

            foreach (var key in keyList)
            {
                humanoidHandPoseHelper.rightHandPoseValueMap[key] = 0f;
            }
        }

        EditorGUI.BeginChangeCheck();
        Dictionary<string, float> updateValues = new Dictionary<string, float>();

        foreach (var pair in humanoidHandPoseHelper.leftHandPoseValueMap)
        {
            float value = EditorGUILayout.Slider(pair.Key,
                pair.Value, -1f, 1f);

            updateValues[pair.Key] = value;
        }

        foreach (var pair in updateValues)
        {
            humanoidHandPoseHelper.leftHandPoseValueMap[pair.Key] = pair.Value;
        }
        updateValues.Clear(); //

        foreach (var pair in humanoidHandPoseHelper.rightHandPoseValueMap)
        {
            float value = EditorGUILayout.Slider(pair.Key,
                pair.Value, -1f, 1f);

            updateValues[pair.Key] = value;
        }

        foreach (var pair in updateValues)
        {
            humanoidHandPoseHelper.rightHandPoseValueMap[pair.Key] = pair.Value;
        }

        if (EditorGUI.EndChangeCheck())
        {
            humanoidHandPoseHelper.EditorUpdate();
        }
    }

    public void ExportHandPose(bool left, bool right, string outputFileName)
    {
        if (string.IsNullOrEmpty(outputFileName))
        {
            Debug.LogError("No output file name for exported animation", this);
            return;
        }

        var clip = new AnimationClip { frameRate = 30 };
        AnimationUtility.SetAnimationClipSettings(clip, new AnimationClipSettings { loopTime = true });

        for (int i = 0; i < HumanTrait.MuscleCount; i++)
        {
            var muscle = HumanTrait.MuscleName[i];
            if (left && HumanoidHandPoseHelper.TraitLeftHandPropMap.ContainsKey(muscle))
            {
                var curve = new AnimationCurve();
                curve.AddKey(0f, humanoidHandPoseHelper.leftHandPoseValueMap[muscle]);
                curve.AddKey(1f, humanoidHandPoseHelper.leftHandPoseValueMap[muscle]);
                var clipAttribute = humanoidHandPoseHelper.leftHandExportNameMap[muscle];
                clip.SetCurve("", typeof(Animator), clipAttribute, curve);
            }
            else if (right && HumanoidHandPoseHelper.TraitRightHandPropMap.ContainsKey(muscle))
            {
                var curve = new AnimationCurve();
                curve.AddKey(0f, humanoidHandPoseHelper.rightHandPoseValueMap[muscle]);
                curve.AddKey(1f, humanoidHandPoseHelper.rightHandPoseValueMap[muscle]);
                var clipAttribute = humanoidHandPoseHelper.rightHandExportNameMap[muscle];
                clip.SetCurve("", typeof(Animator), clipAttribute, curve);
            }
        }

        string path;
        if (string.IsNullOrEmpty(humanoidHandPoseHelper.animationClipName))
        {
            path = "Assets/" + this.name + outputFileName;
        }
        else
        {
            path = "Assets/" + humanoidHandPoseHelper.animationClipName + ".anim";
        }
        var uniqueAssetPath = AssetDatabase.GenerateUniqueAssetPath(path);

        AssetDatabase.CreateAsset(clip, uniqueAssetPath);
        AssetDatabase.SaveAssets();
    }
}
#endif
