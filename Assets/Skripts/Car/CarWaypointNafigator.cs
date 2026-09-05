using Meta.WitAi;
using Meta.XR.MRUtilityKit.BuildingBlocks;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CarWaypointNafigator : WaypointNavigatorBase
{
    [Header("AI Car")]
    public CarNavigator car;

    [Header("Car Intersection")]
    [Tooltip("Радиус проверки свободного места на выбранной выходной полосе")]
    [SerializeField] float exitCheckRadius = 1.5f; //Радиус сферы вокруг выброной половины waypoint

    [Tooltip("Слои коллайдеров машин, каторые могут занимать выход")]
    [SerializeField] LayerMask carMask = ~0; // По умлочянию проверям все слои, затем ищём CarWaypointNafingator

    bool isCrossingCarInteresetion; // true от рарешения на въезд до достижения выброного выхода
    WayPoint reservedIntersectionWaypoint; // Въездная точка нужна, что бы освободить правилный перекрёсток
    WayPoint plannedExitWaypoint; // Случяйно выброный выход сохроняеца, пока бибика ожидает
    int plannedExitDeraction; // Половтна waypointWidht, соотекстушея выцходной полосе

    protected override bool hasCharacter => car != null;
    protected override bool hasDestinationReached => car != null && car.destinationReached;

    private void Awake()
    {
        car = GetComponent<CarNavigator>();
    }

    protected override void LoceteDestination(Vector3 destination)
    {
        car.LoceteDestination(destination);
    }

    protected override void Start()
    {
        diraction = Random.Range(0, 2);

        if (hasCharacter && currentWaypoint != null)
        {
            LoceteDestination(GetCarLanePosition(currentWaypoint, diraction));
        }

    }

    protected override void Update()
    {
        if (!hasCharacter || currentWaypoint == null || !hasDestinationReached)// Проврям бибику, точку и дочтежения цули
        {
            return;
        }

        // Бибика достигла выброной выхдной полоины waypoint
        if (isCrossingCarInteresetion)
        {
            FinishCarIntersection();// Освабождаем перекрёсток и выбираем продолжения дороги
            return;
        }

        // На краю перёкрекрёска скачяло выбираем ветку и спрашивоем разрешения
        if (currentWaypoint.isCarIntersection)
        {
            TryStartCarIntersection();// Выбираем выход, проверяем место и пытаемся получить розрешения
            return;
        }

        SelectNextWaypoint();

        if (currentWaypoint != null)
        {
            LoceteDestination(GetCarLanePosition(currentWaypoint, diraction));
        }
    }

    void TryStartCarIntersection()
    {
        // Выбираем случяйну ветку один раз и не меняем решения во время
        if (plannedExitWaypoint == null)
        {
            // Если точку случяйно отметили перекрёстком без веток
            if (currentWaypoint.branches == null || currentWaypoint.branches.Count == 0)
            {
                // Используем старую общею логику и не блокирем бибику
                SelectNextWaypoint();

                if (currentWaypoint != null)
                {
                    LoceteDestination(currentWaypoint.GetPosition(diraction));
                }

                return;
            }

            plannedExitWaypoint = currentWaypoint.branches[Random.Range(0, currentWaypoint.branches.Count)];// Определяем правилую выходную полосу

            plannedExitDeraction = GetExitDiraction(currentWaypoint ,plannedExitWaypoint);// Сохроняем одну случяйную ветку
        }

        // Пока внутрои другая мошина или занята выходная полома, остаёмся на месте
        if (!CarIntersectionController.TryEnter(this, currentWaypoint, plannedExitWaypoint, plannedExitDeraction, exitCheckRadius, carMask))
        {
            return;
        }

        reservedIntersectionWaypoint = currentWaypoint; // Запоминаем перерёсток, каторый знали
        currentWaypoint = plannedExitWaypoint; // Новая текщая цель - выброный край перекрёста
        diraction = plannedExitDeraction;// Исползуем выходную пловину широкой точки
        plannedExitWaypoint = null;// Разрешения уже принето, временная ссылка болше не нужна
        isCrossingCarInteresetion = true;// Слудующее достиженя точки будет озночять завершенгия проезда

        // Единственоя допкщеноя бибика должна гарантировно покинуть центр
        car.SetIntersectionDriving(true);// Не позволяем боковым очередям оставить бибику в центре
        LoceteDestination(GetIntersectionExitLanePosition(currentWaypoint, diraction));
    }

    // По внешней связи выходной точки определяет нужную половину waypointWidth
    int GetExitDiraction(WayPoint entryWaypoint ,WayPoint exitWaypoint)
    {
        Vector3 interectionDirection = exitWaypoint.transform.position - entryWaypoint.transform.position;
        interectionDirection.y = 0;

        if (interectionDirection.sqrMagnitude < 0.001f)
        {
            return diraction;
        }

        interectionDirection.Normalize();

        float nextScore = GetExitDiractionScore(exitWaypoint, exitWaypoint.nextWaypoint, interectionDirection);
        float previusScore = GetExitDiractionScore(exitWaypoint, exitWaypoint.peviousWaypoint, interectionDirection);

        if (nextScore > float.NegativeInfinity || previusScore > float.NegativeInfinity)
        {
            return nextScore >= previusScore ? 1 : 0;
        }

        // Запосное значения, если связи точки настроены непрвилно
        return diraction;
    }

    float GetExitDiractionScore(WayPoint exitWaypoint, WayPoint roadWaypoint, Vector3 interectionDirection)
    {
        if (roadWaypoint == null)
        {
            return float.NegativeInfinity;
        }

        Vector3 roadDiration = roadWaypoint.transform.position - exitWaypoint.transform.position;
        roadDiration.y = 0;

        if (roadDiration.sqrMagnitude < 0.001)
        {
            return float.NegativeInfinity;
        }

        return Vector3.Dot(interectionDirection, roadDiration.normalized);
    }

    Vector3 GetIntersectionExitLanePosition(WayPoint waypoint, int diraction)
    {
        WayPoint roadWaypoint = diraction == 1 ? waypoint.nextWaypoint : waypoint.peviousWaypoint;

        if (roadWaypoint == null)
        {
            return waypoint.GetPosition(diraction);
        }

        Vector3 roadDiraction = roadWaypoint.transform.position - waypoint.transform.position;
        roadDiraction.y = 0;

        if (roadDiraction.sqrMagnitude < 0.001f)
        {
            return waypoint.GetPosition(diraction);
        }

        // Вычесяляем правую сторону новой дороги, на каторую бибика должна вехат ьпосле поворота
        Vector3 rightSide = Vector3.Cross(Vector3.up, roadDiraction.normalized);
        return waypoint.transform.position + rightSide * (waypoint.waypointWidth * 0.5f);
    }


    // ИСПРАВЛИНО: возрощяет правую полосу входящего учястка дороги
    // Берём направленитй от предедущей точки к с текущй, поэтому цель не сразает угол по деагонали
    Vector3 GetCarLanePosition(WayPoint waypoint, int diraction)
    {
        WayPoint sourceWaypoint = diraction == 1 ? waypoint.peviousWaypoint : waypoint.nextWaypoint;

        if (sourceWaypoint == null)
        {
            // Используем старый способ: одну из двух сторон самого waypoint-а
            return waypoint.GetPosition(diraction);
        }

        // Вычесляем наровления входящего дорожного участка
        Vector3 roadDiraction = waypoint.transform.position - sourceWaypoint.transform.position;
        roadDiraction.y = 0;

        // Защита от совподающий waypoint-ов
        if (roadDiraction.sqrMagnitude < 0.001)
        {
            return waypoint.GetPosition(diraction);
        }

        // Cross с Vector3.up даёт вектор строго вправо относително напровления движения
        Vector3 rightSide = Vector3.Cross(Vector3.up, roadDiraction.normalized);
        return waypoint.transform.position + rightSide * (waypoint.waypointWidth * 0.5f);
    }

    void FinishCarIntersection()
    {
        CarIntersectionController.Leave(this, reservedIntersectionWaypoint); // Разрешаем въезд слудущей машине
        car.SetIntersectionDriving(false); // Возврощяем обычный датчик препятствий
        isCrossingCarInteresetion = false; // Завершаем спецалное состояние перекрёстка
        reservedIntersectionWaypoint = null; // Очщаем ссылку на уде освобождённый въезд
        WayPoint rodWaypoint = diraction == 1 ? currentWaypoint.nextWaypoint : currentWaypoint.peviousWaypoint;

        if (rodWaypoint == null)
        {
            rodWaypoint = diraction == 1 ? currentWaypoint.peviousWaypoint : currentWaypoint.nextWaypoint;

            diraction = diraction == 1 ? 0 : 1;
        }
        if (rodWaypoint != null)
        {
            currentWaypoint = rodWaypoint;
        }
        else
        {
            SelectNextWaypoint();
        }

        if (currentWaypoint != null)
        {
            LoceteDestination(currentWaypoint.GetPosition(diraction));
        }
    }

    private void OnDisable()
    {
        if (reservedIntersectionWaypoint != null)
        {
            CarIntersectionController.Leave(this, reservedIntersectionWaypoint);
        }

        if (car != null)
        {
            car.SetIntersectionDriving(false);
        }
    }

    //void SelectNextWaypoint()
    //{
    //    bool shouldBranch = currentWaypoint.branches != null && currentWaypoint.branches.Count > 0 && Random.value <= currentWaypoint.branchRatio;

    //    if (shouldBranch)
    //    {
    //        currentWaypoint = currentWaypoint.branches[Random.Range(0, currentWaypoint.branches.Count)];
    //        return;
    //    }

    //    if (diraction == 0)
    //    {
    //        if (currentWaypoint.peviousWaypoint != null)
    //            currentWaypoint = currentWaypoint.peviousWaypoint;

    //        else
    //        {
    //            currentWaypoint = currentWaypoint.nextWaypoint;
    //            diraction = 1;
    //        }
    //    }

    //    else
    //    {
    //        if (currentWaypoint.nextWaypoint != null)
    //            currentWaypoint = currentWaypoint.nextWaypoint;

    //        else
    //        {
    //            currentWaypoint = currentWaypoint.peviousWaypoint;
    //            diraction = 0;
    //        }
    //    } 
    //}

}