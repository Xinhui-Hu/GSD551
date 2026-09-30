using System;
using UnityEngine;
 
public class Collectible : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;
    
    [SerializeField] 
    private bool isBadCat = false;
    
    [SerializeField] 
    private float launchForce = 10f;//push player away
    
    private Vector3 startPosition;
    private CatFollower catFollower;
    private Collider2D catCollider;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
        catCollider = GetComponent<Collider2D>();

        if (!isBadCat)
        {
            catFollower = GetComponent<CatFollower>();
        }

        gameManager.RegisterCollectable(this);
    }

    // Update is called once per frame
    void Update()
    {
        
         
    }
    
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            if (isBadCat)
            {
                Vector2 launchDir = (col.transform.position - transform.position).normalized;
                launchDir.y = 0.3f;
                launchDir.x = launchDir.x >= 0 ? 1f : -1f;
                launchDir = launchDir.normalized;

                PlayerMovement player = col.GetComponent<PlayerMovement>();
                player.ApplyKnockback(launchDir * launchForce);
                player.ApplyJumpBoost();  // ← 加这一行
                gameObject.SetActive(false);
            }
            else
            {
                // Good Cat: Add score, follow the player
                gameManager.CollectItem();
                catCollider.enabled = false;
                catFollower.StartFollowing(col.transform);
            }
        }
    }

    /*
    void OnTriggerEnter2D(Collider2D col)
    {
        //
        if (col.CompareTag( "Player"))
        {
            Debug.Log(col.gameObject.name + ":" + gameObject.name + ":" + Time.time);
            gameManager.CollectItem();
            gameObject.SetActive(false); 
        }
    }*/
    
    public void ResetCat()
    {
        gameObject.SetActive(true);
        transform.position = startPosition;
        catCollider.enabled = true;

        if (catFollower != null)
        {
            catFollower.StopFollowing();
        }
    }
    
    
    
    
    
}
