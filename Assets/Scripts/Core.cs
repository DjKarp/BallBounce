using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Главный скрипт всей игры. Реализован по шаблону проектирования "Одиночка".
/// Следит за состоянием игры и запускает на старте указанное количество шариков с указанным в инспекторе промежутком времени.
/// </summary>
public class Core : MonoBehaviour
{

    public static Core instance = null;
    
    [Header("Слои, с которыми шарик должен сталкиваться и отскакивать.")]
    [SerializeField]
    public LayerMask m_LayerMask;

    [Header("Один шарик или много запускать?")]
    [SerializeField]
    public bool isOneBall = true;

    [Header("Если много, то сколько именно штук?")]
    [SerializeField]
    public int ballsCount = 1;

    [Header("Пауза между стартом шариков")]
    [SerializeField]
    public float timeToNextBallStart = 1.0f;

    private GameObject ball;
    private List<GameObject> ballsAll = new List<GameObject>();
    private int activateBallNumber = 0;


    /// <summary>
    /// Переменная, которая хранит состояние игры.
    /// </summary>
    [HideInInspector]
    public GameState m_GameState;

    public enum GameState
    {

        WaitToClickMouse,       //Начало игры, когда игра ждёт от пользователя первого щелчка мышки, чтобы задать направление шарику.
        BallMove                //Стейт самой игры, когда шарик сам летит и отталкиваается.

    }




    private void Awake()
    {

        if (instance == null) instance = this;
        else if (instance == this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        m_GameState = GameState.WaitToClickMouse;

        ball = Resources.Load("Ball") as GameObject;
        for (int i = 0; i < ballsCount; i++)
        {

            ballsAll.Add(Instantiate(ball, Vector3.zero, Quaternion.identity));
            ballsAll[ballsAll.Count - 1].SetActive(false);

        }

    }

    private void Start()
    {

        StartCoroutine(StartBall());

    }

    private void Update()
    {


    }

    /// <summary>
    /// Переключение состояния игры. Сейчас пусто почти, кроме задания m_GameState нужным состоянием, но подорузомевается, что при переключения стейтов, должно что-то происходить. 
    /// </summary>
    /// <param name="newGameState">Новыое состояние игры</param>
    public void ChangeGameState(GameState newGameState)
    {

        m_GameState = newGameState;

        switch (m_GameState)
        {

            case GameState.BallMove:

                break;

            case GameState.WaitToClickMouse:

                break;

        }

    }
    
    public IEnumerator StartBall()
    {

        while(activateBallNumber < ballsAll.Count)
        {

            yield return new WaitForSeconds(timeToNextBallStart);

            ballsAll[activateBallNumber].SetActive(true);
            activateBallNumber++;

            yield return null;

        }

    }

}
