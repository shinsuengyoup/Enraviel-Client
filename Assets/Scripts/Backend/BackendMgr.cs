using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BackEnd;

public class BackendMgr : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var bro = Backend.Initialize();

        if(bro.IsSuccess())
            Debug.Log("Initialize Succ");
        else
            Debug.Log("Initialize Fail");

            Debug.Log("test");
    }
}
