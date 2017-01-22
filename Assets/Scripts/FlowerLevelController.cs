using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerLevelController : OSCReciever
{
    public GameObject Bee;
    public GameObject BeeTarget;
    public FlowerController Flower;

    public override void MapFeaturesToVisualisers ()
    {
        base.MapFeaturesToVisualisers ();
        if (Flower != null && Flower.OpenFlower != AudioGesturePlaying)
            Flower.OpenFlower = AudioGesturePlaying;
        if (HasBeeReachedTarget() && !LevelComplete)
        { 
            LevelComplete = true;
            GlobalController.GetGlobalController().CurrentLevelCompleted();
        }
    }

    public bool HasBeeReachedTarget()
    {
        if (Bee != null && BeeTarget != null)
            return Bee.transform.position.x < BeeTarget.transform.position.x;
        return false;
    }
}
