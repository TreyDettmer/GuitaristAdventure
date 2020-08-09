using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBullet : MonoBehaviour
{
    float startTime;
    [SerializeField] float lifeTime = 3f;
    public int damage = 34;
    bool bActive = true;
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - startTime >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.tag == "Player")
        {

            if (bActive)
            {
                bool bSuccessfulHit = true;
                //Check if the player is shielding
                if (collision.gameObject.GetComponent<PlayerCombat>().currentState == PlayerCombat.PlayerCombatState.Shielding)
                {
                    //Check if the player is facing the bullet
                    if (Vector3.Angle(-collision.gameObject.transform.forward, GetComponent<Rigidbody>().velocity) < 100)
                    {
                        bSuccessfulHit = false;
                    }
                }
                if (bSuccessfulHit)
                {
                    collision.gameObject.GetComponent<HealthManager>().TakeDamage(damage);
                    
                }
                bActive = false;
            }
        }
        Destroy(gameObject);
    }
}
