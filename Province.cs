using UnityEngine;

public class Province : MonoBehaviour
{
    [Header("Province Information")]
    [SerializeField] private string provinceName;
    [SerializeField] private bool alignedWithPlayer = true;

    [Header("Popular Support")]
    [Range(-100f, 100f)]
    [SerializeField] private float popularSupport = 0f;

    [Header("Base Resource Generation")]
    [SerializeField] private int baseManpowerGeneration = 10;
    [SerializeField] private int baseSupplyGeneration = 5;

    public string ProvinceName => provinceName;
    public float PopularSupport => popularSupport;
    public bool AlignedWithPlayer => alignedWithPlayer;

    public SupportLevel CurrentSupportLevel
    {
        get
        {
            if (popularSupport <= -75)
                return SupportLevel.Hostile;

            if (popularSupport <= -25)
                return SupportLevel.Unfriendly;

            if (popularSupport < 25)
                return SupportLevel.Neutral;

            if (popularSupport < 75)
                return SupportLevel.Supportive;

            return SupportLevel.Loyal;
        }
    }

    public void ChangePopularSupport(float amount)
    {
        popularSupport = Mathf.Clamp(
            popularSupport + amount,
            -100f,
            100f
        );

        Debug.Log(
            provinceName +
            " support changed to " +
            popularSupport +
            " (" +
            CurrentSupportLevel +
            ")"
        );
    }

    public int GetManpowerGeneration()
    {
        if (!alignedWithPlayer)
            return 0;

        return Mathf.RoundToInt(
            baseManpowerGeneration *
            GetResourceMultiplier()
        );
    }

    public int GetSupplyGeneration()
    {
        if (!alignedWithPlayer)
            return 0;

        return Mathf.RoundToInt(
            baseSupplyGeneration *
            GetResourceMultiplier()
        );
    }

    public float GetEnemyActivityMultiplier()
    {
        if (popularSupport >= 0)
            return 1f;

        // -100 support = 2x enemy activity
        return 1f + (-popularSupport / 100f);
    }

    private float GetResourceMultiplier()
    {
        // -100 = 50%
        // 0    = 100%
        // +100 = 150%

        return 1f + (popularSupport / 200f);
    }

    public void SetAlignedWithPlayer(bool aligned)
    {
        alignedWithPlayer = aligned;
    }
}

public enum SupportLevel
{
    Hostile,
    Unfriendly,
    Neutral,
    Supportive,
    Loyal
}