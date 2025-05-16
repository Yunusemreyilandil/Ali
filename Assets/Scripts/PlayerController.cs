using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float walkSpeed = 2f;
    public float runSpeed = 5f; 
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal,0,vertical).normalized;

        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        transform.Translate(direction* speed*Time.deltaTime, Space.World);


        if (Input.GetMouseButton(0))
        {
            animator.SetBool("IsAttacking", true);
        }
        else
        {
            animator.SetBool("IsAttacking", false);
        }

        if (Input.GetMouseButton(1))
        {
        }
    }
}
