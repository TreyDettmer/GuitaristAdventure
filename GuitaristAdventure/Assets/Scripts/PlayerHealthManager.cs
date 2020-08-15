using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

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
        MonsterController[] monsters = FindObjectsOfType<MonsterController>();
        foreach (MonsterController monster in monsters)
        {
            NavMeshAgent monsterAgent = monster.gameObject.GetComponent<NavMeshAgent>();
            monsterAgent.enabled = false;

            MonoBehaviour[] monsterComps = monster.gameObject.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour comp in monsterComps)
            {
                comp.enabled = false;
            }

        }
        SmasherController[] smashers = FindObjectsOfType<SmasherController>();
        foreach (SmasherController smasher in smashers)
        {
            NavMeshAgent smasherAgent = smasher.gameObject.GetComponent<NavMeshAgent>();
            smasherAgent.enabled = false;

            MonoBehaviour[] smasherComps = smasher.gameObject.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour comp in smasherComps)
            {
                comp.enabled = false;
            }

        }
        GetComponent<PlayerController>().TurnOnRagdoll();
        MonoBehaviour[] comps = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour c in comps)
        {
            c.enabled = false;
        }
        GetComponent<Rigidbody>().velocity = Vector3.zero;


        PlayerManager.RestartScene();



    }
}

