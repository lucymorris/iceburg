using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabyFollow : MonoBehaviour
{

    public Transform target;
    private UnityEngine.AI.NavMeshAgent agent;
    private Animator anim;
    private Rigidbody myRigidbody;

    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        myRigidbody = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();

        agent.updatePosition = false;
        agent.updateRotation = false;
    }

    void Update()
    {
        //agent.velocity = myRigidbody.velocity;
        agent.nextPosition = myRigidbody.position;
        agent.SetDestination(target.position);
    }

    void FixedUpdate()
    {  
        // move using physics (so collisions apply correctly)
        myRigidbody.AddForce(agent.velocity - myRigidbody.velocity, ForceMode.VelocityChange);
        
        if(Vector3.Distance(target.position, this.transform.position) < 0.5)
        {
            Vector3 direction = target.position - this.transform.position;
            direction.y = 0;

            // rotate using physics (so collisions apply correctly)
            myRigidbody.MoveRotation(Quaternion.Slerp(this.transform.rotation,
                                    Quaternion.LookRotation(direction), 0.1f));

            //this.transform.Translate(0, 0, 0.05f);
        }
        else if (myRigidbody.velocity.magnitude > 0.01f)
        {
            // rotate using physics (so collisions apply correctly)
            myRigidbody.MoveRotation(Quaternion.Slerp(this.transform.rotation,
                                    Quaternion.LookRotation(myRigidbody.velocity), 0.1f));
        }

        if (agent.desiredVelocity.magnitude > 0.01f)
        {
            anim.SetBool("IsWalking", true);
        }
        else
        {
            anim.SetBool("IsWalking", false);
        }
    }

}
