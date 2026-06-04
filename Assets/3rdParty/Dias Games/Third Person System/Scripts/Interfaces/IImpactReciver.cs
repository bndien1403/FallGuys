using UnityEngine;

public interface IImpactReciver
{
    void TakeKnockback(Vector3 direction, float force);

    void TakeStun(float duration);
}
