using Oculus.Interaction.Locomotion;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

public class ObserverController : MonoBehaviour
{
    [Header("References")]
    public Transform cameraRig;   // VR наблюдатель
    public Transform character;       // гуманоид (pivot)

    [Header("Zoom")]
    public float zoomSpeed = 5.0f;
    public float minDistance = 0.2f;
    public float maxDistance = 10.0f;
    public float distance = 3f;

    [Header("Rotation")]
    public float rotateSpeed = 90f;
    public float yaw = 0f;

    [Header("Hight settings")]
    public float minHight = 0f;
    public float maxHight = 8f;
    public float hightSmooth = 5f;

    [Header("Smooothing")]
    public float positionSmooth = 10f;

    Vector3 targetPos;
    bool wasRotating = false;

    private Vector3 pivotPosition;
    private void Start()
    {
        pivotPosition = transform.position;

        Vector3 offset = cameraRig.position - pivotPosition;












        distance = offset.magnitude;
        yaw = Mathf.Atan2(offset.x, offset.z) * Mathf.Rad2Deg;
    }

    void Update()
    {
        Vector2 stick =
            OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);

        HandleZoom(stick.y);
        HandleRotation(stick.x);

        UpdatePosition();

    }

    public void RecenterOnCharecter()
    {
        pivotPosition = Vector3.MoveTowards(pivotPosition, character.position, Time.deltaTime * positionSmooth);
    }

    void HandleZoom(float input)
    {
        if (Mathf.Abs(input) < 0.1f) return;

        RecenterOnCharecter();

        // 👉 направление ОТ персонажа К текущей позиции VR
        distance -= input * zoomSpeed * Time.deltaTime;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
    }

    void SnapPivotToCharacter()
    {
        RecenterOnCharecter();

        Vector3 offset = targetPos - pivotPosition;

        distance = offset.magnitude;

        yaw = Mathf.Atan2(offset.x, offset.z) * Mathf.Rad2Deg;
    }

    void HandleRotation(float input)
    {
        bool rotating = Mathf.Abs(input) > 0.1f;

        if (rotating && !wasRotating)
        {
            SnapPivotToCharacter();
        }

        if (rotating)
        {
            yaw -= input * rotateSpeed * Time.deltaTime;
        }

        wasRotating = rotating;

    }

    void UpdatePosition()
    {
               Quaternion rot = Quaternion.Euler(0f, yaw, 0f);

        Vector3 offset = rot * Vector3.forward * distance;
        targetPos = pivotPosition + offset;

        float t = Mathf.InverseLerp(minDistance, maxDistance, distance);
        float targetHight = Mathf.Lerp(minHight, maxHight, t);
        float targetY = character.position.y + targetHight;

        targetPos.y = Mathf.Lerp(cameraRig.position.y, targetY, Time.deltaTime * hightSmooth);

        cameraRig.position = Vector3.Lerp(cameraRig.position,targetPos,Time.deltaTime * positionSmooth);  // скорость сглаживания 

        Vector3 toCharacter = character.position - cameraRig.position;
        float realDistance = toCharacter.magnitude;

        if (realDistance > maxDistance)
        {
            Vector3 correctedPos = character.position - toCharacter.normalized * maxDistance;

            cameraRig.position = Vector3.Lerp(cameraRig.position,correctedPos,Time.deltaTime * positionSmooth);  // скорость сглаживания

        }

        Vector3 dir = pivotPosition - cameraRig.position;
        dir.y = 0f;

        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            cameraRig.rotation = Quaternion.Lerp(cameraRig.rotation, targetRot, Time.deltaTime * 10f);
        }

    }

}