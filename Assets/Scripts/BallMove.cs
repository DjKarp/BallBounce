using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Скрипт управления движением шарика.
/// 
/// Справедливо для первого коммита на гит хабе - После старта мышкой выбираем куда лететь шарику. Также мышкой можно в любое время задать новую траекторию для движения.
/// 
/// Для крайнего коммита - шарики автоматически стартуют из центра в рандомную сторону. 
/// </summary>
public class BallMove : MonoBehaviour
{

    [Header("Скорость движения шарика")]
    [SerializeField]
    private float speed = 1.0f;

    private Transform m_Transform;

    private Vector2 moveVector = new Vector2(0.0f, 0.0f);
    private Vector2 nextMoveVector = new Vector2(0.0f, 0.0f);

    private RaycastHit2D hit;

    private List<BoxCollider2D> m_BoxCollider2D = new List<BoxCollider2D>();

    private LineRenderer[] m_LineRenderer;

    private Vector2 tempPoint;


    private void Awake()
    {

        m_Transform = gameObject.transform;

        m_LineRenderer = gameObject.GetComponentsInChildren<LineRenderer>();

        m_BoxCollider2D.AddRange(FindObjectsOfType<BoxCollider2D>());

        AutoStartBall();

    }

    private void Update()
    {
        
        if (Core.instance.m_GameState == Core.GameState.BallMove)
        {

            CollisionDetection();
            MoveBall();

        }

    }

    private void MoveBall()
    {

        if (m_LineRenderer[1].GetPosition(0).x == m_LineRenderer[1].GetPosition(1).x && m_LineRenderer[1].GetPosition(0).y == m_LineRenderer[1].GetPosition(1).y) DopStartBall();
        else
        {

            m_Transform.position = Vector2.MoveTowards(m_Transform.position, moveVector, speed * Time.deltaTime);

            m_LineRenderer[0].SetPosition(0, m_Transform.position);

        }
        
    }
    
    /// <summary>
    /// Задаём вектор движения при старте. игрок должен кликнуть мышкой в любом месте экрана.
    /// Или, если это крайний коммит, то направление задаётся рандомно.
    /// </summary>
    /// <param name="directionRaycast">Вектор от шарика к положению курсора мышки</param>
    /// <returns></returns>
    private Vector2 FindOnePointToMoveBall(Vector2 directionRaycast)
    {

        hit = Physics2D.Raycast(m_Transform.position, directionRaycast, 300, Core.instance.m_LayerMask);

        if (hit)
        {

            Vector2 tempDir = new Vector2(m_Transform.position.x, m_Transform.position.y) - hit.point;
            tempPoint = hit.point + (tempDir * 0.01f);

            return tempPoint;

        }
        else
        {

            Debug.Log("No collider find on ray way!");
            return Vector2.one;

        }
        
    }

    /// <summary>
    /// Поиск следующей точки, после той, к которой движемся сейчас.
    /// Так мы сразу показываем по какой траектории полетит шарик, после столкновения с препятствием.
    /// 
    /// Смотрим повёрнут ли объект, к которому полетим, чтобы точно указать какой вектор нормализированный использовать для вычисления вектора отскока.
    /// </summary>
    /// <returns></returns>
    private Vector2 FindNextPointToMoveBall()
    {

        if (hit)
        {

            if (hit.transform.rotation.eulerAngles.z == 0) hit = Physics2D.Raycast(moveVector, -Vector2.Reflect(new Vector2(m_Transform.position.x, m_Transform.position.y) - moveVector, Vector2.up), 300, Core.instance.m_LayerMask);
            else hit = Physics2D.Raycast(moveVector, -Vector2.Reflect(new Vector2(m_Transform.position.x, m_Transform.position.y) - moveVector, Vector2.right), 300, Core.instance.m_LayerMask);

        }
        else
        {

            hit = Physics2D.Raycast(moveVector, -Vector2.Reflect(new Vector2(m_Transform.position.x, m_Transform.position.y) - moveVector, Vector2.up), 300, Core.instance.m_LayerMask);
            if (hit.transform.rotation.eulerAngles.z != 0) hit = Physics2D.Raycast(moveVector, -Vector2.Reflect(new Vector2(m_Transform.position.x, m_Transform.position.y) - moveVector, Vector2.right), 300, Core.instance.m_LayerMask);

        }

        if (hit)
        {

            Vector2 tempDir = new Vector2(m_Transform.position.x, m_Transform.position.y) - hit.point;
            tempPoint = hit.point + (tempDir * 0.01f);
            
            return tempPoint;

        }
        else
        {

            Debug.Log("No collider find on ray way!");
            return Vector2.one;

        }

    }

    /// <summary>
    /// Раз физикой столкновения нельзя пользоваться, то и сами столкновения будем высчитывать по расстоянию между шариком и точкой к которой летит шарик.
    /// Расстояние берём в 2 раза больше диаметра нашего шарика.
    /// После столкновения высчитываем точку, что будет после следующего столкновения и рисуем новые линии.
    /// </summary>
    private void CollisionDetection()
    {

        if(Vector2.Distance(m_Transform.position, moveVector) <= m_Transform.localScale.x * 2)
        {

            moveVector = nextMoveVector;
            nextMoveVector = FindNextPointToMoveBall();

            m_LineRenderer[0].SetPosition(1, moveVector);

            m_LineRenderer[1].SetPosition(0, moveVector);
            m_LineRenderer[1].SetPosition(1, nextMoveVector);

        }

    }

    private void OnEnable()
    {

        AutoStartBall();

    }
    /// <summary>
    /// Метод для автоматического старта шариков.
    /// </summary>
    private void AutoStartBall()
    {

        moveVector = FindOnePointToMoveBall(m_BoxCollider2D[Random.Range(0, m_BoxCollider2D.Count)].transform.position - m_Transform.position);
        m_LineRenderer[0].SetPosition(0, m_Transform.position);
        m_LineRenderer[0].SetPosition(1, moveVector);

        nextMoveVector = FindNextPointToMoveBall();
        m_LineRenderer[1].SetPosition(0, moveVector);
        m_LineRenderer[1].SetPosition(1, nextMoveVector);

        if (Core.instance.m_GameState == Core.GameState.WaitToClickMouse) Core.instance.ChangeGameState(Core.GameState.BallMove);

    }

    /// <summary>
    /// fix
    /// </summary>
    private void DopStartBall()
    {

        moveVector = Vector2.up;
        m_LineRenderer[0].SetPosition(0, m_Transform.position);
        m_LineRenderer[0].SetPosition(1, moveVector);

        nextMoveVector = FindNextPointToMoveBall();
        m_LineRenderer[1].SetPosition(0, moveVector);
        m_LineRenderer[1].SetPosition(1, nextMoveVector);

    }
    
}
