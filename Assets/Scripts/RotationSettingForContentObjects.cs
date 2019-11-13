using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationSettingForContentObjects : MonoBehaviour
{
    void Update()
    {
        //Symbols always look towards the User
        Vector3 relativePos = Camera.main.transform.position - transform.position;
        relativePos = relativePos * -1;
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        rotation.x = 0;
        rotation.z = 0;
        transform.rotation = rotation;
    }
}
