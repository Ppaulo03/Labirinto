using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Exit : MonoBehaviour
{
    [SerializeField] private UnityEvent signalEvent = null;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player") signalEvent.Invoke();
    }
}
