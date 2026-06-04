using UnityEngine;

public class GunScaner : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform whiteCircleUI; 
    public RectTransform redCrosshairUI; 

    [Header("Scanner Settings")]
    public float scanRadius = 100f; // Cứ để 100, code sẽ tự quy đổi chuẩn
    public LayerMask enemyLayer;
    public float maxShootDistance = 50f;

    private Camera mainCam;
    
    // ĐỔI TỪ Transform SANG Collider để lấy được tâm ngực
    private Collider currentTargetCollider; 

    void Start()
    {
        mainCam = Camera.main;
        if (redCrosshairUI != null) redCrosshairUI.gameObject.SetActive(false);
    }

    void Update()
    {
        if (whiteCircleUI != null && whiteCircleUI.gameObject.activeInHierarchy)
        {
            ScanForEnemies();
        }
        else
        {
            currentTargetCollider = null;
            if (redCrosshairUI != null) redCrosshairUI.gameObject.SetActive(false);
        }
    }

    private void ScanForEnemies()
    {
        currentTargetCollider = null;
        float closestDistance = float.MaxValue;
        
        // Tự động đo bán kính thực tế của cái vòng trên màn hình
        float dynamicRadius = (whiteCircleUI.rect.width / 2f) * whiteCircleUI.lossyScale.x;
        Vector2 scanCenter = RectTransformUtility.WorldToScreenPoint(null, whiteCircleUI.position);
        
        Collider[] hits = Physics.OverlapSphere(mainCam.transform.position, maxShootDistance, enemyLayer);

        foreach (var hit in hits)
        {
            // ĐIỂM ĂN TIỀN LÀ ĐÂY: Lấy tâm bụng (bounds.center) thay vì gót chân (transform.position)
            Vector3 centerPos = hit.bounds.center; 
            Vector3 screenPos = mainCam.WorldToScreenPoint(centerPos);
            
            if (screenPos.z > 0) 
            {
                Vector2 enemyScreenPos2D = new Vector2(screenPos.x, screenPos.y);
                float distanceToCenter = Vector2.Distance(scanCenter, enemyScreenPos2D);

                // Kiểm tra bằng dynamicRadius
                if (distanceToCenter <= dynamicRadius && distanceToCenter < closestDistance)
                {
                    closestDistance = distanceToCenter;
                    currentTargetCollider = hit;
                }
            }
        }

        if (currentTargetCollider != null)
        {
            redCrosshairUI.gameObject.SetActive(true);
            
            
            Vector3 targetScreenPos = mainCam.WorldToScreenPoint(currentTargetCollider.bounds.center);
            redCrosshairUI.position = targetScreenPos;
        }
        else
        {
            redCrosshairUI.gameObject.SetActive(false);
        }
    }
    public Vector3 GetTargetPosition()
    {
        if (currentTargetCollider != null) 
            return currentTargetCollider.bounds.center;
            
        return Vector3.zero;
    }

    // trả về IImpact
    public IDamage GetTarget() 
    {
        if (currentTargetCollider != null)
            return currentTargetCollider.GetComponent<IDamage>();
        return null;
    }
}