using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountainLevelController : OSCReciever
{
    public float TargetReachedDistanceThreshold = 0.05f;
    public MoveCloud Cloud;
    public MoveCloud2 Cloud2;

    public GameObject Boat;
    public GameObject BoatTarget;
    public GameObject Cloud1Target;
    public GameObject Cloud2Target;

    public override void MapFeaturesToVisualisers ()
    {
        base.MapFeaturesToVisualisers ();
        bool cloud1Passed = Cloud.gameObject.transform.position.x < Cloud1Target.transform.position.x;
        bool Cloud2Passed = Cloud2.gameObject.transform.position.x > Cloud2Target.transform.position.x;
        if (Boat.transform.position.x < BoatTarget.transform.position.x)
        {
            LevelCompleted();
        }
    }

    public override void AudioRMSGestureBegan ()
    {
        base.AudioRMSGestureBegan ();
        Cloud.CloudShouldMove = true;
        Cloud2.CloudShouldMove = true;
    }

    public override void AudioRMSGestureEnded ()
    {
        base.AudioRMSGestureEnded ();
        Cloud.CloudShouldMove = false;
        Cloud2.CloudShouldMove = false;
    }

    public override void AudioGestureBegan ()
    {
        base.AudioGestureBegan ();
        Cloud.CloudShouldMove = true;
        Cloud2.CloudShouldMove = true;
    }

    public override void AudioGestureEnded ()
    {
        base.AudioGestureEnded ();
        Cloud.CloudShouldMove = false;
        Cloud2.CloudShouldMove = false;
    }
}
