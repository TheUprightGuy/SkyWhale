using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LureScript : MonoBehaviour
{
    #region Singleton
    public static LureScript instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("More than one Lure Exists!");
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    #endregion Singleton
    [Header("Current Position")]
    public float currentSide;
    public float currentForward;
    public float speed;
    [Header("Limits")]
    public float sideLimit = 5.0f;
    public float backLimit = -3.0f;
    public float forwardLimit = 3.0f;
    [Header("Speeds")]
    public float turnSpeed = 5.0f;
    public float forwardSpeed = 5.0f;
    [Header("Control Point")]
    public Transform controlPoint;
    float controlSway;
    [Header("Bird")]
    public Transform bird;
    public float lerpTimer;
    public float vertLerpTimer;

    public Vector3 initPos;
    private void Start()
    {
        initPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (EventHandler.instance.gameState.gamePaused) return;

        speed = (currentForward + 2.0f) / 2.0f;
        lerpTimer -= Time.deltaTime;
        vertLerpTimer -= Time.deltaTime;

        if (!Movement.instance.player.gameObject.activeSelf)
        {
            if (Input.GetKey(KeyCode.A))
            {
                if (currentSide - Time.deltaTime * turnSpeed > -sideLimit)
                {
                    currentSide -= Time.deltaTime * turnSpeed;
                    transform.position -= transform.right * Time.deltaTime * turnSpeed;
                    //controlPoint.position += transform.right * Time.deltaTime * turnSpeed * 0.5f;
                    bird.localEulerAngles -= Vector3.up * Time.deltaTime * 10.0f * turnSpeed;
                }
            }
            else if (Input.GetKey(KeyCode.D))
            {
                if (currentSide + Time.deltaTime * turnSpeed < sideLimit)
                {
                    currentSide += Time.deltaTime * turnSpeed;
                    transform.position += transform.right * Time.deltaTime * turnSpeed;
                    //controlPoint.position -= transform.right * Time.deltaTime * turnSpeed * 0.5f;
                    bird.localEulerAngles += Vector3.up * Time.deltaTime * 10.0f * turnSpeed;
                }
            }

            if (Input.GetKey(KeyCode.W))
            {
                if (currentForward + Time.deltaTime * forwardSpeed < forwardLimit)
                {
                    currentForward += Time.deltaTime * forwardSpeed;
                    transform.position += -transform.up * Time.deltaTime * forwardSpeed;
                    //controlPoint.position += -transform.up * Time.deltaTime * forwardSpeed;
                }
            }
            else if (Input.GetKey(KeyCode.S))
            {
                if (currentForward + Time.deltaTime * forwardSpeed > backLimit)
                {
                    currentForward -= Time.deltaTime * forwardSpeed;
                    transform.position -= -transform.up * Time.deltaTime * forwardSpeed;
                    //controlPoint.position -= -transform.up * Time.deltaTime * forwardSpeed;
                }
            }
        }

        controlSway = Mathf.PingPong(Time.time, 2.0f) - 1.0f;

        controlPoint.localPosition = Vector3.right * controlSway * 50.0f - Vector3.forward * 250.0f;

        transform.position += new Vector3(Mathf.PerlinNoise(Time.time, 0) * 2 - 1.0f, 0, 0) * Time.deltaTime;

        if (!(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)))
        {
            if (lerpTimer <= 0)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(initPos.x, transform.localPosition.y, initPos.z), Time.deltaTime / 2);

                bird.localRotation = Quaternion.Slerp(bird.localRotation, Quaternion.identity, Time.deltaTime / 4);
                currentSide = Mathf.Lerp(currentSide, 0, Time.deltaTime / 2);
            }
        }
        else
        {
            lerpTimer = 0.5f;
        }
        if (!(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.W)))
        {
            if (vertLerpTimer <= 0)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(transform.localPosition.x, initPos.y, transform.localPosition.z), Time.deltaTime / 2);

                bird.localRotation = Quaternion.Slerp(bird.localRotation, Quaternion.identity, Time.deltaTime / 4);
                currentForward = Mathf.Lerp(currentForward, 0, Time.deltaTime / 2);
            }
        }
        else
        {
            vertLerpTimer = 0.5f;
        }
    }
}
