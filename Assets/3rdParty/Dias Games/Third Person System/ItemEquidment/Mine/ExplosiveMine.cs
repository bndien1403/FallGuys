using System;
using UnityEngine;

public class ExplosiveMine : MonoBehaviour
{
   [Header("Explosion Setting")] [SerializeField]
   private float explosionRadius = 4f;

   [SerializeField] private float explosionForce = 15f; // luc hat vang
   [SerializeField] private float stunDuration = 2.5f; // thoi gian choaong
   [SerializeField] private float upwardModifier = 1.2f;
   [Header("Effect")] private ParticleSystem explosionVFX;

   private bool hasExplosded;

   private void OnTriggerEnter(Collider other)
   {
      if (hasExplosded) return;
      if (other.CompareTag("Player"))
      {
         ExPlose();
      }
   }

   private void ExPlose()
   {
   hasExplosded = true;
   if (explosionVFX != null)
   {
      Instantiate(explosionVFX , transform.position, Quaternion.identity);
      
   }

   Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
   foreach (Collider hit in colliders)
   {
      if (hit.TryGetComponent(out IImpactReciver reciver))
      {
         Vector3 pushDirection = (hit.transform.position-transform.position).normalized;
         pushDirection.y += upwardModifier;
         
         reciver.TakeKnockback(pushDirection.normalized , explosionForce);
         reciver.TakeStun(stunDuration);
      }
   }
   Destroy(gameObject);
   }
   private void OnDrawGizmosSelected()
   {
      Gizmos.color = Color.red;
      Gizmos.DrawWireSphere(transform.position, explosionRadius);
   }
}
