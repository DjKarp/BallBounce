using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Главный скрипт всей игры. Реализован по шаблону проектирования "Одиночка".
/// Следит за состоянием игры
/// </summary>
public class Core : MonoBehaviour
{

    public static Core instance = null;
    
    [Header("Слои, с которыми шарик должен сталкиваться и отскакивать.")]
    [SerializeField]
    public LayerMask m_LayerMask;


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

}
