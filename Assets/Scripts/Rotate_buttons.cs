using UnityEngine;

public class Rotate_buttons : MonoBehaviour
{
    public float speed = 5f;

    private Transform _rotator;

    private void Start()
    {
        _rotator = GetComponent<Transform>();
    }

    private void Update()
    {

        _rotator.Rotate(0, 0, speed * PlayerPrefs.GetInt("speed_rotate") * Time.deltaTime);
    }

}
