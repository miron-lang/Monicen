using UnityEngine;

public class WaypointNavigator : WaypointNavigatorBase
{

    [Header("AI ¬ас€")]
    public CharacterNavigatorScript01 charcater;
    protected override bool hasCharacter => charcater != null;
    protected override bool hasDestinationReached => charcater != null && charcater.destinationReached;

    Vector3 dangerPosition;

    private void Awake()
    {
        charcater = GetComponent<CharacterNavigatorScript01>();
    }

    protected override void LoceteDestination(Vector3 destination)
    {
        charcater.LoceteDestination(destination);
    }

    protected override void Update()
    {
        if (charcater != null && charcater.IsEscaping())
        {
            if (currentWaypoint != null && charcater.destinationReached)
            {
                SelectEscapeWaypoint();
            }
            return;
        }

        base.Update();
    }

    // ¬ыбирает среди next, previous и branches точку, напровление к которой наиболее противоположно игроку
    bool SelectEscapeWaypoint()
    {
        WayPoint selectedWaypoint = null;
        int selectedDiraction = diraction;
        float bestDirectionScore = float.NegativeInfinity;

        TrySelectEscapeWaypoint(currentWaypoint.nextWaypoint, 1, ref selectedWaypoint, ref selectedDiraction, ref bestDirectionScore);
        TrySelectEscapeWaypoint(currentWaypoint.peviousWaypoint, 0, ref selectedWaypoint, ref selectedDiraction, ref bestDirectionScore);

        // ¬етки продолжают роботать: это неабходимо, чтобы выйти с перехода
        if (currentWaypoint.branches != null)
        {
            for (int i = 0; i < currentWaypoint.branches.Count; i++)
            {
                TrySelectEscapeWaypoint(currentWaypoint.branches[i], diraction, ref selectedWaypoint, ref selectedDiraction, ref bestDirectionScore);
            }
        }

        if (selectedWaypoint == null)
        {
            return false;
        }

        currentWaypoint = selectedWaypoint;
        diraction = selectedDiraction;

        // ≈сли паника вывела NPC с зебры по ветке, завершаем режим перехода
        // ѕосле оконч€ни€ паники обычно€ логика снова соможет выбирать морут корректно
        if (isCrossing && !currentWaypoint.isCrosswalk)
        {
            isCrossing = false;
            justExitCrosswalk = true;
        }

        // «апоминаем, что NPC оказалс€ на зебре. Ќапровление diraction намерено не мен€ем:
        // ≈го уже бырал алгоритм паники как напровленит€ от игрока
        // SetCrosswalkDirection здесь ек вызываетс€, потому что он мог заметить это
        // Ќаправление и развернуть NPC обратно к стрел€вшему
        if (currentWaypoint.isCrosswalk)
        {
            isCrossing = true;
        }

        LoceteDestination(currentWaypoint.GetPosition(diraction));
        return true;
    }

    // ќбновл€ет выбронную точку, если направлние к ней лучше совпадает с направлением от игрока
    void TrySelectEscapeWaypoint(WayPoint candidate, int candidateDiracion, ref WayPoint selectedWaypont, ref int selectedDiraction, ref float bestDirectionScore)
    {
        if (candidate == null)
        {
            return;
        }

        // Ќаправление, в катором NPC пойдЄт к рассматривамой точке
        Vector3 candidateDirecion = candidate.transform.position - transform.position;
        candidateDirecion.y = 0f;

        //  Ќапрвление от игрока к NPC - это искома€ сторона побега
        Vector3 escapeDiracion = transform.position - dangerPosition;
        escapeDiracion.y = 0f;

        if (candidateDirecion.sqrMagnitude < 0.01f || escapeDiracion.sqrMagnitude < 0.01f)
        {
            return;
        }

        // 1 = строго от игрока, 0 = вбок, -1 = пр€мо к игроку
        float directionScore = Vector3.Dot(candidateDirecion.normalized, escapeDiracion.normalized);
        if (directionScore <= bestDirectionScore)
        {
            return;
        }

        selectedWaypont = candidate;
        selectedDiraction = candidateDiracion;
        bestDirectionScore = directionScore;
    }

    public bool StartEscape(Vector3 dangerPosition)
    {
        if (charcater == null || currentWaypoint == null)
        {
            return false;
        }

        this.dangerPosition = dangerPosition;

        return SelectEscapeWaypoint();

        //WayPoint nextWaypoint = currentWaypoint.nextWaypoint;
        //WayPoint previusWaypoint = currentWaypoint.peviousWaypoint;
        //float nextScore = GetEscapeScore(nextWaypoint, escapeDirection);
        //float previusScore = GetEscapeScore(previusWaypoint, escapeDirection);

        //if (nextScore == float.NegativeInfinity && previusScore == float.NegativeInfinity)
        //{
        //    return false;
        //}

        //if (nextScore >= previusScore)
        //{
        //    currentWaypoint = nextWaypoint;
        //    diraction = 1;
        //}
        //else
        //{
        //    currentWaypoint = previusWaypoint;
        //    diraction = 0;
        //}

        //isCrossing = false;
        //justExitCrosswalk = false;

        //LoceteDestination(currentWaypoint.GetPosition(diraction));
        //return true;
    }

//    float GetEscapeScore(WayPoint candidate, Vector3 escapeDiraction)
//    {
//        if (candidate == null)
//        {
//            return float.NegativeInfinity;
//        }

//        Vector3 canditateDirection = candidate.transform.position - transform.position; 
//        canditateDirection.y = 0;

//        if (canditateDirection.sqrMagnitude < 0.01f)
//        {
//            return float.NegativeInfinity;
//        }

//        return Vector3.Dot(canditateDirection.normalized, escapeDiraction);
//;
//    }
}