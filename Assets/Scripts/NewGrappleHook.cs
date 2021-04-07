/*
  Bachelor of Software Engineering
  Media Design School
  Auckland
  New Zealand
  (c) 2021 Media Design School
  File Name   :   NewGrappleHook.cs
  Description :   Handles movement and rotation for the Hook Component, and checks for collision with appropriate layers. 
  Date        :   07/04/2021
  Author      :   Wayd Barton-Redgrave
  Mail        :   wayd.bartonregrave@mds.ac.nz
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Audio;

/// <summary>
/// Used for switching current layers to prevent collision upon shooting from whale.
/// </summary>
public enum CurrentLayer
{
    Hook,
    FromWhaleHook,
    Player,
    FromWhalePlayer
}

public class NewGrappleHook : MonoBehaviour
{
    [Header("Setup Fields")]
    public float retractSpeed = 24.0f;
    public float shootSpeed = 24.0f;
    public GameObject smokePrefab;

    [Header("Debug Fields")]
    new public bool enabled;
    public bool connected;
    public bool manualRetract;
    public bool retracting;
    public float flightTime;
    public GameObject connectedObj;

    [HideInInspector] public LayerMask grappleableLayers;

    #region Setup
    // Local References
    Rigidbody rb;
    MeshRenderer mr;
    LineRenderer lr;
    //[HideInInspector]
    public Transform pc;
    SphereCollider sc;
    Vector3 cachedPos;
    Vector3 forceDir;
    bool pause;
    public Transform hookModel;

    /// <summary>
    /// Description: Gets Component References.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mr = GetComponentInChildren<MeshRenderer>();
        lr = GetComponent<LineRenderer>();
        sc = GetComponent<SphereCollider>();
    }

    /// <summary>
    /// Description: Sets Callback Functions.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    private void Start()
    {
        CallbackHandler.instance.pause += Pause;
    }
    private void OnDestroy()
    {
        CallbackHandler.instance.pause -= Pause;
    }


    void Pause(bool _pause)
    {
        pause = _pause;
    }
    #endregion Setup

    /// <summary>
    /// Description: Fires in specified direction from shoot position.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="_player">Reference to the player character</param>
    /// <param name="_dir">Direction to shoot in</param>
    public void Fire(Transform _player, Vector3 _dir)
    {
        // Cache References
        pc = _player;
        transform.position = _player.position;

        // Create smoke effect
        GameObject temp = Instantiate(smokePrefab, pc);
        Destroy(temp, 2.0f);

        // Turn collider on
        sc.enabled = true;

        // Set rotation
        transform.LookAt(transform.position + _dir);
        forceDir = _dir * shootSpeed;
        enabled = true;
        flightTime = 0.0f;
        rb.velocity = Vector3.zero;
        // Slowdown Effect
        TimeSlowDown.instance.SpeedUp();
        // Player audio
        AudioManager.instance.PlaySound("fireGrapple");
    }

    /// <summary>
    /// Description: Checks for any shifts of position from the connected object.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    private void Update()
    {
        // If paused, do not continue
        if (pause)
            return;

        // Renderer enabled as long as grapple is in use
        mr.enabled = enabled || retracting || connected;

        // If connected, check if object is moving
        if (connectedObj)
        {
            Vector3 shift = connectedObj.transform.position - cachedPos;
            cachedPos = connectedObj.transform.position;
            transform.position += shift;
        }

        // Update the line renderer
        UpdateLR();
    }

    /// <summary>
    /// Description: Updates the line renderer.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    void UpdateLR()
    {
        lr.positionCount = mr.enabled ? 2 : 0;
        if (!mr.enabled)
            return;

        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, pc.position);
    }

    /// <summary>
    /// Description: Handles Movement and Retraction.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    private void FixedUpdate()
    {
        // If game is paused
        if (pause)
            return;

        // Check Grapple is in Use
        if (enabled)
        {
            // If in use, apply force and check flight time
            if (!retracting)
            {
                sc.enabled = true;
                flightTime += Time.fixedDeltaTime * TimeSlowDown.instance.timeScale;

                // Physics update occurs more rapidly in build
                if (!Application.isEditor)
                {
                    rb.AddForce((forceDir / flightTime) * 0.02f, ForceMode.Acceleration);
                }
                else
                {
                    rb.AddForce((forceDir / flightTime) * 0.2f, ForceMode.Acceleration);
                }

                // If reached max Flight Time - Retract
                if (flightTime > 2.0f)
                {
                    enabled = false;
                    retracting = true;
                    connectedObj = null;
                }
            }
        }
        // If retracting, turn off collider and head to player
        if (retracting)
        {
            rb.velocity = Vector3.zero;

            connectedObj = null;
            sc.enabled = false;
            flightTime = 0.0f;
            
            forceDir = Vector3.Normalize(pc.position - transform.position) * (manualRetract ? retractSpeed * 2 : retractSpeed * 2);
            rb.MovePosition(transform.position + (manualRetract ? forceDir * Time.fixedDeltaTime : forceDir * Time.fixedDeltaTime * TimeSlowDown.instance.timeScale));

            // Check for distance to player - reset once close
            if (Vector3.Distance(transform.position, pc.position) < Mathf.Max((forceDir.magnitude * Time.fixedDeltaTime), 0.3f))
            {
                ResetHook();
            }
        }

        if (!pc)
            return;

        // Check for distance to player - reset once close
        if (Vector3.Distance(transform.position, pc.position) < 0.3f && connected)
        {
            retracting = true;
            manualRetract = false;
            connectedObj = null;

            if (!pc || !pc.GetComponent<PlayerMovement>()) 
                return;

            // Fire player if connected to surface and player enters range
            YeetPlayer(pc.GetComponentInParent<PlayerMovement>());
        }
    }

    /// <summary>
    /// Description: Resets all hook variables.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    public void ResetHook()
    {
        enabled = false;
        retracting = false;
        connected = false;
        manualRetract = false;
        connectedObj = null;
    }

    /// <summary>
    /// Description: Launches the player, helps make grapple movement feel good and helps with just hitting edges.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="_player">Reference to player</param>
    public void YeetPlayer(PlayerMovement _player)
    {
        // If grapple disabled, do not launch.
        if (enabled)
            return;

        // Otherwise add force.
        Rigidbody temp = _player.GetComponent<Rigidbody>();
        temp.AddForce(temp.velocity.magnitude * Vector3.Normalize((Vector3.Normalize(forceDir) + transform.up * 2.0f)) * 3.0f, ForceMode.Impulse);
        // Play grapple disconnect audio
        AudioManager.instance.PlaySound("GrappleFail");
    }

    /// <summary>
    /// Description: Checks for nearby grappleables on collision.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="collision">Colliding Object</param>
    private void OnCollisionEnter(Collision collision)
    {
        #region Switch Physics Layer
        // If hitting whale, change layer to ignore whale collision
        if (collision.gameObject.GetComponent<WhaleMovement>())
        {
            this.gameObject.layer = LayerMask.NameToLayer("HookFromWhale");
        }
        // If not hitting whale, change layer back and enable the player
        else if (this.gameObject.layer == LayerMask.NameToLayer("HookFromWhale"))
        {
            this.gameObject.layer = LayerMask.NameToLayer("Hook");
            // Start Moving Player
            EntityManager.instance.TogglePlayer(true);
        }
        #endregion Switch Physics Layer

        // Check for nearby grappleables.
        Collider[] grappleables = Physics.OverlapSphere(transform.position, sc.radius * 2.0f, grappleableLayers);
        enabled = false;
        // Play Hit Audio
        AudioManager.instance.PlaySound("GrappleHit");

        // Grappleable surface is found
        if (grappleables.Length != 0)
        {
            connected = true;
            connectedObj = collision.gameObject;
            cachedPos = connectedObj.transform.position;
        }
        // Grappleable surface not found
        else
        {
            // Play fail audio
            AudioManager.instance.PlaySound("GrappleFail");
            connectedObj = null;
            retracting = true;
        }

        // Smoke Effect
        GameObject temp = Instantiate(smokePrefab, transform.position, Quaternion.identity);
        Destroy(temp, 2.0f);

        // Stop Grapple
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        sc.enabled = false;
    }

    #region Utility
    /// <summary>
    /// Description: Checks if hook is in use.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <returns></returns>
    public bool InUse()
    {
        return (enabled || retracting || connected || manualRetract);
    }

    /// <summary>
    /// Description: Checks if layer is grappleable.
    /// <br>Author: Wayd Barton-Redgrave</br>
    /// <br>Last Updated: 04/07/2021</br>
    /// </summary>
    /// <param name="layer">Layer to check if grappleable</param>
    /// <returns>Returns whether layer is grappleable</returns>
    public bool GrappleAbleCheck(int layer)
    {
        return grappleableLayers == (grappleableLayers | (1 << layer));
    }
    #endregion Utility
}
