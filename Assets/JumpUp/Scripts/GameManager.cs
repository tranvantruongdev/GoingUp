using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager S_Instance { get; set; }

    public UIManager UIManagerInstance;
    public ScoreManager ScoreManagerInstance;

    [Header("Game settings")]
    [Space(5)]
    public GameObject CameraGameObject;
    public GameObject PlayerGameObject;
    public Rigidbody2D PlayerRigidbody2DInstance;
    public Material TrailMaterialAsset;
    public GameObject StartPlatformGameObject;
    [Space(5)]
    public Color[] ColorsConfig;
    [Space(5)]
    [Header("Obstacles settings")]
    public GameObject ObstaclePrefabGameObject;
    [Space(5)]
    public float ObstacleHeightConfig = .2f;
    public float MinObstacleWidthConfig = 1f;
    public float MaxObstacleWidthConfig = 2f;
    public float VerticalGapBetweenLinesConfig = 2f;
    [Range(.5f, 1.5f)]
    public float MinHorizontalGapConfig = 1f;
    [Range(1.5f, 2.5f)]
    public float MaxHorizontalGapConfig = 2f;
    [Range(1f, 2f)]
    public float MinHorizontalSpeedConfig = 1.5f;
    [Range(2f, 3f)]
    public float MaxHorizontalSpeedConfig = 2.5f;
    [Range(.5f, 1.5f)]
    public float MinVerticalSpeedConfig = 1f;
    [Range(2, 3)]
    public float MaxVerticalSpeedConfig = 2.5f;
    [Space(5)]
    public bool IsStarted;
    public int LineNumberCounter;
    public bool IsInAir;

    Vector3 _screenSizeStoring; //for storing screen size
    GameObject _lastObstacleStoring; //for storing last created obstacle

    void Awake()
    {
        DontDestroyOnLoad(this);

        if (S_Instance == null)
            S_Instance = this;
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        Physics2D.gravity = new Vector2(0, -9.81f);

        _screenSizeStoring = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        // Initialize colors array
        ColorsConfig = new Color[]
        {
            new (1f, 0f, 0f, 1f),        // Red
            new (0f, 1f, 0f, 1f),        // Green
            new (0f, 0f, 1f, 1f),        // Blue
            new (1f, 1f, 0f, 1f),        // Yellow
            new (1f, 0f, 1f, 1f),        // Magenta
            new (0f, 1f, 1f, 1f),        // Cyan
            new (1f, 0.5f, 0f, 1f),      // Orange
            new (0.5f, 0f, 1f, 1f),      // Purple
            new (1f, 0.75f, 0.8f, 1f),   // Pink
            new (0.5f, 0.25f, 0f, 1f),   // Brown
            new (0.5f, 0.5f, 0.5f, 1f),  // Gray
            new (0f, 0.5f, 0f, 1f),      // Dark Green
            new (0f, 0f, 0.5f, 1f),      // Dark Blue
            new (0.5f, 0f, 0f, 1f),      // Dark Red
            new (1f, 0.65f, 0f, 1f),     // Gold
            new (0.75f, 0.75f, 0.75f, 1f), // Light Gray
            new (0.25f, 0.25f, 0.25f, 1f), // Dark Gray
            new (1f, 0.8f, 0.2f, 1f),    // Bright Yellow
            new (0.2f, 0.8f, 1f, 1f),    // Sky Blue
            new (0.8f, 0.2f, 1f, 1f),    // Bright Purple
            new (1f, 0.2f, 0.8f, 1f),    // Hot Pink
            new (0.2f, 1f, 0.8f, 1f),    // Turquoise
            new (0.8f, 1f, 0.2f, 1f),    // Lime Green
            new (1f, 0.4f, 0.2f, 1f),    // Coral
            new (0.4f, 0.2f, 1f, 1f),    // Royal Blue
            new (0.2f, 0.4f, 1f, 1f),    // Electric Blue
            new (1f, 0.2f, 0.4f, 1f),    // Rose Red
            new (0.4f, 1f, 0.2f, 1f),    // Bright Green
            new (0.6f, 0.3f, 0.8f, 1f),  // Lavender
            new (0.8f, 0.6f, 0.2f, 1f),  // Amber
            new (0.3f, 0.8f, 0.6f, 1f),  // Teal
            new (0.8f, 0.3f, 0.6f, 1f)   // Fuchsia
        };

        CreateNewScene();
    }

    void Update()
    {
        if (UIManagerInstance.GameStateEnum == GameStateEnum.RUNNING)
        {
            if (UIManagerInstance.IsButtonClicked())
                return;

            if (Input.GetMouseButton(0))
            {
                if (IsStarted && !IsInAir)
                {
                    PlayerRigidbody2DInstance.linearVelocity = Vector2.zero;
                    PlayerRigidbody2DInstance.AddForce(new Vector2(0, 190));
                    IsInAir = true;
                }

                if (!IsStarted)
                {
                    IsStarted = true;
                    PlayerGameObject.GetComponent<TrailRenderer>().enabled = true;
                    ScoreManager.Instance.StartCountingCouroutine();
                    StartPlatformGameObject.SetActive(false);
                }

            }

            //when player goes bellow the screen trigger game over
            if (PlayerGameObject.transform.position.y < CameraGameObject.transform.position.y - _screenSizeStoring.y)
            {
                SetGameOver();
            }
            else if (PlayerGameObject.transform.position.x > _screenSizeStoring.x || PlayerGameObject.transform.position.x < -_screenSizeStoring.x) // if player go off screen on the left or right side
            {
                SetGameOver();
            }
        }
    }

    //spawn first obstacle
    public void SpawnObstacleObjects()
    {
        _lastObstacleStoring = Instantiate(ObstaclePrefabGameObject);
        _lastObstacleStoring.GetComponent<Obstacle>().InitTheObstacle(new Vector2(0, 0), Random.Range(MinHorizontalSpeedConfig, MaxHorizontalSpeedConfig), GetRandomColorFromConfig(), _screenSizeStoring.x, _screenSizeStoring.y, MinHorizontalGapConfig, MaxHorizontalGapConfig, VerticalGapBetweenLinesConfig, ObstacleHeightConfig, Random.Range(MinObstacleWidthConfig, MaxObstacleWidthConfig), false);
        LineNumberCounter++;
    }

    //create new scene
    public void CreateNewScene()
    {
        CameraGameObject.GetComponent<CameraFollowTarget>().ResetTheCameraPosition();
        LineNumberCounter = 0;
        IsInAir = false;
        ShowPlayerObject();
        SpawnObstacleObjects();
    }

    //set player and platform
    public void ShowPlayerObject()
    {
        Color tempColor = GetRandomColorFromConfig();

        IsStarted = false;
        PlayerGameObject.SetActive(true);
        TrailMaterialAsset.color = tempColor;
        PlayerGameObject.GetComponent<SpriteRenderer>().color = tempColor;
        PlayerGameObject.transform.position = new Vector2(0, -2f);

        StartPlatformGameObject.SetActive(true);
        StartPlatformGameObject.transform.position = new Vector2(0, -2.4f);
        StartPlatformGameObject.GetComponent<SpriteRenderer>().color = tempColor;
    }

    public Color GetRandomColorFromConfig()
    {
        return ColorsConfig[Random.Range(0, ColorsConfig.Length)];
    }

    //restart game, reset score, show player, open sides
    public void RestartTheGame()
    {
        if (UIManagerInstance.GameStateEnum == GameStateEnum.PAUSED_GAME)
            Time.timeScale = 1;

        ClearSceneUI();
        CreateNewScene();
        ScoreManagerInstance.ResetTheCurrentScoreValue();
        IsStarted = false;

        UIManagerInstance.ShowGameplayUI();
    }

    //clear all blocks from scene and reset camera position
    public void ClearSceneUI()
    {
        IsStarted = false;
        GameObject[] arrObstacles = GameObject.FindGameObjectsWithTag("Obstacle");

        foreach (GameObject obstacle in arrObstacles)
        {
            Destroy(obstacle);
        }
    }

    //show game over gui
    public void SetGameOver()
    {
        if (UIManagerInstance.GameStateEnum == GameStateEnum.RUNNING)
        {
            ScoreManager.Instance.StopCountingCouroutine();
            AudioManager.S_Instance.PlayEffectsAudio(AudioManager.S_Instance.GameOverAudio);
            AudioManager.S_Instance.PlayMusicClip(AudioManager.S_Instance.MenuMusicAudio);
            UIManagerInstance.ShowGameOverUI();
            PlayerGameObject.SetActive(false);
            PlayerGameObject.GetComponent<TrailRenderer>().enabled = false;
            ScoreManagerInstance.UpdateScoreGameover();
        }
    }
}
