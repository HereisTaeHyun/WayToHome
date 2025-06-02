using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public enum StatType
{
    HP,
    Money,
    Damage,
}

[System.Serializable]
public class GemStyle
{
    public Color emptyPipe;
    public Color filledPipe;
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
    private Image pipeImage;

    void Awake()
    {
        // 각 바에 맞는 상태 지정
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

        for (int i = 0; i <= filled; i++)
        {
            statGems[i].core.SetActive(true);

            if (statGems[i].pipe != null)
            {
                pipeImage = statGems[i].pipe.GetComponent<Image>();
                pipeImage.color = gemStyle.filledPipe;
            }
        }
        Debug.Log($"{statType} : {filled}");
    }
}
