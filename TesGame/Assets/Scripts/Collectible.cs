using System;
using UnityEngine;
 
public class Collectible : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager.RegisterCollectable(this);
    }

    // Update is called once per frame
    void Update()
    {
        
         
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //
        if (col.CompareTag( "Player"))
        {
            Debug.Log(col.gameObject.name + ":" + gameObject.name + ":" + Time.time);
            gameManager.CollectItem();
            gameObject.SetActive(false); 
        }
    }
}
