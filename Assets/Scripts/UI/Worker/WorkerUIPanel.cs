using UnityEngine;

public class WorkerUIPanel : MonoBehaviour
{
    [SerializeField] private GameObject workerSlotPrefab;
    [SerializeField] private Transform container;

    private void Start()
    {
        WorkerManager.Instance.OnWorkersChanged += RefreshList;
        RefreshList();
    }

    private void OnDestroy()
    {
        if (WorkerManager.Instance != null)
        {
            WorkerManager.Instance.OnWorkersChanged -= RefreshList;
        }
    }

    private void RefreshList()
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < WorkerManager.Instance.activeWorkers.Count; i++)
        {
            GameObject newSlot = Instantiate(workerSlotPrefab, container);
            WorkerSlotUI slotScript = newSlot.GetComponent<WorkerSlotUI>();
            
            if (slotScript != null)
            {
                slotScript.Setup(i);
            }
        }
    }
}