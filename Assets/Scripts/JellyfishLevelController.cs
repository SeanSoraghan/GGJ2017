using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyfishLevelController : OSCReciever
{
    public GameObject JellyfishObject;
    public float VolumeThreshold = 0.1f;

    private int JellyfishIndex = 0;
    private List<GameObject> Jellyfish = new List<GameObject>();

    // Use this for initialization
    protected override void InitialiseLevel ()
    {
        AudioSegmenter.VolumeThreshold = VolumeThreshold;
        if (JellyfishObject != null)
            foreach (Transform jellyfish in JellyfishObject.transform)
                Jellyfish.Add (jellyfish.gameObject); 
    }

    public override void MapFeaturesToVisualisers ()
    {
        float rms = osc.Feature (AudioFeature.RMS);

        if (AudioGesturePlaying)
        {
            if (AudioSegmenter.CheckGestureEnd (rms, Time.deltaTime))
                AudioGestureEnded();
        }
        else
        { 
            if (AudioSegmenter.CheckGestureStart (rms, Time.deltaTime))
                AudioGestureBegan();
        }
    }

    void AudioGestureBegan()
    {
        AudioGesturePlaying = true;
    }

    void AudioGestureEnded()
    {
        AudioGesturePlaying = false;
        if (JellyfishIndex > -1 && JellyfishIndex < Jellyfish.Count)
        { 
            MoveJellyfish (JellyfishIndex);
        }
    }

    void MoveJellyfish (int jellyfishIndex)
    {
        JellyfishController controller = Jellyfish[jellyfishIndex].GetComponent<JellyfishController>();
        if (controller != null)
            controller.BeginMovementTowardsTarget();
    }
}
