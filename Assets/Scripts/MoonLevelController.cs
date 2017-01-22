using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonLevelController : OSCReciever
{
    public MoveMoon MoonMaskController;
    public GameObject MoonMaskTarget;

    public override void MapFeaturesToVisualisers ()
    {
        base.MapFeaturesToVisualisers ();
        if (MoonMaskController != null && MoonMaskController.MoonShouldMove != AudioGesturePlaying)
            MoonMaskController.MoonShouldMove = AudioGesturePlaying;
        if (HasMoonMaskReachedTarget())
        {
            LevelCompleted();
        }
    }

    public bool HasMoonMaskReachedTarget()
    {
        if (MoonMaskController != null && MoonMaskTarget != null)
            return MoonMaskController.transform.position.x < MoonMaskTarget.transform.position.x
                && MoonMaskController.transform.position.y > MoonMaskTarget.transform.position.y;

        return false;
    }
}
