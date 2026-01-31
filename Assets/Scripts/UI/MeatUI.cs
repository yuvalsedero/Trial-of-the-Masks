using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MeatUI : MonoBehaviour
{
    public Image meatIcon;
    public TMP_Text meatText;

    public void UpdateMeat(int amount)
    {
        meatText.text = amount.ToString();
    }
}
