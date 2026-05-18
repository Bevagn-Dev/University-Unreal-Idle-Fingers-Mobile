using UnityEngine;
using System.Collections.Generic;

public class WorkerUIPanel : MonoBehaviour
{
    [SerializeField] private GameObject workerSlotPrefab;
    [SerializeField] private Transform container;

    private List<WorkerSlotUI> spawnedSlots = new List<WorkerSlotUI>();

    private void Start()
    {
        if (WorkerManager.Instance != null)
        {
            WorkerManager.Instance.OnWorkersChanged += RefreshList;
            RefreshList();
        }
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
        int targetCount = WorkerManager.Instance.ActiveWorkers.Count;

        for (int i = 0; i < targetCount; i++)
        {
            if (i >= spawnedSlots.Count)
            {
                GameObject newSlot = Instantiate(workerSlotPrefab, container);
                if (newSlot.TryGetComponent(out WorkerSlotUI slotScript))
                {
                    spawnedSlots.Add(slotScript);
                }
            }

            spawnedSlots[i].gameObject.SetActive(true);
            spawnedSlots[i].Setup(i);
        }

        for (int i = targetCount; i < spawnedSlots.Count; i++)
        {
            spawnedSlots[i].gameObject.SetActive(false);
        }
    }
}