

//This was also worked on by Jacob Gallagher for glider audio functionality
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Audio;
using UnityEngine.Audio;

public class GliderMovement : MonoBehaviour
{
    [Header("Setup Fields")]
    public Transform playerRot;
    public float maxSpeed = 5.0f;
    public float recenterFactor = 5.0f;
    public GameObject glider;
    new public bool enabled;

    public Cinemachine.CinemachineVirtualCamera mainCam;
    public Cinemachine.CinemachineVirtualCamera glideCam;

    private float defaultVolume;
    private float defaultPitch;
    private AudioMixer _audioMixer;

    #region Local Variables
    public float currentSpeed = 0.0f;
    private Rigidbody rb;
    float rotationSpeed = 1.0f;
    Vector3 desiredVec;
    Vector3 desiredRoll;
    public float myRoll = 0.0f;
    public float myTurn = 0.0f;
    public float myPitch = 0.0f;
    float turnSpeed = 40;
    float liftSpeed = 20;
    float rollSpeed = 20;

    float parabolLerp;
    public float moveSpeed = 1;
    public float baseSpeed = 5.0f;
    float gravScale = 1.0f;
    #endregion Local Variables

    private void Awake()
    {
        rb = GetComponentInChildren<Rigidbody>();

        defaultVolume = gameObject.GetComponentInChildren<AudioSource>().volume;
        defaultPitch = gameObject.GetComponentInChildren<AudioSource>().pitch;
        _audioMixer = gameObject.GetComponentInChildren<AudioSource>().outputAudioMixerGroup.audioMixer;
    }

    // Start is called before the first frame update
    void Start()
    {
        desiredVec = transform.transform.eulerAngles;
        VirtualInputs.GetInputListener(InputType.WHALE, "YawLeft").MethodToCall.AddListener(YawLeft);
        VirtualInputs.GetInputListener(InputType.WHALE, "YawRight").MethodToCall.AddListener(YawRight);
        VirtualInputs.GetInputListener(InputType.WHALE, "PitchDown").MethodToCall.AddListener(PitchDown);
        VirtualInputs.GetInputListener(InputType.WHALE, "PitchUp").MethodToCall.AddListener(PitchUp);

        baseSpeed = maxSpeed * 0.4f;
        moveSpeed = baseSpeed;
        currentSpeed = 0.0f;
        EventManager.StartListening("EnableGlider", EnableGlider);
        CallbackHandler.instance.pause += Pause;
    }
    private void OnDestroy()
    {
        CallbackHandler.instance.pause -= Pause;
    }

    bool pause;
    void Pause(bool _pause)
    {
        pause = _pause;
    }

    void EnableGlider()
    {
        unlocked = true;
        EventManager.TriggerEvent("GliderTutorial");
    }

    private void PlayAudioOnToggle()
    {
        if (enabled)
        {
            AudioManager.instance.PlaySound("GliderWind");
            return;
        }
        AudioManager.instance.StopSound("GliderWind");
        AudioManager.instance.PlaySound("GliderClose");
    }

    public bool unlocked;
    public float delayInput;

    public void Toggle()
    {
        if (!unlocked)
            return;

        if (delayInput > 0.0f)
            return;

        delayInput = 1.0f;
        enabled = !enabled;
        glider.SetActive(enabled);
        myRoll = 0.0f;
        myPitch = 0.0f;
        myTurn = 0.0f;
        currentSpeed = baseSpeed;
        moveSpeed = baseSpeed;

        CameraManager.instance.SwitchCamera((enabled) ? CameraType.GlideCamera : CameraType.PlayerCamera);

        mainCam.GetComponent<ThirdPersonCamera>().SetRotation(glideCam.transform);
        rb.angularVelocity = Vector3.zero;

        //transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        PlayAudioOnToggle();
    }


    bool yawChange = false;
    void YawRight(InputState type)
    {
        if (myTurn + Time.deltaTime * turnSpeed < 40)// * TimeSlowDown.instance.timeScale < 40)
        {
            if (myTurn + Time.deltaTime * turnSpeed < 0)// * TimeSlowDown.instance.timeScale < 0)
            {
                myTurn += Time.deltaTime * turnSpeed;// * TimeSlowDown.instance.timeScale;
            }
            myTurn += Time.deltaTime * turnSpeed; // * TimeSlowDown.instance.timeScale;
        }

        if (myRoll - Time.deltaTime * rollSpeed > - 20)// * TimeSlowDown.instance.timeScale > -20)
        {
            myRoll -= Time.deltaTime * rollSpeed;// * TimeSlowDown.instance.timeScale;
        }
        yawChange = true;
        parabolLerp = 0.0f;
    }
    void YawLeft(InputState type)
    {
        if (myTurn - Time.deltaTime * turnSpeed > -40)// * TimeSlowDown.instance.timeScale > -40)
        {
            if (myTurn - Time.deltaTime * turnSpeed > 0)// * TimeSlowDown.instance.timeScale > 0)
            {
                myTurn -= Time.deltaTime * turnSpeed;// * TimeSlowDown.instance.timeScale;
            }
            myTurn -= Time.deltaTime * turnSpeed;// * TimeSlowDown.instance.timeScale;
        }

        if (myRoll + Time.deltaTime * rollSpeed < 20)// * TimeSlowDown.instance.timeScale < 20)
        {
            myRoll += Time.deltaTime * rollSpeed;// * TimeSlowDown.instance.timeScale;
        }
        yawChange = true;
        parabolLerp = 0.0f;
    }

    bool pitchChange = false;
    void PitchDown(InputState type)
    {
        if (myPitch + Time.deltaTime * liftSpeed < 45)// * TimeSlowDown.instance.timeScale < 45)
        {
            myPitch += Time.deltaTime * liftSpeed;// * TimeSlowDown.instance.timeScale;
        }
        pitchChange = true;
        parabolLerp = 0.0f;
    }
    void PitchUp(InputState type)
    {
        if (myPitch - Time.deltaTime * liftSpeed > -45)// * TimeSlowDown.instance.timeScale > -45)
        {
            myPitch -= Time.deltaTime * liftSpeed;// * TimeSlowDown.instance.timeScale;
        }
        pitchChange = true;
        parabolLerp = 0.0f;
    }


    public void MovementCorrections()
    {
        parabolLerp += Time.deltaTime * Time.deltaTime * recenterFactor;// * TimeSlowDown.instance.timeScale;
        parabolLerp = Mathf.Clamp01(parabolLerp);

        if (!yawChange)
        {
            myTurn = Mathf.Lerp(myTurn, 0, Time.deltaTime * turnSpeed * parabolLerp);// * TimeSlowDown.instance.timeScale);
            myRoll = Mathf.Lerp(myRoll, 0, Time.deltaTime * rollSpeed * 5 * parabolLerp);// * TimeSlowDown.instance.timeScale);            
        }
        if (!pitchChange)
        {
            myPitch = Mathf.Lerp(myPitch, 0.0f, Time.deltaTime * parabolLerp);// * TimeSlowDown.instance.timeScale);
            moveSpeed = Mathf.Lerp(moveSpeed, baseSpeed, Time.deltaTime * 0.5f);// * TimeSlowDown.instance.timeScale);                     
        }

        yawChange = false;
        pitchChange = false;
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            Toggle();
        }*/

        delayInput -= Time.fixedDeltaTime;// * TimeSlowDown.instance.timeScale;

        if (!enabled || pause) 
            return;

        if (base.transform.forward.y < 0)
        {
            moveSpeed += (base.transform.forward.y * -(maxSpeed / 1.5f)) * Time.fixedDeltaTime;// * TimeSlowDown.instance.timeScale;
        }
        else
        {
            moveSpeed += (base.transform.forward.y * -(maxSpeed / 3.0f)) * Time.fixedDeltaTime;// * TimeSlowDown.instance.timeScale;
        }

        moveSpeed = Mathf.Clamp(moveSpeed, 1.0f, maxSpeed);

        currentSpeed = Mathf.Lerp(currentSpeed, moveSpeed, Time.deltaTime);// * TimeSlowDown.instance.timeScale);
        MovementCorrections();

        //RotatePlayer();

        // Rot
        desiredVec = new Vector3(myPitch, base.transform.eulerAngles.y + myTurn, myRoll);

        base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(desiredVec), Time.deltaTime * rotationSpeed);// * TimeSlowDown.instance.timeScale);

        //Play sound
        //Code section Author: Jacob Gallagher
        float currentVolume = defaultVolume * (1 + Mathf.Clamp01((currentSpeed / maxSpeed)));
        float currentPitch = defaultPitch * (1 + Mathf.Clamp01((currentSpeed / maxSpeed)));
        _audioMixer.SetFloat("GliderWindVolume", currentVolume);
        _audioMixer.SetFloat("GliderWindPitch", currentPitch);
    }

    /// <summary>
    /// Description: Rotates player based on current glider rotation - removed for now due to climbing check issues.
    /// Author: Wayd Barton-Redgrave
    /// Last Updated: 06/04/2021
    /// </summary>
    public void RotatePlayer()
    {
        float absAngle = Mathf.Abs(base.transform.forward.y) * 90.0f;

        playerRot.localRotation = Quaternion.Euler(Vector3.right * absAngle);
    }

    private void FixedUpdate()
    {
        if (!enabled || pause)
            return;

       /* // Rot
        desiredVec = new Vector3(myPitch, base.transform.eulerAngles.y + myTurn, myRoll);

        base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(desiredVec), Time.deltaTime * rotationSpeed);*/
        
        gravScale = Mathf.Clamp01(0.7f - currentSpeed / maxSpeed) * 40.0f;
        Vector3 movementGrav = base.transform.forward * currentSpeed + gravScale * Physics.gravity * Time.deltaTime;// * TimeSlowDown.instance.timeScale;

        rb.velocity = movementGrav; 
    }
}
