using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepBool : StateMachineBehaviour {

    public string boolName;
    public bool status;
    public bool resetOnExit = true;

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
        animator.SetBool(boolName, status);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
        if(resetOnExit)
            animator.SetBool(boolName, !status);
    }

}
