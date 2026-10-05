using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Vector3 marimeOriginala;


    public float multiplicatorMarire = 1.1f;

    void Start()
    {
      
        marimeOriginala = transform.localScale;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = marimeOriginala * multiplicatorMarire;
    }

   
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = marimeOriginala;
    }
}