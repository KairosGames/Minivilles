using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SunburstRotation : MonoBehaviour
{

    [SerializeField] float rotationSpeed;
    Vector3 rotationSpeedvec;

    void Start()
    {
        rotationSpeedvec = new Vector3(0, 0, rotationSpeed);
    }

    
    void Update()
    {
        transform.Rotate(-rotationSpeedvec);
    }
}
