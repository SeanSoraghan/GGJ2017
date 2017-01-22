using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MothLevelController : OSCReciever
{
    public int NumRequiredSaves = 3;
    public MothController Moth;
    public float MothListenYPosition = 0.5f;

    private int NumSaves = 0;
    // Use this for initialization
    protected override void InitialiseLevel ()
    {
        
    }

    public override void MapFeaturesToVisualisers ()
    {
        base.MapFeaturesToVisualisers();
    }

    public override void AudioGestureBegan()
    {
        base.AudioGestureBegan();
        if (Moth != null && !Moth.IsRunning() && Moth.transform.position.y <= MothListenYPosition)
        { 
            Moth.RunAway();
            if (++NumSaves >= NumRequiredSaves)
                GlobalController.GetGlobalController().PlayWinSound();
        }
    }

    public override void AudioRMSGestureBegan()
    {
        base.AudioRMSGestureBegan();
        if (Moth != null && !Moth.IsRunning() && Moth.transform.position.y <= MothListenYPosition)
        { 
            Moth.RunAway();
            if (++NumSaves >= NumRequiredSaves)
                GlobalController.GetGlobalController().PlayWinSound();
        }
    }
}
