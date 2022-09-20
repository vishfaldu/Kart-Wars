using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class selectCar : MonoBehaviour
{
    public GameObject[] cars;
    int currentCar = 0;

    void Start()
    {
        if (PlayerPrefs.HasKey("PlayerCar"))
            currentCar = PlayerPrefs.GetInt("PlayerCar");

        this.transform.LookAt(cars[currentCar].transform.position);    
    }

   
    void Update()
    {
        PlayerPrefs.SetInt("PlayerCar", currentCar);

       if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentCar++;
            if (currentCar > cars.Length - 1)
                currentCar = 0;
        }
        Quaternion lookDir = Quaternion.LookRotation(cars[currentCar].transform.position - this.transform.position);
        this.transform.rotation = Quaternion.Slerp(transform.rotation, lookDir, Time.deltaTime);
    }

    void ConnectSingle()
    {
        SceneManager.LoadScene("Track1");
    }
}
