using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralSin : MonoBehaviour
{
    public Shader LineShader;
    public Color LineColour = new Color (126.0f / 255.0f, 68.0f / 255.0f, 97.0f / 255.0f);
    public Transform LineStart;
    public Transform LineEnd;
    public float Freq = 0.2f;
    public float Amp = 0.35f;

    private float WaveStartOffset = 0.0f;
	// Use this for initialization
	void Start ()
    {
        WaveStartOffset = Random.Range (0.0f, 1.0f) * Mathf.PI * 2.0f;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (LineStart != null && LineEnd != null)
		    DrawSinCurve (LineStart.position, LineEnd.position, LineColour, Freq, Amp);
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
        lr.startWidth = 0.05f;
        lr.endWidth   = 0.05f;
        lr.SetPosition (0, start);
        lr.SetPosition (1, end);
        GameObject.Destroy(myLine, duration);
    }

    void DrawSinCurve (Vector3 start, Vector3 end, Color color, float frequency, float amp, float segmentLength = 0.05f, float duration = 0.2f)
    {
        float d = Vector3.Distance (end, start);
        float distanceX = end.x - start.x; 
        float distanceY = end.y - start.y;
        float xIncrement = distanceX * segmentLength; 
        float gradient = 1.0f;
        if (Mathf.Abs (distanceX) > 0.0f)
            gradient = distanceY / distanceX;
        float yIncrement = gradient * xIncrement;

        Vector3 directionVec = (end - start).normalized;
        Vector3 perpVec = new Vector3 (directionVec.x, -directionVec.y, directionVec.z);

        float numSegments = 1.0f / segmentLength;
        Vector3 currentPos = start;
        
        float t = WaveStartOffset; 

        float f = frequency * 4.0f;
        float a = amp * 0.1f;
        for (int s = 0; s < numSegments; s++)
        {
            Vector3 target = currentPos + new Vector3 (xIncrement, yIncrement, 0) + Mathf.Sin (t - (Time.time)) * perpVec * a;
            DrawLine (currentPos, target, color);
            currentPos = target;
            t += segmentLength * Mathf.PI * 2.0f * f;
            //color.a -= segmentLength;
        }
    }
}
