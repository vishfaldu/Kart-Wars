using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayLeaderboard : MonoBehaviour
{
    public Text first;
    public Text second;
    public Text third;
    public Text fourth;

    public void Start()
    {
        Leaderboard.Reset();
    }
    void LateUpdate()
    {
        List<string> places = Leaderboard.GetPlaces();
        if(places.Count > 0)
            first.text = places[0];
        if (places.Count > 0)
            second.text = places[1];
        if (places.Count > 0)
            third.text = places[2];
        if (places.Count > 0)
            fourth.text = places[3];
    }
}
