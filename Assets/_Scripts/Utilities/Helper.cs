using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper : MonoBehaviour {

    [Range(-1, 1)] public float vertical;
    [Range(-1, 1)] public float horizontal;

    public bool playAnim;
    public bool enableRootMotion;
    public bool twoHanded;
    public bool useItem;
    public bool interacting;
    public bool lockon;

    public string[] oh_attacks;
    public string[] th_attacks;

    private Animator anim;

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
	}

    private void Update() {
        
        enableRootMotion = !anim.GetBool("canMove");

        anim.applyRootMotion = enableRootMotion;

        interacting = anim.GetBool("interacting");

        if (lockon == false) {
            horizontal = 0;
            vertical = Mathf.Clamp01(vertical);
        }

        anim.SetBool("lockOn", lockon);


        if (enableRootMotion) {
            return;
        }

        if (useItem == true) {
            anim.Play("use_item");
            useItem = false;
        }
        if (interacting) {
            playAnim = false;
            vertical = Mathf.Clamp(vertical, 0, 0.5f);
        }

        anim.SetBool("two_handed", twoHanded);

        if (playAnim) {
            string targetAnim;

            if (!twoHanded) {
                int r = Random.Range(0, oh_attacks.Length);
                targetAnim = oh_attacks[r];
                if (vertical > 0.5f) {
                    targetAnim = "oh_attack_3";
                }
            }
            else {
                int r = Random.Range(0, th_attacks.Length);
                targetAnim = th_attacks[r];
                if (vertical > 0.5f) {
                    targetAnim = "th_attack_2";
                }
            }
            vertical = 0;
            anim.CrossFade(targetAnim, .2f);
            //anim.SetBool("canMove", false);
            //enableRootMotion = true;
            playAnim = false;
        }

        anim.SetFloat("vertical", vertical);
        anim.SetFloat("horizontal", horizontal);

        }
}
