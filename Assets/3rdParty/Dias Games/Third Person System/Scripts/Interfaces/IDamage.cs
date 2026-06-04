using UnityEngine;

public interface IDamage
{
    void Damage(int damagePoints);
    
    void TakeStun(float duration);
    
    // mìn nổ / búa đập
    void TakeKnockback(Vector3 direction, float force);
}
