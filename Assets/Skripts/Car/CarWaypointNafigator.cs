using Meta.XR.MRUtilityKit.BuildingBlocks;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //void Start()
    //{
    //    diraction = Random.Range(0, 2);

    //    if (car == null || currentWaypoint == null)
    //    {
    //        return;
    //    }

    //    Vector3 startPosition = currentWaypoint.GetPosition(diraction);

    //    startPosition.y = transform.position.y;

    //    transform.position = startPosition;

    //    SelectNextWaypoint();

    //    if (currentWaypoint != null)
    //    {
    //        car.LoceteDestination(currentWaypoint.GetPosition(diraction));
    //    }
    //}

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

        // Обычный Next, Previus и Branches остоютса один в один как у NPC
        base.Update(); // Запускаем неизменённый обзщий алгоритм NPC
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

            plannedExitDeraction = GetExitDiraction(plannedExitWaypoint);// Сохроняем одну случяйную ветку
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
        LoceteDestination(currentWaypoint.GetPosition(diraction));// Отпровляем бибику на выброную полосу выхода
    }

    // По внешней связи выходной точки определяет нужную половину waypointWidth
    int GetExitDiraction(WayPoint exitWaypoint)
    {
        if (exitWaypoint.nextWaypoint != null && !exitWaypoint.nextWaypoint.isCarIntersection)
        {
            return 1; // GetPositon(1) выбирает сторону, оответсвующую движению через Next
        }

        if (exitWaypoint.peviousWaypoint != null && !exitWaypoint.peviousWaypoint.isCarIntersection)
        {
            return 0; // GetPosition(0) выбирает сторону, оответсвующую движению через Previous
        }

        // Запосное значения, если связи точки настроены непрвилно
        return diraction;
    }

    void FinishCarIntersection()
    {
        CarIntersectionController.Leave(this, reservedIntersectionWaypoint); // Разрешаем въезд слудущей машине
        car.SetIntersectionDriving(false); // Возврощяем обычный датчик препятствий
        isCrossingCarInteresetion = false; // Завершаем спецалное состояние перекрёстка
        reservedIntersectionWaypoint = null; // Очщаем ссылку на уде освобождённый въезд

        // Выбираем единстенную внешню связь и продолжаем по новой дароге
        if (currentWaypoint.nextWaypoint != null && !currentWaypoint.nextWaypoint.isCarIntersection)
        {
            currentWaypoint = currentWaypoint.nextWaypoint;
            diraction = 1;
        }
        else if (currentWaypoint.peviousWaypoint != null && !currentWaypoint.peviousWaypoint.isCarIntersection)
        {
            currentWaypoint = currentWaypoint.peviousWaypoint;
            diraction = 0;
        }
        else
        {
            // СТАРОЕ ОБЩЕЕ ПОВЕДЕНИЯ оставлино запосным вырянтом
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