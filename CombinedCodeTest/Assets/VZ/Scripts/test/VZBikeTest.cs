//***********************************************************************
// Copyright 2014 VirZOOM
//***********************************************************************

using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections;

public class VZBikeTest : MonoBehaviour
{
   TextMesh mText;

   enum LicenseState
   {
      Init,
      Setting,
      Succeeded,
      Failed,
      Offline,
      Unregistered,
      NotActive
   };

   void Start()
   {
      // Lookup text objects
      mText = GetComponent<TextMesh>();
   }

   void Update()
   {
      var controller = VZPlayer.Controller;

      mText.text = 
         "Type: " + TypeText(controller.BikeType(), controller.BikeBetaVersion()) + "\n" +
         "Connected: " + controller.IsBikeConnected() + "\n" +
         "Licensed: " + controller.IsBikeLicensed() + "\n" +
         "SenderAddress: " + controller.BikeSender() + "\n" +
         "HeartRate: " + controller.HeartRate() + "\n" +
         "BatteryVolts: " + controller.BatteryVolts() + "\n" +
         "Speed: " + controller.InputSpeed + "\n" +
         "Resistance: " + controller.UncalibratedResistance() + "\n" +
         "LeftGrip: " + GripText(controller.LeftButton.Down, controller.DpadUp.Down, controller.DpadDown.Down, controller.DpadLeft.Down, controller.DpadRight.Down) + "\n" +
         "RightGrip: " + GripText(controller.RightButton.Down, controller.RightUp.Down, controller.RightDown.Down, controller.RightLeft.Down, controller.RightRight.Down) + "\n" +
         "Difficulty: " + VZPlayer.Instance.GetDifficulty();

      if (controller.RightButton.Released())
      {
         int difficulty = VZPlayer.Instance.GetDifficulty() + 1;
         if (difficulty > 8)
            difficulty = 1;
         VZPlayer.Instance.SetDifficulty(difficulty);
      }
   }

   string TypeText(int type, int version)
   {
      if (type < 0)
         return "none";
      else if (type == 0)
         return "unsupported bike";
      else if (type == 1)
         return "alpha bike";
      else if (type == 2)
      {
         if (version == 2)
            return "bike sensor";
         else
            return "beta bike";
      }
      else
         return "unknown";
   }

   string GripText(bool trigger, bool up, bool down, bool left, bool right)
   {
      string text = "";

      if (trigger)
         text += "trigger ";
      if (up)
         text += "up ";
      if (down)
         text += "down ";
      if (left)
         text += "left ";
      if (right)
         text += "right ";

      return text;
   }
}
