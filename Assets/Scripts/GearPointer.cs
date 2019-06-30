using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GearPointer : MonoBehaviour
{
    public Inventory Inventory;

    public Image thisImg;

    public Vector3 BasicPos;

    void Update()
    {

    }

    void Start()
    {
        thisImg = GetComponent<Image>();
        BasicPos = transform.position;
    }



}
