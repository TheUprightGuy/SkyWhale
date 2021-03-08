using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassRotation : MonoBehaviour
{
    #region Singleton
    public static CompassRotation instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one Compass Exists!");
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            animator = GetComponent<Animator>();
        }
    }
    #endregion Singleton

    Camera cam;
    Animator animator;
    public Animator arrowAnimator;

    public Transform whale;
    public Transform goal;
    public GameObject objective;

    public Vector3 dirToGoal;
    public Vector3 pointDir;
    public float angle;


    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles =  new Vector3(0, 0, cam.transform.eulerAngles.y);

        dirToGoal = new Vector3(goal.position.x, 0, goal.position.z) - new Vector3(whale.position.x, 0, whale.position.z);
        Vector3 forwardVec = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z);
        angle = -Vector3.SignedAngle(forwardVec, dirToGoal, Vector3.up) - cam.transform.localRotation.eulerAngles.y;
        
        objective.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    public void Show()
    {
        ResetTriggers();

        animator.SetTrigger("Show");
        arrowAnimator.SetTrigger("Show");
    }
    public void Hide()
    {
        ResetTriggers();

        animator.SetTrigger("Hide");
        arrowAnimator.SetTrigger("Hide");
    }

    void ResetTriggers()
    {
        animator.ResetTrigger("Show");
        animator.ResetTrigger("Hide");
        arrowAnimator.ResetTrigger("Show");
        arrowAnimator.ResetTrigger("Hide");
    }
}
