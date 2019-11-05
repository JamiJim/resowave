//***********************************************************************
// Copyright 2014 VirZOOM
//***********************************************************************

using UnityEngine;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Globalization;

[StructLayout(LayoutKind.Sequential, Pack=8)]
public struct VZMotionInput
{
   public float DraftSpeed;
   public float DraftFactor;
   public float DownhillFactor;
   public float UphillFactor;
   public float ControllerHeadLean;
   public float LeanFudge;
   public float ApparentLean;
   public float ControllerHeadRot;
   public float MaxTurn;
   public float StoppedTurnFraction;
   public float BodyRot;
   public float Scale;
   public float LandingHardness;
   public float LandingRadius;
   public float ControllerInputSpeed;
   public float SpeedMultiplier;
   public float MaxSpeed;
   public float MaxVertSpeed;
   public float VelocityX;
   public float VelocityY;
   public float VelocityZ;
   public float DeltaTime;
   public float HitDistance;
   public float HitNormalX;
   public float HitNormalY;
   public float HitNormalZ;
   public float PlayerX;
   public float PlayerY;
   public float PlayerZ;
   [MarshalAs(UnmanagedType.U1)]
   public bool Colliding;
   [MarshalAs(UnmanagedType.U1)]
   public bool OnMenu;
   [MarshalAs(UnmanagedType.U1)]
   public bool AllowDrift;
   [MarshalAs(UnmanagedType.U1)]
   public bool AllowYaw;
   [MarshalAs(UnmanagedType.U1)]
   public bool AllowPitch;
   [MarshalAs(UnmanagedType.U1)]
   public bool AllowRoll;
   [MarshalAs(UnmanagedType.U1)]
   public bool AllowReverse;
   [MarshalAs(UnmanagedType.U1)]
   public bool ForceReverse;
   [MarshalAs(UnmanagedType.U1)]
   public bool RaycastResult;
   [MarshalAs(UnmanagedType.U1)]
   public bool RotateAtStop;
   [MarshalAs(UnmanagedType.U1)]
   public bool ResistanceAdjust;

   public void Init()
   {
      DownhillFactor = 0.5f;
      UphillFactor = 1.414f;
      ControllerHeadLean = 0;
      LeanFudge = 2;
      ApparentLean = 0.5f;
      ControllerHeadRot = 0;
      MaxTurn = 0.5f;
      StoppedTurnFraction = 0;
      BodyRot = 0;
      Scale = 1;
      LandingHardness = 2;
      LandingRadius = 0.25f;
      ControllerInputSpeed = 0;
      SpeedMultiplier = 1;
      MaxSpeed = 20;
      MaxVertSpeed = 4;
      VelocityX = 0;
      VelocityY = 0;
      VelocityZ = 0;
      DeltaTime = 0.016f;
      HitDistance = 0;
      HitNormalX = 0;
      HitNormalY = 1;
      HitNormalZ = 0;
      PlayerX = 0;
      PlayerY = 0;
      PlayerZ = 0;
      Colliding = true;
      OnMenu = false;
      AllowDrift = true;
      AllowYaw = true;
      AllowPitch = false;
      AllowRoll = false;
      AllowReverse = false;
      ForceReverse = false;
      RaycastResult = false;
      RotateAtStop = false;
   }
}

[StructLayout(LayoutKind.Sequential, Pack=8)]
public struct VZMotionOutput
{
   public float BodyRot;
   public float RotVel;
   public float Turn;
   public float VelDot;
   public float TransverseDot;
   public float Speed;
   public float Lean;
   public float TransformEulerX;
   public float TransformEulerY;
   public float TransformEulerZ;
   public float VelocityX;
   public float VelocityY;
   public float VelocityZ;
   public float PlayerX;
   public float PlayerY;
   public float PlayerZ;
   public float ResistanceAdjust;
   [MarshalAs(UnmanagedType.U1)]
   public bool LandingSmoothed;
}

[StructLayout(LayoutKind.Sequential, Pack=8)]
public struct VZBikeState
{
   public float Timestamp;
   public float TimeAtLastPulseMs;
   public float HeartRate;
   public float BatteryVolts;
   public float Speed;
   public float ReprogramProgress;
   public int Pulses;
   public int FilteredResistance;
   public int Type;
   public int BetaVersion;
   public int Firmware;
   [MarshalAs(UnmanagedType.U1)]
   public bool LeftTrigger;
   [MarshalAs(UnmanagedType.U1)]
   public bool DpadUp;
   [MarshalAs(UnmanagedType.U1)]
   public bool DpadDown;
   [MarshalAs(UnmanagedType.U1)]
   public bool DpadLeft;
   [MarshalAs(UnmanagedType.U1)]
   public bool DpadRight;
   [MarshalAs(UnmanagedType.U1)]
   public bool RightTrigger;
   [MarshalAs(UnmanagedType.U1)]
   public bool RightUp;
   [MarshalAs(UnmanagedType.U1)]
   public bool RightDown;
   [MarshalAs(UnmanagedType.U1)]
   public bool RightLeft;
   [MarshalAs(UnmanagedType.U1)]
   public bool RightRight;
   [MarshalAs(UnmanagedType.U1)]
   public bool Connected;
   [MarshalAs(UnmanagedType.U1)]
   public bool Licensed;
   public byte Sender0;
   public byte Sender1;
   public byte Sender2;
   public byte Sender3;
   public byte Sender4;
   public byte Sender5;

   public string Sender()
   {
      /* Not working!
      string sender;
      VZPlugin.SenderString(Sender0, Sender1, Sender2, Sender3, Sender4, Sender5, out sender);
      return sender;
      */

      char[] s = new char[12];
      s[0] = HexChar(Sender0 >> 4);
      s[1] = HexChar(Sender0 & 0xF);
      s[2] = HexChar(Sender1 >> 4);
      s[3] = HexChar(Sender1 & 0xF);
      s[4] = HexChar(Sender2 >> 4);
      s[5] = HexChar(Sender2 & 0xF);
      s[6] = HexChar(Sender3 >> 4);
      s[7] = HexChar(Sender3 & 0xF);
      s[8] = HexChar(Sender4 >> 4);
      s[9] = HexChar(Sender4 & 0xF);
      s[10] = HexChar(Sender5 >> 4);
      s[11] = HexChar(Sender5 & 0xF);

      return new string(s);
   }

   char HexChar(int i)
   {
      switch (i)
      {
         case 0: return '0';
         case 1: return '1';
         case 2: return '2';
         case 3: return '3';
         case 4: return '4';
         case 5: return '5';
         case 6: return '6';
         case 7: return '7';
         case 8: return '8';
         case 9: return '9';
         case 10: return 'A';
         case 11: return 'B';
         case 12: return 'C';
         case 13: return 'D';
         case 14: return 'E';
         case 15: return 'F';
         default: return '?';
      }
   }

   public void Init()
   {
      Type = (int)VZBikeType.None;
      Connected = false;
      BetaVersion = 0;
      Licensed = false;
      LeftTrigger = false;
      DpadUp = false;
      DpadDown = false;
      DpadLeft = false;
      DpadRight = false;
      RightTrigger = false;
      RightUp = false;
      RightDown = false;
      RightLeft = false;
      RightRight = false;
      Timestamp = 0;
      Pulses = 0;
      TimeAtLastPulseMs = 0;
      FilteredResistance = -1; // unknown
      HeartRate = 0;
      BatteryVolts = -1; // unknown
      BetaVersion = -1; // unknown
      Firmware = -1; // unknown
      ReprogramProgress = 0;
      Sender0 = 0;
      Sender1 = 0;
      Sender2 = 0;
      Sender3 = 0;
      Sender4 = 0;
      Sender5 = 0;
      Speed = 0;
   }
}

public enum VZBikeType
{
   NeedsDriver = -2,
   None = -1,
   Unsupported = 0,
   Alpha = 1,
   Beta = 2,
   WiredLifecycle = 3
}

public static class VZPlugin
{
#if UNITY_ANDROID && !UNITY_EDITOR
   static AndroidJavaClass mMainActivity;
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
   public static void RequestPermissions()
   {
# if VZ_GOOGLEVR
      // Must use Google's own requester
      string[] permissionNames = { 
         "android.permission.BLUETOOTH", 
         "android.permission.BLUETOOTH_ADMIN", 
         "android.permission.ACCESS_FINE_LOCATION", 
         "android.permission.READ_EXTERNAL_STORAGE", 
         "android.permission.SET_DEBUG_APP", 
         };

      GvrPermissionsRequester permission = GvrPermissionsRequester.Instance;

      CheckPermissions(true);

      permission.RequestPermissions(permissionNames,
         (GvrPermissionsRequester.PermissionStatus[] permissionResults) =>
         {
            CheckPermissions(false);
         });
# else
      AndroidJavaClass bleClass = new AndroidJavaClass("com.virzoom.ble.vzble.blueGigaBLE");
      bleClass.CallStatic("grantPermissions");
# endif
   }

   public static void CheckPermissions(bool preCheck)
   {
      AndroidJavaClass bleClass = new AndroidJavaClass("com.virzoom.ble.vzble.blueGigaBLE");
      bleClass.CallStatic("checkPermissions", preCheck);
   }
#endif

#if UNITY_PS4 && !UNITY_EDITOR
	[DllImport("vzplugin")]
	public static extern bool PS4HeadsetConnected();

	[DllImport("vzplugin")]
	static extern void PS4Print(IntPtr msg);

   public static void PS4Print(string str)
   {
      IntPtr msg = Marshal.StringToHGlobalAnsi(str);
      PS4Print(msg);
      Marshal.FreeHGlobal(msg);
   }

	[DllImport("vzplugin")]
	public static extern void PS4SetBackground(bool background);
#endif

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
   [DllImport("vzplugin")]
   static extern IntPtr PCGraphicsDriverVersion();

   public static void PCGraphicsDriverVersion(out string version)
   {
      IntPtr buffer = PCGraphicsDriverVersion();
      version = Marshal.PtrToStringAnsi(buffer);
   }

   [DllImport("vzplugin")]
   static extern IntPtr PCOculusVersion();

   public static void PCOculusVersion(out string version)
   {
      IntPtr buffer = PCOculusVersion();
      version = Marshal.PtrToStringAnsi(buffer);
   }
#endif

#if UNITY_WSA_10_0 && !UNITY_EDITOR
   [DllImport("vzplugin")]
   static extern void UWPSetClipboard(string text);

   public static void SetClipboard(string text)
   {
      try
      {
         UnityEngine.WSA.Application.InvokeOnUIThread(() =>
            {
               UWPSetClipboard(text);
            }, false);
      }
      catch (Exception e)
      {
         Debug.Log("Failed to set clipboard");
      }
   }

   [DllImport("vzplugin")]
   public static extern void UWPPrint(string text);
#endif

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN //|| UNITY_WSA_10_0
   [DllImport("vzplugin", EntryPoint="VZButtonDown")]
   public static extern bool ButtonDown();

   [DllImport("vzplugin", EntryPoint="VZButtonUp")]
   public static extern bool ButtonUp();

   [DllImport("vzplugin", EntryPoint="VZButtonLeft")]
   public static extern bool ButtonLeft();

   [DllImport("vzplugin", EntryPoint="VZButtonRight")]
   public static extern bool ButtonRight();

#elif UNITY_WSA_10_0
   // GetAsyncKeyState doesn't work with Mixed Reality!
   // Assume buttons are remapped to 1-4 by Autohotkey & vzbutton.ahk
   public static bool ButtonUp()
   {
      return Input.GetKey(KeyCode.Alpha1);
   }

   public static bool ButtonDown()
   {
      return Input.GetKey(KeyCode.Alpha2);
   }

   public static bool ButtonRight()
   {
      return Input.GetKey(KeyCode.Alpha3);
   }

   public static bool ButtonLeft()
   {
      return Input.GetKey(KeyCode.Alpha4);
   }
#elif UNITY_ANDROID
   // GetAsyncKeyState doesn't work with Mixed Reality!
   public static bool ButtonUp()
   {
      return mMainActivity.CallStatic<bool>("getMediaPrev");
   }

   public static bool ButtonDown()
   {
      return mMainActivity.CallStatic<bool>("getMediaNext");
   }

   public static bool ButtonRight()
   {
      return false;
   }

   public static bool ButtonLeft()
   {
      return false;
   }

   public static bool ButtonConnected()
   {
      return mMainActivity.CallStatic<bool>("buttonConnected");
   }
#endif

#if UNITY_IOS
   private const string pluginName = "__Internal";
#else
   private const string pluginName = "vzplugin";
#endif

   [DllImport(pluginName, EntryPoint="VZInit")]
   static extern void VZInit();

   public static void Init(string dllPath = null)
   {
      // Init plugin dir
      if (dllPath != null)
      {
#if !UNITY_EDITOR
         // Player
         // add plugins path to the environment for the steam dll.
         var currentPath = Environment.GetEnvironmentVariable("PATH",
            EnvironmentVariableTarget.Process);

         if (currentPath != null && currentPath.Contains(dllPath) == false)
            Environment.SetEnvironmentVariable("PATH", currentPath + Path.PathSeparator + dllPath, EnvironmentVariableTarget.Process);
#endif
      }

#if UNITY_ANDROID && !UNITY_EDITOR
      // Init bluetooth class
      AndroidJavaClass playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
      AndroidJavaObject activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");

      AndroidJavaClass bleClass = new AndroidJavaClass("com.virzoom.ble.vzble.blueGigaBLE");

      mMainActivity = new AndroidJavaClass("com.virzoom.pluginapp.MainActivity");
# if VZ_ARCADE && VZ_GOOGLEVR
      bool grantPermissions = false;
# else
      bool grantPermissions = true;
# endif
      bleClass.CallStatic("create", activity, grantPermissions);
#endif

      VZInit();
   }

   [DllImport(pluginName)]
   static extern void VZSetGameName(IntPtr msg);

   public static void VZSetGameName(string str)
   {
      IntPtr msg = Marshal.StringToHGlobalAnsi(str);
      VZSetGameName(msg);
      Marshal.FreeHGlobal(msg);
   }

   [DllImport(pluginName, EntryPoint="VZResetSpeed")]
   public static extern void ResetSpeed(float speed);

   [DllImport(pluginName, EntryPoint="VZResetMotion")]
   public static extern void ResetMotion();

   [DllImport(pluginName, EntryPoint="VZSetTurnSettleTime")]
   public static extern void SetTurnSettleTime(float time);

   [DllImport(pluginName, EntryPoint="VZUpdateMotion")]
   public static extern void UpdateMotion(ref VZMotionInput input, ref VZMotionOutput output);

   [DllImport(pluginName, EntryPoint="VZConnectBike")]
   static extern void ConnectBike(ref VZBikeState state, IntPtr addr, bool bike, bool sensor, bool lifecycle);

   public static void ConnectBike(ref VZBikeState state, string sender, bool bike, bool sensor, bool lifecycle)
   {
      var s = new byte[6];

      // Convert to bike id
      if (sender != null && sender.Length == 12)
      {
         try
         {
            s[0] = byte.Parse(sender.Substring(0, 2), NumberStyles.HexNumber);
            s[1] = byte.Parse(sender.Substring(2, 2), NumberStyles.HexNumber);
            s[2] = byte.Parse(sender.Substring(4, 2), NumberStyles.HexNumber);
            s[3] = byte.Parse(sender.Substring(6, 2), NumberStyles.HexNumber);
            s[4] = byte.Parse(sender.Substring(8, 2), NumberStyles.HexNumber);
            s[5] = byte.Parse(sender.Substring(10, 2), NumberStyles.HexNumber);
         }
         catch (Exception e)
         {
            s = null;
         }
      }
      else
      {
         s = null;
      }

      // Connect to valid bike id, or else any bike
      if (s != null)
      {
         IntPtr unmanagedPointer = Marshal.AllocHGlobal(s.Length);
         Marshal.Copy(s, 0, unmanagedPointer, s.Length);
         ConnectBike(ref state, unmanagedPointer, bike, sensor, lifecycle);
         Marshal.FreeHGlobal(unmanagedPointer);
      }
      else
      {
         ConnectBike(ref state, (IntPtr)null, bike, sensor, lifecycle);
      }
   }

   [DllImport(pluginName, EntryPoint="VZUpdateBike")]
   public static extern bool UpdateBike(ref VZBikeState state, float time);

   [DllImport(pluginName, EntryPoint="VZCloseBike")]
   public static extern void CloseBike();

   [DllImport(pluginName, EntryPoint="VZSetResistance")]
   public static extern void SetResistance(byte level);

   [DllImport(pluginName, EntryPoint="VZWearingHeadset")]
   public static extern void WearingHeadset(bool wearing);

   [DllImport(pluginName, EntryPoint="VZSenderString")]
   static extern IntPtr SenderString(byte s0, byte s1, byte s2, byte s3, byte s4, byte s5);

   public static void SenderString(byte s0, byte s1, byte s2, byte s3, byte s4, byte s5, out string sender)
   {
      char[] s = new char[12];
      IntPtr str = SenderString(s0, s1, s2, s3, s4, s5);
      Marshal.Copy(str, s, 0, s.Length);
      sender = new string(s);
   }

   [DllImport(pluginName, EntryPoint="VZRequestAuth")]
   public static extern bool RequestToken();

   [DllImport(pluginName, EntryPoint="VZGetLFID")]
   static extern IntPtr GetLFID();

   public static void GetLFID(out byte[] lfid)
   {
      lfid = new byte[22];
      IntPtr buffer = GetLFID();
      Marshal.Copy(buffer, lfid, 0, 22);
   }

   [DllImport(pluginName, EntryPoint="VZAuthToken")]
   static extern IntPtr ReturnToken();

   public static void ReturnToken(out byte[] token)
   {
      token = new byte[17];
      IntPtr buffer = ReturnToken();
      Marshal.Copy(buffer, token, 0, 17);
   }

   [DllImport(pluginName, EntryPoint="VZSetLicense")]
   static extern void SetLicense(IntPtr license);

   public static void SetLicense(byte[] license)
   {
      // To try without copy use Marshal.UnsafeAddrOfPinnedArrayElement(license, 0)
      IntPtr unmanagedPointer = Marshal.AllocHGlobal(license.Length);
      Marshal.Copy(license, 0, unmanagedPointer, license.Length);
      SetLicense(unmanagedPointer);
      Marshal.FreeHGlobal(unmanagedPointer);
   }

   [DllImport(pluginName, EntryPoint="VZSettingLicense")]
   public static extern bool SettingLicense();
}
