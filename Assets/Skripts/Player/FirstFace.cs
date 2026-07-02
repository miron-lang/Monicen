using Meta.XR.Movement.Retargeting;
using StarterAssets;
using UnityEngine;

public class FirstFace : MonoBehaviour
{
    public Transform cameraRig;
    public Transform character;

    private bool inFirstPerson = false;

    private Vector3 savedPos;
    private Quaternion savedRot;
    private Vector3 savedScale;

    public GameObject LeftHend;
    public GameObject RightHend;

    public CharacterRetargeter retarget;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (retarget.enabled && !inFirstPerson)
        {
            EnterFirstPerson();
        }
        else if (!retarget.enabled && inFirstPerson)
        {
            ExitFirstPerson();
        }    
    }

    void EnterFirstPerson()
    {
        inFirstPerson = true;
        savedPos = cameraRig.position;
        //savedScale = cameraRig.localScale;
        savedRot = cameraRig.rotation;

        character.GetComponent<ObserverController>().enabled = false;
        character.GetComponent<ThirdPersonController>().enabled = false;
        character.GetComponent<CharacterController>().enabled = false;

        LeftHend.SetActive(false);
        RightHend.SetActive(false);

        cameraRig.position = character.position;
        //cameraRig.localScale = character.localScale;
        cameraRig.rotation = character.rotation;
        character.SetParent(cameraRig, true);
        
    }

    void ExitFirstPerson()
    {
        character.SetParent(null, true);

        cameraRig.position = savedPos;
        //cameraRig.localScale = savedScale;
        cameraRig.rotation = savedRot;

        character.GetComponent<CharacterController>().enabled = true;
        character.GetComponent<ThirdPersonController>().enabled = true;
        character.GetComponent<ObserverController>().enabled = true;

        LeftHend.SetActive(true);
        RightHend.SetActive(true);
 
        inFirstPerson = false;
    }

} 
 

