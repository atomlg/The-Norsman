using System.Collections.Generic;
using UnityEngine;

public class HumanoidHandPoseHelper : MonoBehaviour
{
    public string animationClipName = "";
    public Animator animator = null;

    //JSV: Note the value of these Dictionary pairs MUST correspond to the rig fingers naming convention.
    public static Dictionary<string, string> TraitLeftHandPropMap = new Dictionary<string, string>
    {
            {"Left Thumb Spread",       "LeftThumb.L.Spread"},
            {"Left Thumb 1 Stretched",  "LeftThumb1.L Stretched"},
            {"Left Thumb 2 Stretched",  "LeftThumb2.L Stretched"},
            {"Left Thumb 3 Stretched",  "LeftThumb3.L Stretched"},
            {"Left Index Spread",       "LeftIndex.L.Spread"},
            {"Left Index 1 Stretched",  "LeftIndex1.L Stretched"},
            {"Left Index 2 Stretched",  "LeftIndex2.L Stretched"},
            {"Left Index 3 Stretched",  "LeftIndex3.L Stretched"},
            {"Left Middle Spread",      "LeftMiddle.L.Spread"},
            {"Left Middle 1 Stretched", "LeftMiddle1.L Stretched"},
            {"Left Middle 2 Stretched", "LeftMiddle2.L Stretched"},
            {"Left Middle 3 Stretched", "LeftMiddle3.L Stretched"},
            {"Left Ring Spread",        "LeftRing.L.Spread"},
            {"Left Ring 1 Stretched",   "LeftRing1.L Stretched"},
            {"Left Ring 2 Stretched",   "LeftRing2.L Stretched"},
            {"Left Ring 3 Stretched",   "LeftRing3.L Stretched"},
            {"Left Little Spread",      "LeftPinky.L.Spread"},
            {"Left Little 1 Stretched", "LeftPinky1.L Stretched"},
            {"Left Little 2 Stretched", "LeftPinky2.L Stretched"},
            {"Left Little 3 Stretched", "LeftPinky3.L Stretched"}
    };

    public static Dictionary<string, string> TraitRightHandPropMap = new Dictionary<string, string>
    {
            {"Right Thumb Spread",       "RightThumb.R.Spread"},
            {"Right Thumb 1 Stretched",  "RightThumb1.R Stretched"},
            {"Right Thumb 2 Stretched",  "RightThumb2.R Stretched"},
            {"Right Thumb 3 Stretched",  "RightThumb3.R Stretched"},
            {"Right Index Spread",       "RightIndex.R.Spread"},
            {"Right Index 1 Stretched",  "RightIndex1.R Stretched"},
            {"Right Index 2 Stretched",  "RightIndex2.R Stretched"},
            {"Right Index 3 Stretched",  "RightIndex3.R Stretched"},
            {"Right Middle Spread",      "RightMiddle.R.Spread"},
            {"Right Middle 1 Stretched", "RightMiddle1.R Stretched"},
            {"Right Middle 2 Stretched", "RightMiddle2.R Stretched"},
            {"Right Middle 3 Stretched", "RightMiddle3.R Stretched"},
            {"Right Ring Spread",        "RightRing.R.Spread"},
            {"Right Ring 1 Stretched",   "RightRing1.R Stretched"},
            {"Right Ring 2 Stretched",   "RightRing2.R Stretched"},
            {"Right Ring 3 Stretched",   "RightRing3.R Stretched"},
            {"Right Little Spread",      "RightPinky.R.Spread"},
            {"Right Little 1 Stretched", "RightPinky1.R Stretched"},
            {"Right Little 2 Stretched", "RightPinky2.R Stretched"},
            {"Right Little 3 Stretched", "RightPinky3.R Stretched"}
    };

    public Dictionary<string, float> leftHandPoseValueMap = new Dictionary<string, float>
    {
            {"Left Thumb 1 Stretched", 0f},
            {"Left Thumb Spread", 0f},
            {"Left Thumb 2 Stretched", 0f},
            {"Left Thumb 3 Stretched", 0f},
            {"Left Index 1 Stretched", 0f},
            {"Left Index Spread", 0f},
            {"Left Index 2 Stretched", 0f},
            {"Left Index 3 Stretched", 0f},
            {"Left Middle 1 Stretched", 0f},
            {"Left Middle Spread", 0f},
            {"Left Middle 2 Stretched", 0f},
            {"Left Middle 3 Stretched", 0f},
            {"Left Ring 1 Stretched", 0f},
            {"Left Ring Spread", 0f},
            {"Left Ring 2 Stretched", 0f},
            {"Left Ring 3 Stretched", 0f},
            {"Left Little 1 Stretched",0f},
            {"Left Little Spread", 0f},
            {"Left Little 2 Stretched", 0f},
            {"Left Little 3 Stretched", 0f}
    };

    public Dictionary<string, float> rightHandPoseValueMap = new Dictionary<string, float>
    {
            {"Right Thumb Spread", 0f},
            {"Right Thumb 1 Stretched", 0f},
            {"Right Thumb 2 Stretched", 0f},
            {"Right Thumb 3 Stretched", 0f},
            {"Right Index Spread", 0f},
            {"Right Index 1 Stretched", 0f},
            {"Right Index 2 Stretched", 0f},
            {"Right Index 3 Stretched", 0f},
            {"Right Middle Spread", 0f},
            {"Right Middle 1 Stretched", 0f},
            {"Right Middle 2 Stretched", 0f},
            {"Right Middle 3 Stretched",0f},
            {"Right Ring Spread", 0f},
            {"Right Ring 1 Stretched", 0f},
            {"Right Ring 2 Stretched", 0f},
            {"Right Ring 3 Stretched", 0f},
            {"Right Little Spread", 0f},
            {"Right Little 1 Stretched", 0f},
            {"Right Little 2 Stretched", 0f},
            {"Right Little 3 Stretched", 0f},
    };

    public Dictionary<string, string> leftHandExportNameMap = new Dictionary<string, string>
    {
            {"Left Thumb Spread",       "LeftHand.Thumb.Spread"},
            {"Left Thumb 1 Stretched",  "LeftHand.Thumb.1 Stretched"},
            {"Left Thumb 2 Stretched",  "LeftHand.Thumb.2 Stretched"},
            {"Left Thumb 3 Stretched",  "LeftHand.Thumb.3 Stretched"},
            {"Left Index Spread",       "LeftHand.Index.Spread"},
            {"Left Index 1 Stretched",  "LeftHand.Index.1 Stretched"},
            {"Left Index 2 Stretched",  "LeftHand.Index.2 Stretched"},
            {"Left Index 3 Stretched",  "LeftHand.Index.3 Stretched"},
            {"Left Middle Spread",      "LeftHand.Middle.Spread"},
            {"Left Middle 1 Stretched", "LeftHand.Middle.1 Stretched"},
            {"Left Middle 2 Stretched", "LeftHand.Middle.2 Stretched"},
            {"Left Middle 3 Stretched", "LeftHand.Middle.3 Stretched"},
            {"Left Ring Spread",        "LeftHand.Ring.Spread"},
            {"Left Ring 1 Stretched",   "LeftHand.Ring.1 Stretched"},
            {"Left Ring 2 Stretched",   "LeftHand.Ring.2 Stretched"},
            {"Left Ring 3 Stretched",   "LeftHand.Ring.3 Stretched"},
            {"Left Little Spread",      "LeftHand.Little.Spread"},
            {"Left Little 1 Stretched", "LeftHand.Little.1 Stretched"},
            {"Left Little 2 Stretched", "LeftHand.Little.2 Stretched"},
            {"Left Little 3 Stretched", "LeftHand.Little.3 Stretched"},
    };

    public Dictionary<string, string> rightHandExportNameMap = new Dictionary<string, string>
    {
            {"Right Thumb Spread",       "RightHand.Thumb.Spread"},
            {"Right Thumb 1 Stretched",  "RightHand.Thumb.1 Stretched"},
            {"Right Thumb 2 Stretched",  "RightHand.Thumb.2 Stretched"},
            {"Right Thumb 3 Stretched",  "RightHand.Thumb.3 Stretched"},
            {"Right Index Spread",       "RightHand.Index.Spread"},
            {"Right Index 1 Stretched",  "RightHand.Index.1 Stretched"},
            {"Right Index 2 Stretched",  "RightHand.Index.2 Stretched"},
            {"Right Index 3 Stretched",  "RightHand.Index.3 Stretched"},
            {"Right Middle Spread",      "RightHand.Middle.Spread"},
            {"Right Middle 1 Stretched", "RightHand.Middle.1 Stretched"},
            {"Right Middle 2 Stretched", "RightHand.Middle.2 Stretched"},
            {"Right Middle 3 Stretched", "RightHand.Middle.3 Stretched"},
            {"Right Ring Spread",        "RightHand.Ring.Spread"},
            {"Right Ring 1 Stretched",   "RightHand.Ring.1 Stretched"},
            {"Right Ring 2 Stretched",   "RightHand.Ring.2 Stretched"},
            {"Right Ring 3 Stretched",   "RightHand.Ring.3 Stretched"},
            {"Right Little Spread",      "RightHand.Little.Spread"},
            {"Right Little 1 Stretched", "RightHand.Little.1 Stretched"},
            {"Right Little 2 Stretched", "RightHand.Little.2 Stretched"},
            {"Right Little 3 Stretched", "RightHand.Little.3 Stretched"},
    };


    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        
    }

    public void EditorUpdate () {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("Null Animator", this);
                return;
            }
        }

        HumanPose pose = new HumanPose();
        float maxRange = 0.00001f;
        if(Vector3.Distance(animator.transform.position, Vector3.zero) > maxRange || 
           Quaternion.Angle(animator.transform.rotation, Quaternion.identity) > maxRange )
        {
            Debug.LogError("Ensure that object is at position zero and quarternion identity prior to executing changes");
            return;
        }

        var handler = new HumanPoseHandler(animator.avatar, animator.transform);
        handler.GetHumanPose(ref pose);

        for (int i = 0; i < pose.muscles.Length; i++)
        {
            var muscle = HumanTrait.MuscleName[i];
            if (TraitLeftHandPropMap.ContainsKey(muscle)) 
            {
                pose.muscles[i] = leftHandPoseValueMap[muscle];
            }
            else if (TraitRightHandPropMap.ContainsKey(muscle)) 
            {
                pose.muscles[i] = rightHandPoseValueMap[muscle];
            }
        }

        handler.SetHumanPose(ref pose); 
	}
}
