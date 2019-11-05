//***********************************************************************
// Copyright 2014 VirZOOM
//***********************************************************************

using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class VZBikeAnim : MonoBehaviour
{
   Animator mAnim;

   void Start()
   {
      // Lookup anim objects
      mAnim = GetComponent<Animator>();
   }

   void Update()
   {
      var controller = VZPlayer.Controller;

      mAnim.SetBool("ButtonA", controller.RightDown.Down);
      mAnim.SetBool("ButtonB", controller.RightRight.Down);
      mAnim.SetBool("ButtonX", controller.RightLeft.Down);
      mAnim.SetBool("ButtonY", controller.RightUp.Down);
      mAnim.SetBool("ButtonLeft", controller.DpadLeft.Down);
      mAnim.SetBool("ButtonUp", controller.DpadUp.Down);
      mAnim.SetBool("ButtonDown", controller.DpadDown.Down);
      mAnim.SetBool("ButtonRight", controller.DpadRight.Down);
      mAnim.SetBool("LTrigger", controller.LeftButton.Down);
      mAnim.SetBool("RTrigger", controller.RightButton.Down);
      mAnim.SetBool("FoldUp", false);
      mAnim.SetBool("FoldDown", false);
      mAnim.SetFloat("Pedal", controller.InputSpeed / 6.5f);
      mAnim.SetFloat("ResistanceUp", controller.UncalibratedResistance());
      mAnim.SetFloat("SeatDown", 0);
   }
}
