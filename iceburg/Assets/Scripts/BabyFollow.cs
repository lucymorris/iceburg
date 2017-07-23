using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BabyFollow : MonoBehaviour
{

    public Transform target;
    private UnityEngine.AI.NavMeshAgent agent;
    static Animator anim;

    void Start()
    {

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        anim = GetComponent<Animator>();

      }


    void Update()
    {

        agent.SetDestination(target.position);
        
        if(Vector3.Distance(target.position, this.transform.position) > 2)
        {
            Vector3 direction = target.position - this.transform.position;
            direction.y = 0;

            this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
                                        Quaternion.LookRotation(direction), 0.1f);

            //anim.SetBool("IsWalking", false);
            if(direction.magnitude > 2)
            {
                this.transform.Translate(0, 0, 0.05f);
                //anim.SetBool("IsWalking", true);
            }
        }

        if (agent.velocity.magnitude > 0.01f)
        {
            anim.SetBool("IsWalking", true);
        }
        else
        {
            anim.SetBool("IsWalking", false);
        }
    }

}
