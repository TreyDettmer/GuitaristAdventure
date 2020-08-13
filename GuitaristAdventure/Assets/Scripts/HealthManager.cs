using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public int maxHealth = 100;
    protected int currentHealth;

    public event Action<float> OnHealthPercentChanged = delegate { };
    public GameObject healthBarObject;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    public void ChangeHealthDisplay()
    {
        float currentHealthPercent = (float)currentHealth / (float)maxHealth;
        OnHealthPercentChanged(currentHealthPercent);
    }

    public virtual void TakeDamage(int damage)
    {
    }

    protected virtual void Die()
    {


    }

    // function copied from https://forum.unity.com/threads/change-gameobject-layer-at-run-time-wont-apply-to-child.10091/
    protected void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (null == obj)
        {
            return;
        }

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (null == child)
            {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }


}
