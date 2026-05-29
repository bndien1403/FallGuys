using UnityEngine;

public class ItemBase : MonoBehaviour
{
    public ItemDataSO data;
    public virtual void OnEquid() { }
    public virtual  void OnUnequid() { }
    public virtual void OnUse() { }
    public virtual void OnUseEnd() {}
    public virtual void OnUsing() {}
    


}
 