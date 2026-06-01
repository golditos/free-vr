using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Sword : MonoBehaviour
{
    /*
    private XRGrabInteractable grabInteractable;
    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable == null) return;
        
    }   */
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger with " + other.name);
        
           EnemyAI enemy = other.gameObject.GetComponentInParent<EnemyAI>();
           if (enemy)
           {
               enemy.TakeDamage(1);
           };
        
    }
}
