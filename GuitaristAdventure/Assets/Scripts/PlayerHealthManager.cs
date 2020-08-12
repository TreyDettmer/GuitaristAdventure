using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthManager : HealthManager
{
    public List<GameObject> livesGUI = new List<GameObject>();
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        currentHealth -= damage;
        if (livesGUI.Count > 0)
        {
            Image im = livesGUI[livesGUI.Count - 1].GetComponent<Image>();
            im.enabled = false;
            livesGUI.RemoveAt(livesGUI.Count - 1);
        }


        //TODO: play animation

        if (currentHealth <= 0)
        {
            //die
            Die();

        }
    }

    protected override void Die()
    {
        base.Die();

        MonoBehaviour[] comps = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour c in comps)
        {
            c.enabled = false;
        }
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().isKinematic = true;
                

        
    }
}

