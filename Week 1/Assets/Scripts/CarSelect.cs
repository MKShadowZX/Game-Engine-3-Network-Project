using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarSelect : MonoBehaviour
{
    public CarSO car;

    public Text carNameText;
    public Text carSpeedText;

    private void Awake()
    {
        carNameText.text = car.carName;
        carSpeedText.text = "Speed: " + car.speed.ToString();
    }
}
