using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class JellyfishController : MonoBehaviour
{
    public Shader LineShader;
    public float MovementSpeed                     = 1.0f;
    public float MovementTime                      = 1.0f;
    public float MovementFinishedThresholdDistance = 0.01f;
    public float ExpectedPitch                     = 0.5f;
    public float ExpectedGestureTime               = 0.5f;

    private Vector3 TargetPosition;
    private Vector3 CurrentMoveStartPosition;
    private float   CurrentMoveTime;
    private bool    ShouldMoveTowardsTarget = false;

	// Use this for initialization
	void Start ()
    {
		CurrentMoveStartPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
        //DrawLine (transform.position + new Vector3 (0, 0, -2), transform.position + new Vector3 (2.0f, 2.0f, -2.0f), Color.black);
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
        if (!ShouldMoveTowardsTarget)
        { 
            CurrentMoveStartPosition = transform.position;
            float MovementDiagonal   = MovementSpeed * MovementTime;
            float AxesMovement       = (Mathf.Sqrt (MovementDiagonal)) / 2.0f;
            TargetPosition           = new Vector3 (CurrentMoveStartPosition.x - AxesMovement, CurrentMoveStartPosition.y + AxesMovement, CurrentMoveStartPosition.z);
            CurrentMoveTime = 0.0f;
            ShouldMoveTowardsTarget  = true;
        }
    }

    void DrawLine (Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material (LineShader);
        lr.startColor = color;
        lr.endColor   = color;
        lr.startWidth = 1.0f;
        lr.endWidth   = 1.0f;
        lr.SetPosition (0, start);
        lr.SetPosition (1, end);
        GameObject.Destroy(myLine, duration);
    }

    void DrawSinCurve (Vector3 start, Vector3 end, Color color, float segmentLength = 0.01f, float duration = 0.2f)
    {
        float d = Vector3.Distance (end, start);
        float distanceX = end.x - start.x; 
        float distanceY = end.y - start.y;
        float xIncrement = distanceX / segmentLength;
        float yIncrement = (end.y - start.y) / segmentLength;
        float gradient = 1.0f;
        if (Mathf.Abs (distanceX) > 0.0f)
            gradient = distanceY / distanceX;
        for (int s = 0; s < segmentLength; s++)
        {

        }
    }
}
