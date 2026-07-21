using UnityEngine;

public class Gun : MonoBehaviour
{

    [Header("Gun's Info")]
    public GameObject upGun;
    public GameObject downGun;
    public GameObject leftGun;
    public GameObject rightGun;
    public int itemPrice = 0;

    public GameObject weponHelper;

    public GameObject cameraPosition;

    public float spinningSpeed = 0.325f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        weponHelper.transform.Rotate(0, -spinningSpeed, 0);
    }
}
