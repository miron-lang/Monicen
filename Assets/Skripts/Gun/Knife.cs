using StarterAssets;
using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public class Knife : MonoBehaviour
{
    float nextAttackTime;
    Vector3 lastHitPoint;
    float hitAreaHideTime;

    Collider[] attackResults = new Collider[16];
    ThirdPersonController playerController;
    bool isAttackWating;

    [Header("Knife Animation")]
    [Tooltip("Animator игрока, в котором создан Trigger KnifeAttack")]
    [SerializeField] Animator playerAnimator;
    [SerializeField] string knifeAttackTrigger = "KnifeAttack";
    [SerializeField] float damageDelay = 0.15f;
    [SerializeField] float moventLockDuration = 0.45f;
    [SerializeField] RuntimeAnimatorController animationController;

    [Header("Knfe Attack")]
    [SerializeField] float knifeDamage = 35f;
    [SerializeField] float attackDistance = 1.25f;
    [FormerlySerializedAs("attackDistance")]
    [SerializeField] float attackSphereRadius = 0.35f;
    [SerializeField] float attackCooldwon = 0.45f;
    [SerializeField] LayerMask attackMask = ~0;

    [Header("Attack Origin")]
    [Tooltip("Точка начяла удара. Оставьте пустой, что бы исползовоть полодений ножа.")]
    [SerializeField] Transform attackOrigin;
    [Tooltip("Игрок нуден, что бы NPC понимал, кто его удраил")]
    [SerializeField] Transform player;

    [Header("Debug Hit Area")]
    [SerializeField] bool showAttackArea = true;
    [SerializeField] bool showHitArea = true;
    [SerializeField] float hitAreaDuration = 1f;
    [Tooltip("Обычный обЪект Sphare для показа облости удара в Game")]
    [SerializeField] GameObject hitSphere;


    [Header("Effects")]
    [SerializeField] GameObject blood;
    [SerializeField] GameObject metalEffect;

    private void Awake()
    {
        if (player == null)
        {
            Player playerComonent = FindFirstObjectByType<Player>();

            if (playerComonent != null)
            {
                player = playerComonent.transform;
            }
        }

        if (playerAnimator == null && player != null)
        {
            playerAnimator = player.GetComponent<Animator>();
        }

        if (player != null)
        {
            playerController = player.GetComponentInParent<ThirdPersonController>();
        }

        if (hitSphere != null)
        {
            hitSphere.SetActive(false);
        }
    }

    void OnEnable()
    {
        playerAnimator.runtimeAnimatorController = animationController;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (Time.time >= nextAttackTime)
            {
                nextAttackTime = Time.time + attackCooldwon;

                if (!isAttackWating)
                {
                    isAttackWating = true;

                    StartKnifeAttack();

                    Invoke("Attack", damageDelay);
                    Invoke("FinishAttack", moventLockDuration);
                }
            }
        }
    }

    void StartKnifeAttack()
    {
        if (playerController != null)
        {
            playerController.SetMovementLocked(true);
        }

        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger(knifeAttackTrigger);
        }
    }

    void FinishAttack()
    {
        if (playerController != null)
        {
            playerController.SetMovementLocked(false);
        }
    }

    void Attack()
    {
        isAttackWating = false;

        Transform origin = attackOrigin != null ? attackOrigin : transform;

        lastHitPoint = origin.position;
        hitAreaHideTime = Time.time + hitAreaDuration;
        ShowHitSphere(origin.position);

        int hitCount = Physics.OverlapSphereNonAlloc(origin.position, attackSphereRadius, attackResults, attackMask, QueryTriggerInteraction.Ignore);

        Collider targetCollider = null;
        float closestDistancSqr = float.PositiveInfinity;

        for (int i = 0; i < hitCount; i++)
        {
            Collider candidate = attackResults[i];

            if (candidate == null || (player != null && candidate.transform.IsChildOf(player)))
            {
                continue;
            }

            bool isCharacter = candidate.GetComponentInParent<PoliceOfficer>() != null || candidate.GetComponentInParent<CharacterNavigatorScript01>() != null || candidate.GetComponentInParent<CharacterNavigatorScript>() != null || candidate.GetComponentInParent<SimpleGuncsterSkript>() != null || candidate.GetComponentInParent<Boss>() != null;

            if (!isCharacter)
            {
                continue;
            }

            float distanceSqr = (candidate.ClosestPoint(origin.position) - origin.position).sqrMagnitude;

            if (distanceSqr < closestDistancSqr)
            {
                targetCollider = candidate;
                closestDistancSqr = distanceSqr;
            }
        }

        if (targetCollider == null)
        {
            return;
        }

        Vector3 hitPoint = targetCollider.ClosestPoint(origin.position);

        Transform targetTransform = targetCollider.transform;

        PoliceOfficer police = targetTransform.GetComponentInParent<PoliceOfficer>();

        CharacterNavigatorScript01 npc = targetTransform.GetComponentInParent<CharacterNavigatorScript01>();

        CharacterNavigatorScript oldNpc = targetTransform.GetComponentInParent<CharacterNavigatorScript>();

        SimpleGuncsterSkript guncster = targetTransform.GetComponentInParent<SimpleGuncsterSkript>();

        Boss boss = targetTransform.GetComponentInParent<Boss>();

        bool damageDealt = false;

        if (police != null)
        {
            police.PoliceGetDamage(knifeDamage);
            damageDealt = true;
        }
        else if (npc != null)
        {
            npc.NpcGetDamage(knifeDamage, player, hitPoint);
            damageDealt = true;
        }
        else if (oldNpc != null)
        {
            oldNpc.NpcGetDamage(knifeDamage);
            damageDealt = true;
        }
        else if (guncster != null)
        {
            guncster.GuncsterGetDamage(knifeDamage);
            damageDealt = true;
        }
        else if (boss != null)
        {
            boss.BossGetDamage(knifeDamage);
            damageDealt = true;
        }
        else if (metalEffect != null)
        {
            Instantiate(metalEffect, hitPoint, Quaternion.identity);
        }

        if (damageDealt && blood != null)
        {
            Instantiate(blood, hitPoint, Quaternion.identity);
        }
    }

    void ShowHitSphere(Vector3 hitPoint)
    {
        if (hitSphere == null)
        {
            return;
        }

        hitSphere.transform.position = hitPoint;
        hitSphere.transform.localScale = Vector3.one * attackSphereRadius * 2f;

        hitSphere.SetActive(true);
        CancelInvoke("HideHitSphere");
        Invoke("HideHitSphare", hitAreaDuration);
    }

    void HideHitSphare()
    {
        if (hitSphere != null)
        {
            hitSphere.SetActive(false);
        }
    }

    private void OnDisable()
    {
        CancelInvoke();

        isAttackWating = false;

        HideHitSphare();

        if (playerController != null)
        {
            playerController.SetMovementLocked(false);
        }
    }
}
