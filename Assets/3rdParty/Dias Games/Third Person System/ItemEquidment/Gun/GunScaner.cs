using UnityEngine;

// Tên class đã được đổi thành GunScaner theo đúng ý bác
public class GunScaner : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform whiteCircleUI; 
    public RectTransform redCrosshairUI; 

    [Header("Scanner Settings")]
    public float scanRadius = 200f; 
    public LayerMask enemyLayer;
    public float maxShootDistance = 50f;

    private Camera mainCam;
    private Transform currentTarget;

    void Start()
    {
        mainCam = Camera.main;
        
        if (redCrosshairUI != null) 
            redCrosshairUI.gameObject.SetActive(false);
    }

    void Update()
    {
        if (whiteCircleUI != null && whiteCircleUI.gameObject.activeInHierarchy)
        {
            ScanForEnemies();
        }
        else
        {
            currentTarget = null;
            if (redCrosshairUI != null) redCrosshairUI.gameObject.SetActive(false);
        }
    }

    private void ScanForEnemies()
    {
        currentTarget = null;
        float closestDistance = float.MaxValue;
        
        Vector2 scanCenter = RectTransformUtility.WorldToScreenPoint(null, whiteCircleUI.position);
        Collider[] hits = Physics.OverlapSphere(mainCam.transform.position, maxShootDistance, enemyLayer);

        foreach (var hit in hits)
        {
            Vector3 screenPos = mainCam.WorldToScreenPoint(hit.transform.position);
            
            if (screenPos.z > 0) 
            {
                Vector2 enemyScreenPos2D = new Vector2(screenPos.x, screenPos.y);
                float distanceToCenter = Vector2.Distance(scanCenter, enemyScreenPos2D);

                if (distanceToCenter <= scanRadius && distanceToCenter < closestDistance)
                {
                    closestDistance = distanceToCenter;
                    currentTarget = hit.transform;
                }
            }
        }

        if (currentTarget != null)
        {
            redCrosshairUI.gameObject.SetActive(true);
            Vector3 targetScreenPos = mainCam.WorldToScreenPoint(currentTarget.position);
            redCrosshairUI.position = targetScreenPos;
        }
        else
        {
            redCrosshairUI.gameObject.SetActive(false);
        }
    }

    public Transform GetTargetPoint()
    {
        return currentTarget; 
    }

    public StunReceiver GetTarget() 
    {
        if (currentTarget != null)
            return currentTarget.GetComponent<StunReceiver>();
        return null;
    }
}