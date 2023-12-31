using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public enum EffectStatus
{
    None, Invincible
}

public class BaseHealth : MonoBehaviour
{
    public float maxHealth = 100;
    [Range(0, 1)] public float defense = 0;
    public float currentHealth;
    public bool canDestroyBullet = true;

    [Header("Component")] 
    public SpriteRenderer effectSpriteRenderer;
    
    [Header("Setting")] 
    public EffectStatus status;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }
    
    public virtual void TakeDamage(float damage)
    {
        switch (status)
        {
            case EffectStatus.Invincible:
                damage = 0;
                break;
        }
        currentHealth -= damage * (1 - defense);
       // Debug.Log("d");
        if (currentHealth <= 0)
            Die();
    }

    public IEnumerator GiveEffect(EffectStatus targetEffect, float effectEndTime)
    {
        status = targetEffect;
        EffectAction(targetEffect, effectEndTime);
        yield return new WaitForSeconds(effectEndTime);
        status = EffectStatus.None;
    }

    void EffectAction(EffectStatus targetEffect, float effectEndTime)
    {
        switch (targetEffect)
        {
           case EffectStatus.Invincible:
               
               for (int i = 0; i < effectEndTime * 2; i++)
               {
                   
                   Sequence sequence = DOTween.Sequence();
                   sequence.Append(effectSpriteRenderer.DOFade(0, effectEndTime * 0.3f));
                   sequence.Append(effectSpriteRenderer.DOFade(1, effectEndTime * 0.3f));
               }

               effectSpriteRenderer.DOFade(1, 0);
               break;
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
