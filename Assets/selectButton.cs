using UnityEngine;
using UnityEngine.SceneManagement;

public class selectButton : MonoBehaviour
{
    public GameObject[] cars;
    int currentCar = 0;

    void Start()
    {
        if (PlayerPrefs.HasKey("PlayerCar"))
            currentCar = PlayerPrefs.GetInt("PlayerCar");
    }

    void Update()
    {
        PlayerPrefs.SetInt("PlayerCar", currentCar);
    }

    public void Scene1()
    {
        SceneManager.LoadScene("Track1");
    }    
}
