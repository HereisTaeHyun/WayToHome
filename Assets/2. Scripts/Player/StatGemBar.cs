using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum StatType
{
    HP,
    Money,
    Damage,
}

[System.Serializable]
public class GemStyle
{
    [SerializeField] private Color emptyPipe;
    [SerializeField] private Color filledPipe;
}

public class StatGemBar : MonoBehaviour
{
    public StatType statType;
    [SerializeField] private List<StatGem> statGems = new List<StatGem>();
    [SerializeField] private GemStyle gemStyle;

    private const int HP_BENCH = 5;
    private const int MONEY_BENCH = 10;
    private const int DAMAGE_BENCH = 1;
    private int activeBench;

    void Awake()
    {
        switch (statType)
        {
            case StatType.HP:
                activeBench = HP_BENCH;
                break;
            case StatType.Money:
                activeBench = MONEY_BENCH;
                break;
            case StatType.Damage:
                activeBench = DAMAGE_BENCH;
                break;
        }
    }

    // 젬 계산 및 스프라이트 적용
    public void RefreshGem(float currentStat)
    {
        int filled = Mathf.Clamp(Mathf.FloorToInt(currentStat / activeBench), 0, statGems.Count);
        Debug.Log($"{statType} : {filled}");
    }
}
