using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameUIController : MonoBehaviour
{
    public Text playerName;
    public Text lapDistance;
    public Transform target;
    CanvasGroup canvasGroup;
    public Renderer carRend;
    CheckpointManager cpManager;

    int CarRego;        //car registeration
    bool regoSet = false;
    void Start()
    {
        this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
        playerName = this.GetComponent<Text>();
        canvasGroup = this.GetComponent<CanvasGroup>();

    }
    
    void LateUpdate()
    {
        if (!raceMonitor.racing) { canvasGroup.alpha = 0; return; }
        if (!regoSet)
        {
            CarRego = Leaderboard.RegisterCar(playerName.text);
            regoSet = true;
            return;
        }
        if (carRend == null) return;
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        bool carInView = GeometryUtility.TestPlanesAABB(planes, carRend.bounds);
        canvasGroup.alpha = carInView ? 1 : 0;
        this.transform.position = Camera.main.WorldToScreenPoint(target.position + Vector3.up * 1.2f);
        if (cpManager == null)
            cpManager = target.GetComponent<CheckpointManager>();

        Leaderboard.SetPosition(CarRego, cpManager.lap, cpManager.checkPoint, cpManager.timeEntered);
        string position = Leaderboard.GetPosition(CarRego);

        lapDistance.text = position + " (" + cpManager.checkPoint + ")" + " " + cpManager.lap ;
    }
}
