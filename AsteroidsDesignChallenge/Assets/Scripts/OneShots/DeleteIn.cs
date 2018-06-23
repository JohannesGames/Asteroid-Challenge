using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteIn : MonoBehaviour
{
    public float destroyIn;

    void Start()
    {
        Destroy(gameObject, destroyIn);
    }
}
