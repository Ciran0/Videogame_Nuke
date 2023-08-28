using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controls : MonoBehaviour
{

    public float walkSpeed = 0f;
    public float dashMaxSpeed = 0f;
    public float dashMinSpeed = 0f;
    public float dashLenght = 0f;
    public float dashRecovery = 0f;
    public int dashMaxNumber = 0;
    public float dashChainDelay = 0f;

    int remainingDashes = 0;
    float dashRecoveryTimer = 0f;
    bool dashing = false;
    bool dashChain = false;
    Vector3 mouseWorldPos;
    float dashingLenght = 0f;
    Vector3 dashDirection;
    Rigidbody2D body;

    void Start() {
        body = gameObject.GetComponent<Rigidbody2D>();
    }

    void OnDrawGizmos() 
    {
        Debug.DrawLine(transform.position, mouseWorldPos, Color.magenta);
        Debug.DrawRay(transform.position, dashDirection * -dashLenght, Color.green);
    }

    void Update()
    {
        //get mouse poss in world
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);

        //walk movement
        if (!dashing) {
            Vector3 tmp = new Vector3(Input.GetAxis("Horizontal") * walkSpeed * Time.deltaTime, Input.GetAxis("Vertical") * walkSpeed  * Time.deltaTime, 0f);
            body.MovePosition(transform.position + tmp);
        }
    
        //dash motion
        if (dashing) {
            float curentSpeed = (Mathf.Pow(Mathf.Cos(dashingLenght*Mathf.PI/2*dashLenght),2)*(dashMaxSpeed-dashMinSpeed))+dashMinSpeed;
            Vector3 motion = new Vector3(-curentSpeed * Time.deltaTime * dashDirection.x, -curentSpeed * Time.deltaTime * dashDirection.y, 0f);
            body.MovePosition(transform.position + motion);
            dashingLenght += motion.magnitude;
        } else {
            Vector3 mouseOffset = transform.position - mouseWorldPos;
            dashDirection = new Vector3(mouseOffset.x, mouseOffset.y, 0f).normalized;
        }

        //dash activation
        if (Input.GetButtonDown("Jump") && remainingDashes > 0){
            if (!dashing) {
                dashing = true;
                remainingDashes -= 1;
            } else if ((dashLenght-dashingLenght)/((dashMinSpeed+dashMaxSpeed)/2) < dashChainDelay) {
                dashChain = true;
            }
        }

        //dash stop
        if (dashing && dashingLenght >= dashLenght) {
            dashing = false;
            dashingLenght = 0f;

            if (dashChain) {
                dashing = true;
                dashChain = false;
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
