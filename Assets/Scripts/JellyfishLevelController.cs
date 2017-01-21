using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyfishLevelController : OSCReciever
{
    public bool DebugPropertiesComparison = false;
    public GameObject JellyfishObject;
    public float PitchDifferenceThreshold = 0.1f;
    public float GestureTimeDifferenceThreshold = 0.1f;

    private List<GameObject> Jellyfish = new List<GameObject>();

    // Use this for initialization
    protected override void InitialiseLevel ()
    {
        
        if (JellyfishObject != null)
            foreach (Transform jellyfish in JellyfishObject.transform)
                Jellyfish.Add (jellyfish.gameObject); 
    }

    public override void MapFeaturesToVisualisers ()
    {
        if (AudioGesturePlaying)
        {
            if (AudioSegmenter.CheckGestureEnd (ref osc, Time.deltaTime))
                AudioGestureEnded();
        }
        else
        { 
            if (AudioSegmenter.CheckGestureStart (ref osc, Time.deltaTime))
                AudioGestureBegan();
        }
        if (HaveAllTargetsBeenReached())
            GlobalController.GetGlobalController().PlayWinSound();
        
    }

    public bool HaveAllTargetsBeenReached()
    {
        for (int j = 0; j < Jellyfish.Count; ++j)
        {
            GameObject jellyfish = Jellyfish[j];
            if (jellyfish == null)
                return false;
            JellyfishController controller = jellyfish.GetComponent<JellyfishController>();
            if (controller == null)
                return false;
            if (controller != null && !controller.HasPassedTarget())
                return false;
        }
        return true;
    }

    void AudioGestureBegan()
    {
        AudioGesturePlaying = true;
    }

    void AudioGestureEnded()
    {
        AudioGesturePlaying = false;
        for (int j = 0; j < Jellyfish.Count; ++j)
            if (DoesGestureMatchJellyfishExpectedProperties (j))
                MoveJellyfish (j);
    }

    void MoveJellyfish (int jellyfishIndex)
    {
        JellyfishController controller = Jellyfish[jellyfishIndex].GetComponent<JellyfishController>();
        if (controller != null)
            controller.BeginMovementTowardsTarget();
    }

    bool DoesGestureMatchJellyfishExpectedProperties (int jellyfishIndex)
    {
        JellyfishController controller = Jellyfish[jellyfishIndex].GetComponent<JellyfishController>();
        if (controller != null)
        { 
            float gesturePitch = AudioSegmenter.GetGestureFeature (AudioFeature.F0);
            float gestureTime  = AudioSegmenter.LastAudioGestureLength;
            float PitchDifference = Mathf.Abs (gesturePitch - controller.ExpectedPitch);
            float TimeDifference = Mathf.Abs (gestureTime - controller.ExpectedGestureTime);
            if (DebugPropertiesComparison)
            { 
                Debug.Log ("Pitch: " + gesturePitch + " | Difference: " + PitchDifference);
                Debug.Log ("Time: " + gestureTime + " | Difference: " + TimeDifference);
            }
            return PitchDifference < PitchDifferenceThreshold && TimeDifference < GestureTimeDifferenceThreshold;
        }
        return false;
    }

    
}
