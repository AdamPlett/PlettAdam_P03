using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public GameObject player;
    public Transform cam;
    
    public float speed = 6f;
    public float sprintSpeed = 10f;
    public float dashDistance = 1500f;
    public float turnSmoothTime = 0.1f;

    bool isSprinting;
    int sprintCount;
    Vector3 moveDir;

    //gravitational velocity
    Vector3 gVelocity;
    //gravity
    public float gravity = -9.81f;

    float turnSmoothVelocity;
    private void Start()
    {
        isSprinting = false;
        sprintCount = 0;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (sprintCount % 2 == 0) isSprinting = true;
            else isSprinting = false;
            sprintCount++;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            teleport();
        }
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            if (isSprinting == true) controller.Move(moveDir.normalized * sprintSpeed * Time.deltaTime);
            else controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }
        gVelocity.y += gravity * Time.deltaTime;
        controller.Move(gVelocity * Time.deltaTime);
    }
    void teleport()
    {
        controller.Move( moveDir * dashDistance * Time.deltaTime);
    }
}
