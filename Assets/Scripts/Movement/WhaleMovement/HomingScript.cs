using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingScript : MonoBehaviour
{
    public Transform player;
    public float dist;
    public float pickupHeight = 10.0f;
    Vector3 homingPos;
    Vector3 homingDir;

    Vector3 exitPos;
    Vector3 exitDir;

    #region Callbacks
    private void Start()
    {
        WhaleHandler.instance.startHoming += StartHoming;
        WhaleHandler.instance.startExit += StartExit;
        WhaleHandler.instance.moveWhale += MoveWhale;
    }
    private void OnDestroy()
    {
        WhaleHandler.instance.startHoming -= StartHoming;
        WhaleHandler.instance.startExit -= StartExit;
        WhaleHandler.instance.moveWhale -= MoveWhale;
    }
    #endregion Callbacks

    public void MoveWhale()
    {
        transform.position = homingPos;
    }

    public void StartHoming(Transform _player)
    {
        player = _player;
        Movement.instance.homing = true;
        Movement.instance.orbiting = false;
    }

    public void StartExit()
    {
        exitPos = Movement.instance.orbit.leashObject.transform.position;
    }

    private void Update()
    {
        if (Movement.instance.homing)
        {
            homingPos = player.position + Vector3.up * pickupHeight;

            dist = Vector3.Distance(transform.position, homingPos);
            if (dist < 5.0f)
            {
                Fader.instance.FadeOut();
                Movement.instance.homing = false;
            }

            homingDir = Vector3.Normalize(homingPos - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(homingDir), Time.deltaTime * 10.0f);
        }

        if (Movement.instance.exit)
        {
            dist = Vector3.Distance(transform.position, exitPos);
            if (dist >= 20.0f)
            {
                Movement.instance.exit = false;
            }

            exitDir = Vector3.Normalize(Vector3.Normalize(transform.position - exitPos) + transform.forward);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(exitDir), Time.deltaTime * 5.0f);
        }
    }
}
