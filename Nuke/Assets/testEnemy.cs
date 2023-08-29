using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testEnemy : MonoBehaviour
{
    public float speed = 0f;
    Transform player;
    Rigidbody2D body;

    void Start()
    {
        player = GameObject.Find("Player").transform;
        body = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector3 playerDirection = (transform.position - player.position).normalized;
        Debug.DrawLine(transform.position, player.position, Color.red);
        body.MovePosition(transform.position + new Vector3(-speed * playerDirection.x * Time.deltaTime, -speed * playerDirection.y * Time.deltaTime,0f));
    }
}
