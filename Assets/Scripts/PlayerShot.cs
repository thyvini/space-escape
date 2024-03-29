﻿using System.Collections;
using SpaceEscape.Audio;
using SpaceEscape.EventSystem;
using SpaceEscape.ScriptableObjectVariables;
using UnityEngine;

namespace SpaceEscape
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(AudioClip))]
    public class PlayerShot : MonoBehaviour
    {
        [SerializeField] private AudioClip audioClip;

        [SerializeField] private GameEvent onShoot;
        [SerializeField] private GameEvent onEnemyDie;
        [SerializeField] private Variable<Vector3> lastDeadEnemyPosition;


        private IEnumerator _coroutine;

        private void Start()
        {
            _coroutine = Expire();
            StartCoroutine(_coroutine);
            onShoot.Raise();
            AudioManager.Instance.PlaySound(audioClip, transform.position);
        }

        private void Update()
        {
            if (Camera.main == null)
            {
                StopCoroutine(_coroutine);
                Destroy(gameObject);
                return;
            }
            Vector2 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
            var onScreen = screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
            
            if (onScreen) return;
            
            StopCoroutine(_coroutine);
            Destroy(gameObject);
        }

        private IEnumerator Expire() {
            yield return new WaitForSeconds(3);
            Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Enemy")) return;
            
            StopCoroutine(_coroutine);
            
            lastDeadEnemyPosition.SetValue(transform.position);
            
            Destroy(gameObject);
            Destroy(other.gameObject);
            
            onEnemyDie.Raise();
        }
    }
}