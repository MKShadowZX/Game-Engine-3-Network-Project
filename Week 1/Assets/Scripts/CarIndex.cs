using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarIndex : MonoBehaviour
{
    public int index;

    //Lazy Singleton
    #region LAZY_SINGLETON_AND_AWAKE
    public static CarIndex instance = null;
    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    #endregion

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
