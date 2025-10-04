using UnityEngine;

public class CameraFollowTarget : MonoBehaviour
{
    [Header("Object to follow")]
    public GameObject TargetYFollowGameObject;
    //public GameObject TargetXFollow;

    [Header("Follow speed")]
    [Range(0.0f, 50.0f)]
    public float SpeedConfig = 6f;

    [Header("Camera Offset")]
    [Range(-10.0f, 10.0f)]
    public float YOffsetConfig = 2f;
    //[Range(-10.0f, 10.0f)]
    //public float xOffset;

    [Space(15)]
    public UIManager UIManagerInstance;

    float _interpolationVar;
    Vector3 _positionVector3;

    //camera follow the player
    void LateUpdate()
    {
        if (UIManagerInstance.GameStateEnum == GameStateEnum.RUNNING)
        {
            _interpolationVar = SpeedConfig * Time.deltaTime;

            _positionVector3 = transform.position;

            if (TargetYFollowGameObject.transform.position.y + YOffsetConfig > transform.position.y)
                _positionVector3.y = Mathf.Lerp(transform.position.y, TargetYFollowGameObject.transform.position.y + YOffsetConfig, _interpolationVar);
            //position.x = Mathf.Lerp(transform.position.x, TargetXFollow.transform.position.x + xOffset, interpolation);

            transform.position = _positionVector3;
        }
    }

    //reset camera position
    public void ResetTheCameraPosition()
    {
        transform.position = new Vector3(0, 0, -10);
    }
}