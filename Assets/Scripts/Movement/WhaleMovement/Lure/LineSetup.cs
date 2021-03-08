using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LineSetup : MonoBehaviour
{
    public Transform tracker;
    public Transform fallBackTracker;
    public Transform target;

    LineRenderer lr;
    CapsuleCollider cc;
    Vector3[] positions = new Vector3[2];
    public float TimeToTransition = 5.0f;

    public AnimationCurve LineWidthOverTime;
    //Vector3 currentPos = transform.position;
    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        cc = GetComponent<CapsuleCollider>();

        positions[0] = transform.position;
        positions[1] = transform.position;
        lr.SetPositions(positions);
    }

    private void OnEnable() {
        DrawLine();
    }

    public void SetLampTarget(Transform _target)
    {
        target = _target;
        DrawLine();
    }

    void DrawLine()
    {
        if (target)
        {
            positions[0] = transform.position;
            StartCoroutine(LerpToPos(target.position, TimeToTransition));
        }
    }

    IEnumerator LerpToPos(Vector3 targetPos, float timeToLerp)
    {
        float elapsedTime = 0.0f;
        while (elapsedTime < timeToLerp)
        {
            positions[0] = Vector3.Lerp(transform.position, targetPos, (elapsedTime / timeToLerp));
            //lr.startWidth = LineWidthOverTime.Evaluate(elapsedTime / timeToLerp);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
    // Update is called once per frame
    void Update()
    {
        transform.position = (tracker.gameObject.activeSelf) ? (tracker.position) : (fallBackTracker.position);
        if (Application.isPlaying) 
        {
            //positions[0] = target.position;
            positions[1] = transform.position;
            lr.SetPositions(positions);
            UpdateCollider();
        }
    }

    /*private void OnValidate()
    {
        lr = GetComponent<LineRenderer>();
        cc = GetComponent<CapsuleCollider>();

        positions[0] = target.position;
        positions[1] = transform.position;
        lr.SetPositions(positions);
        UpdateCollider();
    }*/

    void UpdateCollider()
    {
        Vector3 dir = positions[0] - positions[1];
        float dist = Vector3.Distance(positions[1], positions[0]);
        transform.up = dir;
        cc.center = new Vector3(0.0f, dist / 2, 0.0f);
        cc.height = dist;
    }
}
