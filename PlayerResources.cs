using System;
using UnityEngine;

public class PlayerResources : MonoBehaviour
{
    [Header("Starting Resources")]
    [SerializeField] private int manpower = 100;
    [SerializeField] private int supplies = 100;

    public int Manpower => manpower;
    public int Supplies => supplies;

    // The UI can listen for this event.
    public event Action OnResourcesChanged;

    public void AddManpower(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("Use SpendManpower() to remove manpower.");
            return;
        }

        manpower += amount;

        OnResourcesChanged?.Invoke();
    }

    public void AddSupplies(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("Use SpendSupplies() to remove supplies.");
            return;
        }

        supplies += amount;

        OnResourcesChanged?.Invoke();
    }

    public bool SpendManpower(int amount)
    {
        if (amount < 0)
            return false;

        if (manpower < amount)
        {
            Debug.Log("Not enough manpower.");
            return false;
        }

        manpower -= amount;

        OnResourcesChanged?.Invoke();

        return true;
    }

    public bool SpendSupplies(int amount)
    {
        if (amount < 0)
            return false;

        if (supplies < amount)
        {
            Debug.Log("Not enough supplies.");
            return false;
        }

        supplies -= amount;

        OnResourcesChanged?.Invoke();

        return true;
    }
}