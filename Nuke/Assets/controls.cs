using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controls : MonoBehaviour
{

    public float walkSpeed = 2f;
    public float dashSpeed = 4f;
    public float dashLenght = 4f;
    public float dashRecovery = 1f;
    public int dashMaxNumber = 4;

    int remainingDashes = 0;
    float dashRecoveryTimer = 0;
    private float dashingTime = 0;
    private Vector3 dashDirection;
    // Start is called before the first frame update
    void Start()
    {
        remainingDashes = dashMaxNumber;
    }

    // Update is called once per frame
    void Update()
    {
        //get mouse poss in world
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
        Debug.DrawLine(transform.position, mouseWorldPos, Color.magenta);//aim debug line

        //walk movement
        if (dashingTime <= 0f) {
            transform.position += new Vector3(Input.GetAxis("Horizontal") * walkSpeed * Time.deltaTime, Input.GetAxis("Vertical") * walkSpeed  * Time.deltaTime, 0f) ;
        }
    
        //dash
        if (dashingTime > 0f) {
            dashingTime -= Time.deltaTime;
            transform.position += new Vector3(-dashSpeed * Time.deltaTime * dashDirection.x, -dashSpeed * Time.deltaTime * dashDirection.y, 0f);
        } else {
            Vector3 mouseOffset = transform.position - mouseWorldPos;
            dashDirection = new Vector3(mouseOffset.x, mouseOffset.y, 0f).normalized;
            Debug.DrawRay(transform.position, dashDirection * -dashLenght, Color.green);//dash preview
            if (Input.GetButtonDown("Jump") && remainingDashes > 0){
                dashingTime = dashLenght/dashSpeed;
                remainingDashes -= 1;
            }
        }

        //dash recovery
        if (remainingDashes < dashMaxNumber){
            if (dashRecoveryTimer >= dashRecovery) {
                remainingDashes += 1;
                dashRecoveryTimer = 0f;
            }
            dashRecoveryTimer += Time.deltaTime;
        }
    }
}
