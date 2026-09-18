using System.Collections.Generic;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerResources playerResources;

    [Header("Controlled Provinces")]
    [SerializeField] private List<Province> provinces =
        new List<Province>();

    public void GenerateResources()
    {
        int totalManpower = 0;
        int totalSupplies = 0;

        foreach (Province province in provinces)
        {
            if (province == null)
                continue;

            if (!province.AlignedWithPlayer)
                continue;

            int manpower =
                province.GetManpowerGeneration();

            int supplies =
                province.GetSupplyGeneration();

            totalManpower += manpower;
            totalSupplies += supplies;

            Debug.Log(
                province.ProvinceName +
                " generated " +
                manpower +
                " manpower and " +
                supplies +
                " supplies."
            );
        }

        playerResources.AddManpower(totalManpower);
        playerResources.AddSupplies(totalSupplies);

        Debug.Log(
            "Total income: +" +
            totalManpower +
            " Manpower, +" +
            totalSupplies +
            " Supplies"
        );
    }
}