using UnityEngine;
using UnityEngine.UI;

public class healthBarUI : MonoBehaviour
{
    public float Health;
    public float MaxHealth;
    public float width;
    public float height;

    [SerializeField] private RectTransform healthBar;

    public void setMaxHealth(float maxHealth)
    {
        MaxHealth = maxHealth;
    }
    public void setHealth(float health)
    {
        Health = health;
        float newWidth = (Health / MaxHealth) * width;
        healthBar.sizeDelta = new Vector2(newWidth, height);
    }
}
