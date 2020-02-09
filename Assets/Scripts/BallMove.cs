using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Скрипт управления движением шарика.
/// После старта мышкой выбираем куда лететь шарику. Также мышкой можно в любое время задать новую траекторию для движения.
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




    private void Awake()
    {

        m_Transform = gameObject.transform;

        m_LineRenderer = gameObject.GetComponentsInChildren<LineRenderer>();

        m_BoxCollider2D.AddRange(FindObjectsOfType<BoxCollider2D>());

    }

    private void Update()
    {
        
        if (Core.instance.m_GameState == Core.GameState.BallMove)
        {

            CollisionDetection();
            MoveBall();

        }

        UserInputAndDrawLinePath();

    }

    private void MoveBall()
    {

        m_Transform.position = Vector2.MoveTowards(m_Transform.position, moveVector, speed * Time.deltaTime);
        
        m_LineRenderer[0].SetPosition(0, m_Transform.position);
        
    }
    
    /// <summary>
    /// Задаём вектор движения при старте. игрок должен кликнуть мышкой в любом месте экрана.
    /// </summary>
    /// <param name="directionRaycast">Вектор от шарика к положению курсора мышки</param>
    /// <returns></returns>
    private Vector2 FindOnePointToMoveBall(Vector2 directionRaycast)
    {

        hit = Physics2D.Raycast(m_Transform.position, directionRaycast, 300, Core.instance.m_LayerMask);

        if (hit)
        {

            SearhColliderAndDisableHim();

            return hit.point;

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
            else hit = Physics2D.Raycast(moveVector, -Vector2.Reflect(new Vector2(m_Transform.position.x, m_Transform.position.y) - moveVector, Vector2.right), 100, Core.instance.m_LayerMask);

        }
        else
        {

            hit = Physics2D.Raycast(moveVector, -Vector2.Reflect(new Vector2(m_Transform.position.x, m_Transform.position.y) - moveVector, Vector2.up), 100, Core.instance.m_LayerMask);
            if (hit.transform.rotation.eulerAngles.z != 0) hit = Physics2D.Raycast(moveVector, -Vector2.Reflect(new Vector2(m_Transform.position.x, m_Transform.position.y) - moveVector, Vector2.right), 300, Core.instance.m_LayerMask);

        }

        if (hit)
        {

            SearhColliderAndDisableHim();

            return hit.point;

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

    /// <summary>
    /// Чтобы бросок лучами не приходился на тот же объект и в той же точке, к которой мы летим, то после того, как узнали точку, отключем на этом объекте коллайдер. 
    /// А предыдущие включаем.
    /// </summary>
    private void SearhColliderAndDisableHim()
    {

        foreach (BoxCollider2D bc in m_BoxCollider2D)
        {

            if (bc.gameObject == hit.transform.gameObject) bc.enabled = false;
            else bc.enabled = true;

        }

    }

    /// <summary>
    /// Метод который следит за вводом пользователя, ищет точки и рисует линии.
    /// Несколько блоков if - считаем и рисуем в зависимости от того какой стейт игры сейчас идёт и что жал пользователь.
    /// </summary>
    private void UserInputAndDrawLinePath()
    {

        if (Input.GetKeyDown(KeyCode.Mouse0) && Core.instance.m_GameState == Core.GameState.WaitToClickMouse)
        {

            Core.instance.ChangeGameState(Core.GameState.BallMove);

            moveVector = FindOnePointToMoveBall(Camera.main.ScreenToWorldPoint(Input.mousePosition) - m_Transform.position);
            m_LineRenderer[0].SetPosition(1, moveVector);

            nextMoveVector = FindNextPointToMoveBall();
            m_LineRenderer[1].SetPosition(0, moveVector);
            m_LineRenderer[1].SetPosition(1, nextMoveVector);

        }
        else if (Input.GetKeyDown(KeyCode.Mouse0) && Core.instance.m_GameState == Core.GameState.BallMove)
        {

            moveVector = FindOnePointToMoveBall(Camera.main.ScreenToWorldPoint(Input.mousePosition) - m_Transform.position);
            m_LineRenderer[0].SetPosition(1, moveVector);

            nextMoveVector = FindNextPointToMoveBall();
            m_LineRenderer[1].SetPosition(0, moveVector);
            m_LineRenderer[1].SetPosition(1, nextMoveVector);

        }
        else if (Core.instance.m_GameState == Core.GameState.WaitToClickMouse)
        {

            m_LineRenderer[1].SetPosition(0, m_Transform.position);
            m_LineRenderer[1].SetPosition(1, Camera.main.ScreenToWorldPoint(Input.mousePosition));

        }

    }
    
}
