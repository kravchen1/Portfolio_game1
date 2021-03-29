using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class cube_no_Y_plus : MonoBehaviour
{
    void OnCollisionEnter()
    {
        GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
    }

}
