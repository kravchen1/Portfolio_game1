using UnityEngine;
using UnityEngine.EventSystems;
using System;
public class shop_cube : MonoBehaviour
{
    public Material lock_cube;
    public Material[] material_cubes;
    void OnMouseDown()
    {
        int count_unlock_cube = 0;
        for (int i = 0; i < material_cubes.Length; i++)
        {
            string number_cube = "cube " + i;
            if (PlayerPrefs.GetInt(number_cube) == 1)
            {
                count_unlock_cube++;
            }
        }

        int number = Convert.ToInt32(this.name.Substring(5));
        
        if (PlayerPrefs.GetInt(this.name) == 1 && count_unlock_cube > 1)
        {
            PlayerPrefs.SetInt(this.name, 2);
            GetComponent<MeshRenderer>().material = lock_cube;
        }
        else if (PlayerPrefs.GetInt(this.name) == 2)
        {
            PlayerPrefs.SetInt(this.name, 1);
            GetComponent<MeshRenderer>().material = material_cubes[number];
        }
        
    }
}
