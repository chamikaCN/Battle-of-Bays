using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusBar : MonoBehaviour
{
    Color color = Color.gray;
    Slider slider;
    int maxHealth;
    public Image fill;

    void Awake()
    {
        slider = GetComponent<Slider>();
        fill.color = color;
    }

    public void setMaxHealth(int health)
    {
        maxHealth = health;
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
    }

    public void changeHealth(int health)
    {
        slider.value = health;
    }

    public void setColor(Color newColor)
    {
        color = newColor;
        fill.color = color;
    }

}
