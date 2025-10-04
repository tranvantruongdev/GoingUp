using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public Rigidbody2D ObstacleRigidbody2DReference;

    float _obstacleSpeedConfig;
    bool _isIcreatedNew = false;
    bool _isCreatedNewLine = false;
    float _screenWidthConfig, _screenHeightConfig, _minGapConfig, _maxGapConfig, _verticalGapConfig;
    float _heightConfig, _widthConfig;
    GameObject _nextObstacleGameObject;

    //player has landed on plaform -> enable another jump
    void OnCollisionEnter2D(Collision2D collision)
    {
        GameManager.S_Instance.IsInAir = false;
    }

    public void InitTheObstacle(Vector2 position, float obstacleSpeed, Color color, float screenWidth, float screenHeight, float minHorizontalGap, float maxHorizontalGap, float verticalGap, float height, float width, bool newLine)
    {
        transform.position = position;
        _obstacleSpeedConfig = obstacleSpeed;
        GetComponent<SpriteRenderer>().color = color;
        _screenWidthConfig = screenWidth;
        _screenHeightConfig = screenHeight;
        _minGapConfig = minHorizontalGap;
        _maxGapConfig = maxHorizontalGap;
        _verticalGapConfig = verticalGap;
        _heightConfig = height;
        _widthConfig = width;
        _isCreatedNewLine = newLine;
        transform.localScale = new Vector2(_widthConfig, _heightConfig);
    }

    void Update()
    {
        ObstacleRigidbody2DReference.linearVelocity = new Vector2(_obstacleSpeedConfig, 0);

        if (!_isCreatedNewLine && transform.position.y < GameManager.S_Instance.CameraGameObject.transform.position.y + _screenHeightConfig)
        {
            CreateNewLine();
        }

        if (_obstacleSpeedConfig < 0)
        {
            //if not created next obstacle create new when conditions go through
            if (!_isIcreatedNew && transform.position.x < _screenWidthConfig)
            {
                CreateNewObstacle();
            }

            //check when obstacle is over the screen to destroy it
            if (transform.position.x + _widthConfig / 2 < -_screenWidthConfig)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            //if not created next obstacle create new when conditions go through
            if (!_isIcreatedNew && transform.position.x > -_screenWidthConfig)
            {
                CreateNewObstacle();
            }

            //check when obstacle is over the screen to destroy it
            if (transform.position.x - _widthConfig / 2 > _screenWidthConfig)
            {
                Destroy(gameObject);
            }
        }

        if (transform.position.y + _heightConfig / 2 < GameManager.S_Instance.CameraGameObject.transform.position.y - _screenHeightConfig) //destroy game object, when its bellow the bottom screen border
        {
            Destroy(gameObject);
        }
    }

    //create new obstacle outside of screen
    void CreateNewObstacle()
    {
        _nextObstacleGameObject = Instantiate(GameManager.S_Instance.ObstaclePrefabGameObject);
        float tempWidth = Random.Range(GameManager.S_Instance.MinObstacleWidthConfig, GameManager.S_Instance.MaxObstacleWidthConfig);

        if (_obstacleSpeedConfig < 0)
            _nextObstacleGameObject.GetComponent<Obstacle>().InitTheObstacle(new Vector2(transform.position.x + Random.Range(_minGapConfig, _maxGapConfig) + _widthConfig / 2 + tempWidth / 2, transform.position.y), _obstacleSpeedConfig, GameManager.S_Instance.GetRandomColorFromConfig(), _screenWidthConfig, _screenHeightConfig, _minGapConfig, _maxGapConfig, _verticalGapConfig, _heightConfig, tempWidth, _isCreatedNewLine);
        else
            _nextObstacleGameObject.GetComponent<Obstacle>().InitTheObstacle(new Vector2(transform.position.x - Random.Range(_minGapConfig, _maxGapConfig) - _widthConfig / 2 - tempWidth / 2, transform.position.y), _obstacleSpeedConfig, GameManager.S_Instance.GetRandomColorFromConfig(), _screenWidthConfig, _screenHeightConfig, _minGapConfig, _maxGapConfig, _verticalGapConfig, _heightConfig, tempWidth, _isCreatedNewLine);

        _isIcreatedNew = true;
    }

    //create new line of obstacles on top of current
    void CreateNewLine()
    {
        _isCreatedNewLine = true;

        //let only one obstacle to create new line
        if (_nextObstacleGameObject != null)
            return;

        GameObject tempObstacle = Instantiate(GameManager.S_Instance.ObstaclePrefabGameObject);
        float tempWidth = Random.Range(GameManager.S_Instance.MinObstacleWidthConfig, GameManager.S_Instance.MaxObstacleWidthConfig);

        if (GameManager.S_Instance.LineNumberCounter % 2 == 0)
            tempObstacle.GetComponent<Obstacle>().InitTheObstacle(new Vector2(0, transform.position.y + GameManager.S_Instance.ObstacleHeightConfig + _verticalGapConfig), Random.Range(GameManager.S_Instance.MinHorizontalSpeedConfig, GameManager.S_Instance.MaxHorizontalSpeedConfig), GameManager.S_Instance.GetRandomColorFromConfig(), _screenWidthConfig, _screenHeightConfig, _minGapConfig, _maxGapConfig, _verticalGapConfig, _heightConfig, tempWidth, false);
        else
            tempObstacle.GetComponent<Obstacle>().InitTheObstacle(new Vector2(0, transform.position.y + GameManager.S_Instance.ObstacleHeightConfig + _verticalGapConfig), Random.Range(-GameManager.S_Instance.MaxHorizontalSpeedConfig, -GameManager.S_Instance.MinHorizontalSpeedConfig), GameManager.S_Instance.GetRandomColorFromConfig(), _screenWidthConfig, _screenHeightConfig, _minGapConfig, _maxGapConfig, _verticalGapConfig, _heightConfig, tempWidth, false);

        GameManager.S_Instance.LineNumberCounter++;
    }
}
