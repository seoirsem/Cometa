using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueFlameFunction : MonoBehaviour
{
    // Start is called before the first frame update
    Animator animator;

    float timeStart;
    float timeOff = 10f;


    public bool start = true;
    public bool exit = false;



    void Start()
    {
        animator = GetComponent<Animator>();
        timeStart = Time.time;
        StartJet();
        this.gameObject.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {

//        Debug.Log(animator.GetCurrentAnimatorStateInfo(0));   
    }



    public void TransitionToFullJet()
    {
        Debug.Log("fullFlame");
    }

    public void StopJet()
    {
        //animator.ResetTrigger("start");
        //animator.SetTrigger("exit");
        exit = true;
        start = false;
        animator.SetBool("start",false);
        animator.SetBool("exit",true);
    }

    public void StartJet()
    {
//        animator.ResetTrigger("exit");
  //      animator.SetTrigger("start");
        start = true;
        exit = false;
        //animator.SetBool("start",true);
        //animator.SetBool("exit",false);
    }

    public void DestroyAnimationGO()
    {
        Destroy(this.gameObject);
    }


}
