using UnityEngine;
using UnityEngine.EventSystems;

public class ItemInputHandler : MonoBehaviour
{
    [SerializeField] private ItemSpawner itemSpawner;

    private bool isHolding = false;

    void Update()
    {
        ItemBase item = itemSpawner.GetCurrentBehaviour();
        if (item == null) return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) return;

            if (touch.phase == TouchPhase.Began)
            {
                item.OnUse();
                isHolding = true;
            }
            else if (touch.phase == TouchPhase.Stationary && isHolding)
            {
                item.OnUsing();
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                item.OnUseEnd();
                isHolding = false;
            }
        }
        else
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            if (Input.GetMouseButtonDown(0))
            {
                item.OnUse();
                isHolding = true;
            }
            else if (Input.GetMouseButton(0) && isHolding)
            {
                item.OnUsing();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                item.OnUseEnd();
                isHolding = false;
            }
        }
    }
}