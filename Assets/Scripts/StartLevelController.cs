using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLevelController : OSCReciever
{
    public GameObject Haiku;
    private bool AnimationComplete = false;

    protected override void InitialiseLevel ()
    {
        base.InitialiseLevel ();
        ShouldMapSoundtoBloom = false;
    }

    public override void MapFeaturesToVisualisers ()
    {
        base.MapFeaturesToVisualisers ();
        if (Haiku != null)
        {
            Animator animator = Haiku.GetComponent<Animator>();
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
                AnimationComplete = true;
            if (AnimationComplete)
                ShouldMapSoundtoBloom = true;
        }
    }

    public override void AudioRMSGestureBegan ()
    {
        base.AudioRMSGestureBegan ();
        if (AnimationComplete) { Debug.Log ("StartGame"); }
            
    }
}
