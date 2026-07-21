using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool isHovered;

    public InvertoryOpen invertoryOpen;

    public GameObject image;

    public int weaponNumber;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        invertoryOpen.selectedSlot = weaponNumber;
        Debug.Log("Навели");

        foreach (Transform child in transform.parent)
        {
            child.GetChild(0).gameObject.SetActive(false);
        }

        image.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        Debug.Log("Убрали курсор");
    }

}