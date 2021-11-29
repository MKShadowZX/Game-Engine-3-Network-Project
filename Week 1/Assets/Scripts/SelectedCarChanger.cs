using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCarChanger : MonoBehaviour
{
    CarIndex carIndex;
    public List<GameObject> cars;
    int index = 0;
    int previousIndex = 0;

    private void Start()
    {
        carIndex = FindObjectOfType<CarIndex>();
    }

    public void NextBTN()
    {
        previousIndex = index;
        index++;
        if (index > cars.Count - 1)
        {
            index = 0;
        }
        cars[index].SetActive(true);
        cars[previousIndex].SetActive(false);
        carIndex.index = index;
    }

    public void BackBTN()
    {
        previousIndex = index;
        index--;
        if (index < 0)
        {
            index = cars.Count - 1;
        }
        cars[index].SetActive(true);
        cars[previousIndex].SetActive(false);
        carIndex.index = index;
    }
}
