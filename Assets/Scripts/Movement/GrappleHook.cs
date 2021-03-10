using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    float storedSpringVal;
    Rigidbody storedRB;

    public GameObject Hook;
    public Transform CamTarget;
    public Vector3 AimOffset;

    public float CamTransitionTime = 0.5f;
    Vector3 cachedTargetPos;
    Vector3 targetStartOffset;
    Vector3 calcGotoPos;
    // Start is called before the first frame update
    void Start()
    {
        targetStartOffset = ( CamTarget.position - transform.position);
        cachedTargetPos = CamTarget.position;
        calcGotoPos = transform.position + AimOffset;
        SpringJointActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ToggleAimPos(true);
        }

        if (Input.GetMouseButton(1) && LerpDone)
        {
            Debug.Log("Yep");
        }
        if (Input.GetMouseButtonUp(1))
        {
           ToggleAimPos(false);
        }
    }

    void SpringJointActive(bool _active)
    {
        if (_active)
        {
            Hook.SetActive(true);
        }
        else
        {
            Hook.SetActive(false);
        }
    }
    bool LerpDone = true;
    bool MovingToAim = true;

    //No one look in here
    //It works okay?
    IEnumerator MoveTargetToPos()
    {
        LerpDone = false;
        float elapsedTime = 0;
        float waitTime = CamTransitionTime;
        Vector3 startPos = CamTarget.position;
        while (elapsedTime < waitTime)
        {
            Vector3 initPos = transform.position
                + (transform.right * targetStartOffset.x)
                + (transform.forward * targetStartOffset.z)
                + (transform.up * targetStartOffset.y);

            Vector3 aimPos = transform.position 
                + (transform.right * AimOffset.x) 
                + (transform.forward * AimOffset.z) 
                + (transform.up * AimOffset.y);
            Vector3 endPos = (MovingToAim) ? (aimPos) : (initPos);

            CamTarget.position = Vector3.Lerp(startPos, endPos, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return null;
        LerpDone = true;
    }

    /// <summary>
    /// Lerps the camera target from cachedStartPos to AimOffset or visa versa
    /// </summary>
    /// <param name="_MovingToAim">if true goes to aimoffset, if false goes back to start</param>
    void ToggleAimPos(bool _MovingToAim)
    {
        MovingToAim = _MovingToAim;

        IEnumerator cor = MoveTargetToPos();
        StartCoroutine(cor);
    }

    private void OnDisable()
    {
        Hook.SetActive(false);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
    }
}
