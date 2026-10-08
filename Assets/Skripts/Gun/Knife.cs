using UnityEngine;

public class Knife : MonoBehaviour
{
    [Header("Knfe Attack")]
    [SerializeField] float knifeDamage = 35f;
    [SerializeField] float attackDistance = 1.25f;
    [SerializeField] float attackRadius = 0.35f;
    [SerializeField] float attackCooldwon = 0.45f;
    [SerializeField] LayerMask attackMask = ~0;

    [Header("Attack Origin")]
    [Tooltip("Точка начяла удара. Оставьте пустой, что бы исползовоть полодений ножа.")]
    [SerializeField] Transform attackOrigin;
    [Tooltip("Игрок нуден, что бы NPC понимал, кто его удраил")]
    [SerializeField] Transform player;

    float nextAttackTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
