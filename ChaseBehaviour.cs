using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseBehaviour : StateMachineBehaviour
{
    public float PlayerDistance;
    public float Speed = 2;

    private Transform _player;
    private float DirectedSpeed;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Check triggers
        var playerClose = IsPlayerClose(animator.transform);
        animator.SetBool("IsChasing", playerClose);

        //Move to player
        if (animator.transform.position.x >= _player.position.x)
        {
            DirectedSpeed = Speed * -1;
        }
        else
        {
            DirectedSpeed = Speed;
        }
        ((Rigidbody2D)animator.GetComponentInParent<Rigidbody2D>()).velocity = new Vector2(
            DirectedSpeed,
            ((Rigidbody2D)animator.GetComponentInParent<Rigidbody2D>()).velocity.y);     
    }

    private bool IsPlayerClose(Transform transform)
    {
        var dist = Vector3.Distance(transform.position, _player.position);
        return dist < PlayerDistance;
    }

    
}
