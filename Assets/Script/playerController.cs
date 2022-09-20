using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class playerController : MonoBehaviour
{
    carDrive driveScript;
    float lastTimeMoving = 0;
    Vector3 lastPosition;
    Quaternion lastRotation;

    CheckpointManager cpm;
    float finishSteer;
    void ResetLayer()
    {
        driveScript.rb.gameObject.layer = 0;
        this.GetComponent<Ghost>().enabled = false;
    }

    void Start()
    {
        driveScript = this.GetComponent <carDrive>();
        this.GetComponent<Ghost>().enabled = false;
        lastPosition = driveScript.rb.gameObject.transform.position;
        lastRotation = driveScript.rb.gameObject.transform.rotation;
        finishSteer = Random.Range(-1.0f, 1.0f);
    }


    void Update()
    {
        if (cpm == null)
            cpm = driveScript.rb.GetComponent<CheckpointManager>();

        if (cpm.lap == raceMonitor.totalLaps + 1)
        {
            driveScript.highAccel.Stop();
            driveScript.Go(0, finishSteer, 1);
            return;
        }

        float a = Input.GetAxis("Vertical");        //acceleration
        float s = Input.GetAxis("Horizontal");        //Steering
        float b = Input.GetAxis("Jump");        //Braking

        if (driveScript.rb.velocity.magnitude > 1 || !raceMonitor.racing)
            lastTimeMoving = Time.time;

        RaycastHit hit;

        if (Physics.Raycast(driveScript.rb.gameObject.transform.position, -Vector3.up, out hit, 10))
        {
            if (hit.collider.gameObject.tag == "road")
            {
                lastPosition = driveScript.rb.gameObject.transform.position;
                lastRotation = driveScript.rb.gameObject.transform.rotation;
            }
        }
        
        if(Time.time > lastTimeMoving + 4 || driveScript.rb.gameObject.transform.position.y < -5)
        {
           

            driveScript.rb.gameObject.transform.position = cpm.lastCP.transform.position + Vector3.up * 2;
            driveScript.rb.gameObject.transform.rotation = cpm.lastCP.transform.rotation;
            driveScript.rb.gameObject.layer = 8;
            this.GetComponent<Ghost>().enabled = true;
            Invoke("ResetLayer", 3);
        }
        


        if (!raceMonitor.racing) a = 0;

        driveScript.Go(a,s,b);
        driveScript.CheckForSkid();
        driveScript.CalcEngineSound();
        
    }
}
