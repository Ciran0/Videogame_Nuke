using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controls : MonoBehaviour
{

    [Tooltip("default movement speed [m/s]")]
    public float walkSpeed = 0f;
    [Tooltip("initial speed of dash [m/s]")]
    public float dashMaxSpeed = 0f;
    [Tooltip("end speed of dash [m/s]")]
    public float dashMinSpeed = 0f;
    [Tooltip("lenght of dash [m]")]
    public float dashLenght = 0f;
    [Tooltip("dash counter cooldown [s]")]
    public float dashRecovery = 0f;
    [Tooltip("max number of consequitive dashes")]
    public int dashMaxNumber = 0;
    [Tooltip("delay for dash preshot [m]")]
    public float dashChainDelay = 0f;
    [Tooltip("activation window for quickstep [s]")]
    public float quickstepDelay = 0f;
    [Tooltip("initial speed of quickstep [m/s]")]
    public float quickStepMaxSpeed = 0f;
    [Tooltip("end quickstep speed [m/s]")]
    public float quickStepMinSpeed = 0f;
    [Tooltip("lenght of quickstep [m]")]
    public float quickstepLenght = 0f;
    [Tooltip("quickstep recovery time [s]")]
    public float quickstepCooldown = 0f;

    int remainingDashes = 0;
    float dashRecoveryTimer = 0f;
    bool dashing = false;
    bool quickstepping = false;
    bool dashChain = false;
    Vector3 mouseWorldPos;
    float dashingLenght = 0f;
    float quicksteppingLenght = 0f;
    Vector2 dashDirection;
    Rigidbody2D body;
    Vector2 quickstepDirection;
    float quickstepUpTimer = 0f;
    float quickstepDownTimer = 0f;
    float quickstepLeftTimer = 0f;
    float quickstepRightTimer = 0f;
    float quickstepCooldownTimer = 0f;

    void Start() {
        body = gameObject.GetComponent<Rigidbody2D>();
    }

    void OnDrawGizmos() 
    {
        if (dashing || remainingDashes <= 0) {
            Gizmos.color = Color.magenta;
        } else {

            Gizmos.color = Color.green;
        }
        Gizmos.DrawRay(transform.position, dashDirection * -dashLenght);
        Gizmos.color = Color.magenta;
        if (quickstepping || quickstepCooldownTimer > 0) {
            Gizmos.color = Color.magenta;
        } else {
            Gizmos.color = Color.blue;
        }
        Gizmos.DrawWireSphere(transform.position, quickstepLenght);
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        //get mouse poss in world
        Vector2 mousePos = Input.mousePosition;
        mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);

        //walk movement
        if (!dashing && !quickstepping) {
            Vector2 tmp = new Vector2(moveX, moveY).normalized;
            body.velocity = tmp * walkSpeed;
        }
    
        //dash motion
        if (dashing && !quickstepping) {
            float curentSpeed = (Mathf.Pow(Mathf.Cos(dashingLenght*Mathf.PI/2*dashLenght),2)*(dashMaxSpeed-dashMinSpeed))+dashMinSpeed;
            body.velocity = -dashDirection*curentSpeed;
            dashingLenght += (-dashDirection*curentSpeed).magnitude * Time.deltaTime;
        } else {
            Vector2 mouseOffset = transform.position - mouseWorldPos;
            dashDirection = new Vector2(mouseOffset.x, mouseOffset.y).normalized;
        }

        //dash activation
        if (Input.GetButtonDown("Jump") && remainingDashes > 0 && !quickstepping){
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

        //quickstep start
        if (!quickstepping && quickstepCooldownTimer <= 0) {
            if (moveY > 0) {
                if (quickstepUpTimer < quickstepDelay && quickstepUpTimer > 0) {
                    quickstepDirection = new Vector2(0, 1);
                    quickstepping = true;
                }
                quickstepUpTimer = quickstepDelay;
            } else if (quickstepUpTimer > 0) {
                quickstepUpTimer -= Time.deltaTime;
            }

            if (moveY < 0) {
                if (quickstepDownTimer < quickstepDelay && quickstepDownTimer > 0) {
                    quickstepDirection = new Vector2(0, -1);
                    quickstepping = true;
                }
                quickstepDownTimer = quickstepDelay;
            } else if (quickstepDownTimer > 0) {
                quickstepDownTimer -= Time.deltaTime;
            }

            if (moveX > 0) {
                if (quickstepLeftTimer < quickstepDelay && quickstepLeftTimer > 0) {
                    quickstepDirection = new Vector2(1, 0);
                    quickstepping = true;
                }
                quickstepLeftTimer = quickstepDelay;
            } else if (quickstepLeftTimer > 0) {
                quickstepLeftTimer -= Time.deltaTime;
            }

            if (moveX < 0) {
                if (quickstepRightTimer < quickstepDelay && quickstepRightTimer > 0) {
                    quickstepDirection = new Vector2(-1, 0);
                    quickstepping = true;
                }
                quickstepRightTimer = quickstepDelay;
            } else if (quickstepRightTimer > 0) {
                quickstepRightTimer -= Time.deltaTime;
            }
        }
        

        //quickstep move
        if (quickstepping) {
            float curentSpeed = (Mathf.Pow(Mathf.Cos(quicksteppingLenght*Mathf.PI/2*quickstepLenght),2)*(quickStepMaxSpeed-quickStepMinSpeed))+quickStepMinSpeed;
            body.velocity = quickstepDirection*curentSpeed;
            quicksteppingLenght += (quickstepDirection*curentSpeed).magnitude * Time.deltaTime;
            Debug.Log(quicksteppingLenght);
        }

        //quickstep stop
        if (quickstepping && quicksteppingLenght >= quickstepLenght) {
            quicksteppingLenght = 0;
            quickstepping = false;
            quickstepUpTimer = 0;
            quickstepDownTimer = 0;
            quickstepLeftTimer = 0;
            quickstepRightTimer = 0;
            quickstepCooldownTimer = quickstepCooldown;
        }

        //quickstep recovery
        if (quickstepCooldownTimer > 0) {
            quickstepCooldownTimer -= Time.deltaTime;
        }
    }
}
