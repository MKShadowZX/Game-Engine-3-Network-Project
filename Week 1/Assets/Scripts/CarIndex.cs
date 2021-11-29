using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarIndex : MonoBehaviour
{
    public int index;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
