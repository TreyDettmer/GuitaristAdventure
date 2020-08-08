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
                Debug.Log("hit player");
                collision.gameObject.GetComponent<HealthManager>().TakeDamage(damage);
                bActive = false;
            }
        }
        Destroy(gameObject);
    }
}
