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
    public float teleportDistance = 1500f;
    public float turnSmoothTime = 0.1f;

    bool isSprinting;
    int sprintCount;
    Vector3 moveDir;

    public AudioSource tpSound;
    public ParticleSystem tpCircleStart;
    public ParticleSystem tpCircleEnd;
    bool isTeleporting;
    public GameObject trail;

    //ground check
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    bool isGrounded;

    //gravitational velocity
    Vector3 gVelocity;
    //gravity
    public float gravity = -9.81f;

    float turnSmoothVelocity;
    private void Start()
    {
        isSprinting = false;
        sprintCount = 0;
        isTeleporting = false;
        trail.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && gVelocity.y < 0)
        {
            gVelocity.y = -2f;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (sprintCount % 2 == 0) isSprinting = true;
            else isSprinting = false;
            sprintCount++;
        }
        if ((Input.GetKeyDown(KeyCode.LeftShift)) && (isTeleporting == false))
        {
            startTeleport();
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
    void startTeleport()
    {
        isTeleporting = true;
        player.SetActive(false);
        tpSound.Play();
        StartCoroutine(teleportSequence());
        Invoke(nameof(Teleport), .25f);
        trail.SetActive(true);
    }
    IEnumerator teleportSequence()
    {
        ParticleSystem circle = Instantiate(tpCircleStart, player.transform.position, cam.transform.rotation);
        circle.Play();
        yield return new WaitForSeconds(1f);
        Destroy(circle.gameObject);
    }
    void Teleport()
    {
        controller.Move(moveDir * teleportDistance * Time.deltaTime);
        tpCircleEnd.Play();
        Invoke(nameof(endTeleport), .25f);
    }
    void endTeleport()
    {
        player.SetActive(true);
        isTeleporting = false;
        trail.SetActive(false);
    }
}
