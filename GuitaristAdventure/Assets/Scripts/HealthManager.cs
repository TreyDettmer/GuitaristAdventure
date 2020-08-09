using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public int maxHealth = 100;
    int currentHealth;
    public List<GameObject> livesGUI = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        switch (tag)
        {
            case "Player":
                if (livesGUI.Count > 0)
                {
                    Image im = livesGUI[livesGUI.Count - 1].GetComponent<Image>();
                    im.enabled = false;
                    livesGUI.RemoveAt(livesGUI.Count - 1);
                }
                break;
        }

        //TODO: play animation

        if (currentHealth <= 0)
        {
            //die
            Die();
            
        }
    }

    void Die()
    {
        switch (tag)
        {
            case "Player":
                MonoBehaviour[] comps = GetComponents<MonoBehaviour>();
                foreach (MonoBehaviour c in comps)
                {
                    c.enabled = false;
                }
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                GetComponent<Rigidbody>().isKinematic = true;
                break;
            case "Monster":
                MonsterController monsterController = gameObject.GetComponentInParent<MonsterController>();
                monsterController.TurnOnRagdoll();
                Destroy(gameObject,3f);
                break;
        }

    }

    
}
