using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SceneTriggerTemplate<T> : MonoBehaviour where T : Component
{
    [SerializeField] string[] m_CheckTags;
    [SerializeField] bool m_CheckStay = true;

    List<T> m_ContainsTargetList = new List<T>();

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (m_CheckTags != null && m_CheckTags.Length > 0)
        {
            if (System.Array.Exists(m_CheckTags, tag => other.CompareTag(tag)) == false)
                return;
        }

        T component = other.GetComponent<T>();
        if (component == null) return;

        if (m_ContainsTargetList.Contains(component) == false)
        {
            m_ContainsTargetList.Add(component);
            OnEnterTarget(component);
        }
    }
    
    protected virtual void OnTriggerStay(Collider other)
    {
        if (m_CheckStay == false)
            return;
        
        OnTriggerEnter(other);
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (m_CheckTags != null && m_CheckTags.Length > 0)
        {
            if (System.Array.Exists(m_CheckTags, tag => other.CompareTag(tag)) == false)
                return;
        }

        T component = other.GetComponent<T>();
        if (component == null) return;

        if (m_ContainsTargetList.Remove(component) == true)
        {
            OnExitTarget(component);
        }
    }


    protected virtual void OnEnterTarget(T entity)
    {

    }

    protected virtual void OnExitTarget(T entity)
    {

    }
}
