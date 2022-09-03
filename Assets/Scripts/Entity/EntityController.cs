using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityController : MonoBehaviour
{
    Animator m_Animator;
    bool m_IsWalking = false;

    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = Vector3.zero;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            dir += Vector3.left;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            dir += Vector3.right;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            dir += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            dir += Vector3.back;
        }

        if (dir.sqrMagnitude > 0f)
        {
            transform.rotation = Quaternion.LookRotation(Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0) * dir);
            transform.Translate(Vector3.forward * 2.0f * Time.deltaTime);

            if (m_IsWalking == false)
            {
                m_Animator.Play("Walk");
            }
            m_IsWalking = true;
        }
        else
        {
            if (m_IsWalking == true)
            {
                m_Animator.Play("Idle");
            }
            m_IsWalking = false;
        }
    }
}
