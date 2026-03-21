using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HealthBar : MonoBehaviour
{
    private Entity entity;
    private RectTransform rectTransform;
    private Slider slider;
    private CharacterStats mystats;

    private void Start()
    {
        entity = GetComponentInParent<Entity>();
        rectTransform=GetComponent<RectTransform>();    
        slider=GetComponentInChildren<Slider>();
        mystats=GetComponentInParent<CharacterStats>();

        entity.OnFlipped += FlipUI;
        mystats.OnHealthChanged += UpdateHealthUI;

        UpdateHealthUI();
    }

    private void Update()
    {
        UpdateHealthUI();
    }


    private void FlipUI()
    {
        rectTransform.Rotate(0, 180, 0);
    }



    private void UpdateHealthUI()
    {
        slider.maxValue = mystats.GetMaxHealthValue();
        slider.value =mystats.currentHealth;
    }


    private void OnDisable()
    {
        entity.OnFlipped -= FlipUI;
        mystats.OnHealthChanged -= UpdateHealthUI;
    }
}
