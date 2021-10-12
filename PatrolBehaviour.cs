using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolBehaviour : StateMachineBehaviour
{

    public float StayTime;
    public float PlayerDistance;

    private float _timer;
    private Transform _player;
    private Vector2 _target;
    private Vector2 _startPos;
    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _timer = 0;
        _startPos = new Vector2(animator.transform.position.x, animator.transform.position.y);
        _target = new Vector2(_startPos.x + Random.Range(-1f, 1f), _startPos.y + Random.Range(-1f, 1f));

        // Move
        ((Patrol)animator.GetComponentInParent<Patrol>()).enabled = true;
    }

   // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Check triggers
        var playerClose = IsPlayerClose(animator.transform);
        var timeUp = IsTimeUp();

        animator.SetBool("IsChasing", playerClose);
        animator.SetBool("IsPatroling", !timeUp);
    }

    private bool IsTimeUp()
    {
        _timer += Time.deltaTime;
        return _timer > StayTime;
    }

    private bool IsPlayerClose(Transform transform)
    {
        var dist = Vector3.Distance(transform.position, _player.position);
        return dist < PlayerDistance;
    }


}
