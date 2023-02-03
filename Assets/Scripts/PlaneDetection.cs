using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARPlaneManager))]
public class PlaneDetection : MonoBehaviour
{
   private bool m_PlaneVisible = false;
   // Start is called before the first frame update
   void Start()
   {
       
   }

   // Update is called once per frame
   void Update()
   {
       foreach (var plane in m_ARPlaneManager.trackables)
       {
           plane.gameObject.SetActive(m_PlaneVisible);
       }
   }

   public void SetAllPlanesActive()
   {
       m_PlaneVisible = !m_PlaneVisible;
   }

   void Awake()
   {
       m_ARPlaneManager = GetComponent<ARPlaneManager>();
   }

   ARPlaneManager m_ARPlaneManager;
}