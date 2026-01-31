using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HeartsUI : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public GameObject heartPrefab;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    private List<Image> hearts = new List<Image>();

    void Start()
    {
        InitHearts();
        UpdateHearts();
    }

    void InitHearts()
    {
        // Clear old hearts
        foreach (var img in hearts)
            Destroy(img.gameObject);

        hearts.Clear();

        // Rebuild based on maxHearts
        for (int i = 0; i < playerHealth.maxHearts; i++)
        {
            GameObject heart = Instantiate(heartPrefab, transform);
            Image img = heart.GetComponent<Image>();
            hearts.Add(img);
        }
    }


    public void UpdateHearts()
    {
        if (hearts.Count != playerHealth.maxHearts)
            InitHearts();

        for (int i = 0; i < hearts.Count; i++)
        {
            if (i < playerHealth.currentHearts)
                hearts[i].sprite = fullHeart;
            else
                hearts[i].sprite = emptyHeart;
        }
    }
}
