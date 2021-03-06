﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class JellyfishController : MonoBehaviour
{
    
    public float MovementSpeed                     = 1.0f;
    public float MovementTime                      = 1.0f;
    public float MovementFinishedThresholdDistance = 0.01f;
    public float ExpectedPitch                     = 0.5f;
    public float ExpectedGestureTime               = 0.5f;
    public Transform Target;

    private Vector3 TargetPosition;
    private Vector3 CurrentMoveStartPosition;
    private float   CurrentMoveTime;
    private bool    ShouldMoveTowardsTarget = false;

	// Use this for initialization
	void Start ()
    {
		CurrentMoveStartPosition = transform.position;
        AudioSource[] audioSources = GetComponents<AudioSource>();
        foreach (AudioSource source in audioSources)
            source.volume = 0.15f;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (ShouldMoveTowardsTarget)
            MoveTowardsTarget();
	}

    void MoveTowardsTarget()
    {
        CurrentMoveTime    += Time.deltaTime;
        float t            =  CurrentMoveTime / MovementTime;
        float tSkew        =  Mathf.Log10 (9.0f * t + 1.0f); 
        transform.position =  Vector3.Lerp (CurrentMoveStartPosition, TargetPosition, tSkew);
        CheckMovementEnd (t);
    }

    void CheckMovementEnd (float movementProportion)
    {
        if (Vector3.Distance (transform.position, TargetPosition) < MovementFinishedThresholdDistance || movementProportion >= 0.99f)
        { 
            transform.position = TargetPosition;
            ShouldMoveTowardsTarget = false;
        }
    }

    public void BeginMovementTowardsTarget()
    {
        if (!ShouldMoveTowardsTarget && !HasPassedTarget())
        { 
            TriggerWaterSound();
            CurrentMoveStartPosition = transform.position;
            float MovementDiagonal   = MovementSpeed * MovementTime;
            float AxesMovement       = (Mathf.Sqrt (MovementDiagonal)) / 2.0f;
            TargetPosition           = new Vector3 (CurrentMoveStartPosition.x - AxesMovement, CurrentMoveStartPosition.y + AxesMovement, CurrentMoveStartPosition.z);
            CurrentMoveTime = 0.0f;
            ShouldMoveTowardsTarget  = true;
        }
        
    }

    public void TriggerWaterSound()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length > 0)
        {
            int audioSourceIndex = (int) Mathf.Floor (Random.Range (0.0f, audioSources.Length - 0.01f));
            //while (audioSourceIndex < 0 || audioSourceIndex > audioSources.Length - 1)
            //    audioSourceIndex = (int) Mathf.Floor (Random.Range (0.0f, audioSources.Length - 0.01f));
            //while (audioSources[audioSourceIndex].isPlaying)
            //    audioSourceIndex = (int) Mathf.Floor (Random.Range (0.0f, audioSources.Length - 0.01f));
            audioSources[audioSourceIndex].Play();
        }
    }

    public bool HasPassedTarget()
    {
        if (Target != null)
            return transform.position.x < Target.position.x && transform.position.y > Target.position.y;
        return false;
    }
}
