using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pooling : MonoBehaviour
{
   private List<GameObject> pool;

   public Pooling(int capacity, GameObject prefab)
   {
      pool = new List<GameObject>(capacity);

      for (int i = 0; i < capacity; i++)
      {
         GameObject obj = Object.Instantiate(prefab);
         obj.gameObject.SetActive(false);
         pool.Add(obj);
      }
   }

   public GameObject ActiveObject()
   {
      foreach (GameObject obj in pool)
      {
         if (!obj.gameObject.activeInHierarchy)
         {
            obj.gameObject.SetActive(true);
            DisableCollisionForDuration(obj);
            return obj;
         }
      }
      return null;
   }
   
   public void DesactiveObject(GameObject obj)
   {
      obj.gameObject.SetActive(false);
   }
   
   
   private IEnumerable<WaitForSeconds> DisableCollisionForDuration(GameObject obj)
   {
      gameObject.GetComponent<Rigidbody>().isKinematic = true;

      yield return new WaitForSeconds(5f);

   
      gameObject.GetComponent<Rigidbody>().isKinematic = false;
   }
}
