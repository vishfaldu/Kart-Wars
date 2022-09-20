using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIcontroller : MonoBehaviour
{
    public Circuit circuit;
    carDrive driveScript;
    public float steeringSensitivity = 0.01f;
    public float brakingSensitivity = 1.1f;
    public float accelSensitivity = 0.3f;

    Vector3 target;                // target waypoint
    Vector3 nextTarget;
    int currentWP = 0;          //current waypoint
    float totalDistanceToTarget;

    GameObject tracker;
    int currentTrackerWP = 0;
    public float lookAhead = 12;

    float lastTimeMoving = 0;

    CheckpointManager cpm;
    float finishSteer;

    void Start()
    {
        if (circuit == null)
            circuit = GameObject.FindGameObjectWithTag("circuit").GetComponent<Circuit>();

        driveScript = this.GetComponent<carDrive>();
        target = circuit.waypoints[currentWP].transform.position;
        nextTarget = circuit.waypoints[currentWP + 1].transform.position;
        totalDistanceToTarget = Vector3.Distance(target, driveScript.rb.gameObject.transform.position);

        tracker = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        DestroyImmediate(tracker.GetComponent<Collider>());
        tracker.GetComponent<MeshRenderer>().enabled = false;

        tracker.transform.position = driveScript.rb.gameObject.transform.position;
        tracker.transform.rotation = driveScript.rb.gameObject.transform.rotation;

        this.GetComponent<Ghost>().enabled = false;
        finishSteer = Random.Range(-1.0f, 1.0f);
    }

    void ProgressTracker()
    {
        Debug.DrawLine(driveScript.rb.gameObject.transform.position, tracker.transform.position);

        if (Vector3.Distance(driveScript.rb.gameObject.transform.position, tracker.transform.position) > lookAhead) return;

        tracker.transform.LookAt(circuit.waypoints[currentTrackerWP].transform.position);
        tracker.transform.Translate(0, 0, 1.0f);        //speed of tracker

        if(Vector3.Distance(tracker.transform.position, circuit.waypoints[currentTrackerWP].transform.position) < 1)
        {
            currentTrackerWP++;
            if (currentTrackerWP >= circuit.waypoints.Length)
                currentTrackerWP = 0;
        }
    }

    void ResetLayer()
    {
        driveScript.rb.gameObject.layer = 0;
        this.GetComponent<Ghost>().enabled = false;
    }




    //bool isJump = false;
    void Update()
    {
        if(!raceMonitor.racing)
        {
            lastTimeMoving = Time.time;
            return;
        }

        if (cpm == null)
            cpm = driveScript.rb.GetComponent<CheckpointManager>();

        if(cpm.lap == raceMonitor.totalLaps + 1)
        {
            driveScript.highAccel.Stop();
            driveScript.Go(0, finishSteer, 1);
            return;
        }

        ProgressTracker();
        Vector3 localTarget;

        if (driveScript.rb.velocity.magnitude > 1)
            lastTimeMoving = Time.time;

        if(Time.time > lastTimeMoving + 4 || driveScript.rb.gameObject.transform.position.y < -5)
        {
            

            driveScript.rb.transform.position = cpm.lastCP.transform.position + Vector3.up * 2;
            driveScript.rb.transform.rotation = cpm.lastCP.transform.rotation;

            /*driveScript.rb.transform.position = circuit.waypoints[currentTrackerWP].transform.position + Vector3.up * 2
                + new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1)); */
            tracker.transform.position = cpm.lastCP.transform.position;
            driveScript.rb.gameObject.layer = 8;
            this.GetComponent<Ghost>().enabled = true;
            Invoke("ResetLayer", 3);
        }




        if(Time.time < driveScript.rb.GetComponent<avoidDetector>().avoidTime)
        {
            localTarget = tracker.transform.right * driveScript.rb.GetComponent<avoidDetector>().avoidPath;
        }
        else
        {
            localTarget = driveScript.rb.gameObject.transform.InverseTransformPoint(tracker.transform.position);
        }
        float targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;

        float steer = Mathf.Clamp(targetAngle * steeringSensitivity, -1, 1) * Mathf.Sign(driveScript.currentSpeed);

        float speedFactor = driveScript.currentSpeed / driveScript.maxSpeed;
        float corner = Mathf.Clamp(Mathf.Abs(targetAngle), 0, 90);
        float cornerFactor = corner / 90.0f;
        
        float brake = 0;
        if (corner > 10 && speedFactor > 0.1f)
            brake = Mathf.Lerp(0, 1 + speedFactor * brakingSensitivity, cornerFactor);

        float acc = 1f;
        if (corner > 20 && speedFactor > 0.2f)
            acc = Mathf.Lerp(0, 1 * accelSensitivity, 1 - cornerFactor);

        float prevTorque= driveScript.torque;
        if(speedFactor < 0.3f && driveScript.rb.gameObject.transform.forward.y > 0.1f)
        {
            driveScript.torque *= 3.0f;
            acc = 1f;
            brake = 0;
        } 

        driveScript.Go(acc, steer, brake);
        driveScript.CheckForSkid();
        driveScript.CalcEngineSound();

        driveScript.torque = prevTorque;
        //Debug.Log("Brake :: " +brake+ "  Accel :: " +acc+ "  Speed :: " + driveScript.rb.velocity.magnitude);
        //Vector3 nextLocalTarget = driveScript.rb.gameObject.transform.InverseTransformPoint(nextTarget);
        //float distanceToTarget = Vector3.Distance(target, driveScript.rb.gameObject.transform.position);
        //float nextTargetAngle = Mathf.Atan2(nextLocalTarget.x, nextLocalTarget.z) * Mathf.Rad2Deg;
        //float distanceFactor = distanceToTarget / totalDistanceToTarget;
        //float speedFactor = driveScript.currentSpeed / driveScript.maxSpeed;
        //float acc = Mathf.Lerp(accelSensitivity, 1, distanceFactor);
        //float brake = Mathf.Lerp((-1 - Mathf.Abs(nextTargetAngle)) * brakingSensitivity, 1 + speedFactor, 1 - distanceFactor);

        /*
            if (Mathf.Abs(nextTargetAngle) > 20)
        {
            brake += 0.08f;
            acc -= 0.08f;
        }

        if(isJump)
        {
            acc = 1;
            brake = 0;
            Debug.Log("JUMP!");

        }
        */

        //if (distanceToTarget < 5) {brake = 0.8f; acc = 0.1f;}

        /*
        if(distanceToTarget < 4)            //threshold, make larger if car starts to circle waypoint
        {
            currentWP++;
            if(currentWP >= circuit.waypoints.Length)
                currentWP = 0;
            target = circuit.waypoints[currentWP].transform.position;
            
            if(currentWP == circuit.waypoints.Length - 1)
                nextTarget = circuit.waypoints[0].transform.position;
            else
                nextTarget = circuit.waypoints[currentWP + 1].transform.position;

            nextTarget = circuit.waypoints[currentWP + 1].transform.position;
            totalDistanceToTarget = Vector3.Distance(target, driveScript.rb.gameObject.transform.position);

            if (driveScript.rb.gameObject.transform.InverseTransformPoint(target).y > 5)
            {
                isJump = true;
            }
            else
                isJump = false;
        }            
        */

    }
}
