//***********************************************************************
// Copyright 2014 VirZOOM
//***********************************************************************

using UnityEngine;
using System;

public class VZControllerMap
{
   public enum ControlType
   {
      Key,
      Axis,
      Custom
   };

   public class Control
   {
      public ControlType Type = ControlType.Custom;
      public string Id = "None";
      public float Multiplier = 1;
      public float Offset = 0;
      public KeyCode Code;

      public virtual bool GetBool()
      {
         if (Type == ControlType.Key)
            return Input.GetKey(Code);
         else if (Type == ControlType.Axis)
            return Input.GetAxis(Id) * Multiplier + Offset > 0;
         else
            return GetFloat() != 0 ? true : false;
      }

      public virtual float GetFloat()
      {
         if (Type == ControlType.Key)
            return Input.GetKey(Code) ? 1 : 0;
         else if (Type == ControlType.Axis)
            return Input.GetAxis(Id) * Multiplier + Offset;
         else
            return 0;
      }

      public void Finalize(int joystick)
      {
         Id = string.Format(Id, joystick);

         if (Type == ControlType.Key)
            Code = (KeyCode)Enum.Parse(typeof(KeyCode), Id);
      }
   }

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA_10_0 || UNITY_ANDROID
   class VZButtonLeftTrigger : Control
   {
      public override float GetFloat()
      {
         return VZPlugin.ButtonDown() ? 1 : 0;
      }
   }

   class VZButtonRightTrigger : Control
   {
      public override float GetFloat()
      {
         return VZPlugin.ButtonUp() ? 1 : 0;
      }
   }

   class VZButtonLeftLeft : Control
   {
      public override float GetFloat()
      {
         return VZPlugin.ButtonLeft() ? 1 : 0;
      }
   }

   class VZButtonLeftRight : Control
   {
      public override float GetFloat()
      {
         return VZPlugin.ButtonRight() ? 1 : 0;
      }
   }
#endif

#if (VZ_ARCADE || VZ_STREETVIEW) && (VZ_GEARVR || VZ_GOOGLEVR) && !UNITY_EDITOR
   class MobileForward : Control
   {
      public override float GetFloat()
      {
# if VZ_GOOGLEVR
         return GvrController.IsTouching ? (0.75f - GvrController.TouchPos.y) * 1.466f : 0;
# else
         return OVRInput.Get(OVRInput.Touch.PrimaryTouchpad) ? (OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad).y + 0.5f) * 0.733f : 0;
# endif
      }
   }

   class MobileReverse : Control
   {
      public override float GetFloat()
      {
# if VZ_GOOGLEVR
         return GvrController.IsTouching ? (GvrController.TouchPos.y - 0.75f) * 4.4f : 0;
# else
         return OVRInput.Get(OVRInput.Touch.PrimaryTouchpad) ?  (-0.5f - OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad).y) * 2.2f : 0;
# endif
      }
   }

   class MobileLeftTrigger : Control
   {
      public override float GetFloat()
      {
# if VZ_GOOGLEVR
         return GvrController.AppButton ? 1 : 0;
# else
         return OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) ? 1 : 0;
# endif
      }
   }

   class MobileRightTrigger : Control
   {
      public override float GetFloat()
      {
# if VZ_GOOGLEVR
         if (VZController.IsCardboard())
            return Input.GetMouseButton(0) ? 1 : 0;
         else
            return GvrController.ClickButton ? 1 : 0;
# else
         return OVRInput.Get(OVRInput.Button.PrimaryTouchpad) ? 1 : 0;
# endif
      }
   }
#endif

   public class Controller
   {
      public string Name;
      public string Description;
      public string Icons;
      public Control LeftUp;
      public Control LeftLeft;
      public Control LeftRight;
      public Control LeftDown;
      public Control RightUp;
      public Control RightLeft;
      public Control RightRight;
      public Control RightDown;
      public Control LeftTrigger;
      public Control RightTrigger;
      public Control LeanLeft;
      public Control LeanRight;
      public Control LeanForward;
      public Control LeanBack;
      public Control LookLeft;
      public Control LookRight;
      public Control LookUp;
      public Control LookDown;
      public Control Forward;
      public Control Reverse;

      public void Init()
      {
         LeftUp = new Control();
         LeftLeft = new Control();
         LeftRight = new Control();
         LeftDown = new Control();
         RightUp = new Control();
         RightLeft = new Control();
         RightRight = new Control();
         RightDown = new Control();
         LookLeft = new Control();
         LookRight = new Control();
         LookUp = new Control();
         LookDown = new Control();
         LeanLeft = new Control();
         LeanRight = new Control();
         LeanForward = new Control();
         LeanBack = new Control();
         LeftTrigger = new Control();
         RightTrigger = new Control();
         Forward = new Control();
         Reverse = new Control();
      }
   }

   public Controller[] Controllers;

   Controller FindController(string description)
   {
      foreach (var controller in Controllers)
      {
         if (controller.Description == description)
            return controller;
      }
      
      return null;
   }

   public Controller PickController(bool sensor, bool steamVR)
   {
      string[] joysticks = Input.GetJoystickNames();
      int joyNum = 0;
      Controller controller = null;

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_WSA_10_0 || UNITY_ANDROID
      if (sensor)
      {
         controller = new Controller();
         controller.Name = "VZButton";
         controller.Description = "VZButton";
         controller.Icons = "buttons_vzbutton";
         controller.Init();
         controller.LeftTrigger = new VZButtonLeftTrigger();
         controller.RightTrigger = new VZButtonRightTrigger();
         controller.LeftLeft = new VZButtonLeftLeft();
         controller.LeftRight = new VZButtonLeftRight();
      }
      else
#endif

#if (VZ_ARCADE || VZ_STREETVIEW) && (VZ_GEARVR || VZ_GOOGLEVR) && !UNITY_EDITOR
      {
         controller = new Controller();
#if VZ_GOOGLEVR
         if (VZController.IsCardboard())
         {
            controller.Name = "Cardboard";
            controller.Description = "Cardboard";
            // Could be Bobo controller or selfie-stick buttons
            controller.Icons = "buttons_d";
         }
         else
         {
            controller.Name = "Gvr";
            controller.Description = "Gvr";
            controller.Icons = "buttons_Daydream";
         }
#else
         controller.Name = "GearVR";
         controller.Description = "GearVR";
         controller.Icons = "buttons_GearVR";
#endif
         controller.Init();
         controller.LeftTrigger = new MobileLeftTrigger();
         controller.RightTrigger = new MobileRightTrigger();
         controller.Forward = new MobileForward();
         controller.Reverse = new MobileReverse();
      }
#endif

      foreach (var desc in joysticks)
      {
         if (controller != null)
            break;

         joyNum++;

#if UNITY_PS4 && !UNITY_EDITOR
         // Treat any joystick as a DS4 on PS4
         if (desc != "")
            controller = FindController("DS4");
         else
#endif
            controller = FindController(desc);
      }

      if (controller == null)
         controller = FindController("Keyboard");

      // Set joystick index
      controller.LeftUp.Finalize(joyNum);
      controller.LeftLeft.Finalize(joyNum);
      controller.LeftRight.Finalize(joyNum);
      controller.LeftDown.Finalize(joyNum);
      controller.RightUp.Finalize(joyNum);
      controller.RightLeft.Finalize(joyNum);
      controller.RightRight.Finalize(joyNum);
      controller.RightDown.Finalize(joyNum);
      controller.LeftTrigger.Finalize(joyNum);
      controller.RightTrigger.Finalize(joyNum);
      controller.LeanLeft.Finalize(joyNum);
      controller.LeanRight.Finalize(joyNum);
      controller.LeanForward.Finalize(joyNum);
      controller.LeanBack.Finalize(joyNum);
      controller.LookLeft.Finalize(joyNum);
      controller.LookRight.Finalize(joyNum);
      controller.LookUp.Finalize(joyNum);
      controller.LookDown.Finalize(joyNum);
      controller.Forward.Finalize(joyNum);
      controller.Reverse.Finalize(joyNum);

      return controller;
   }
}
