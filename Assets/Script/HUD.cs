using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HUD : MonoBehaviour
{
    CanvasGroup canvasGroup;
    float HUDSetting = 0;

    void Start()
    {
        canvasGroup = this.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;

        if (PlayerPrefs.HasKey("HUD"))
            HUDSetting = PlayerPrefs.GetFloat("HUD");
    }

    void Update()
    {
        if (raceMonitor.racing)
            canvasGroup.alpha = HUDSetting;

        if (Input.GetKeyDown(KeyCode.H))
        {
            canvasGroup.alpha = canvasGroup.alpha == 1 ? 0 : 1;
            HUDSetting = canvasGroup.alpha;
            PlayerPrefs.SetFloat("HUD", canvasGroup.alpha);
        }
    }
}
