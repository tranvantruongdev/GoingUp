using UnityEngine;

public class Side : MonoBehaviour
{
    Camera _camera;

    void Start()
    {
        _camera = Camera.main;
    }

    //side follow the camera on y axis
    void Update()
    {
        transform.position = new Vector2(transform.position.x, _camera.transform.position.y);
    }
}
