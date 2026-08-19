using UnityEngine;

public static class NpcMovementUtility
{
    // Ќаправлени€ провер€ютс€ от самого близкого к желаемому до разворота назад.
    private static readonly float[] CheckAngles = { 0f, -30f, 30f, -60f, 60f, -90f, 90f, 135f, -135f, 180f };

    // ¬озвращает свободное направление, максимально близкое к desiredDirection.
    public static Vector3 GetClearDirection(
        Transform owner,
        Vector3 desiredDirection,
        float checkRadius,
        float checkDistance,
        LayerMask obstacleMask)
    {
        desiredDirection.y = 0f;

        if (desiredDirection.sqrMagnitude < 0.001f)
            return Vector3.zero;

        desiredDirection.Normalize();

        foreach (float angle in CheckAngles)
        {
            Vector3 candidate = Quaternion.Euler(0f, angle, 0f) * desiredDirection;

            if (!IsBlocked(owner, candidate, checkRadius, checkDistance, obstacleMask))
                return candidate;
        }

        // ≈сли свободного направлени€ нет, NPC останавливаетс€, а не идЄт в стену.
        return Vector3.zero;
    }

    // ѕровер€ет объЄм перед персонажем и игнорирует его собственные коллайдеры.
    private static bool IsBlocked(
        Transform owner,
        Vector3 direction,
        float checkRadius,
        float checkDistance,
        LayerMask obstacleMask)
    {
        Vector3 origin = owner.position + Vector3.up * Mathf.Max(checkRadius, 0.25f);
        RaycastHit[] hits = Physics.SphereCastAll(
            origin,
            checkRadius,
            direction,
            checkDistance,
            obstacleMask,
            QueryTriggerInteraction.Ignore
        );

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform != owner && !hit.transform.IsChildOf(owner))
                return true;
        }

        return false;
    }
}
