using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate_small_gold_cube : MonoBehaviour
{
    private void Update()
    {
        transform.Rotate(25 * Time.deltaTime, 25 * Time.deltaTime, 25 * Time.deltaTime); 
    }
}
