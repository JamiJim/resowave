//***********************************************************************
// Copyright 2014 VirZOOM
//***********************************************************************

using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

#if UNITY_PS4 && !UNITY_EDITOR
using UnityEngine.PS4.VR;
#endif

#if VZ_ARCADE && !VZ_MOBILE && !UNITY_WSA_10_0 && UNITY_STANDALONE_WIN
using Valve.VR;
#endif

public class VZController : MonoBehaviour
{
   //***********************************************************************
   // PUBLIC API
   //***********************************************************************

   /*
   Our InputSpeed is based on a 3:1 gear ratio with 27" wheels, which works out to be a 40.5 gear-inch
   which is right in the middle of a touring bike's gear range.
   https://www.cyclingabout.com/gear-ratios-how-to-select-touring-bike-gearing/
   */
   public const float kMetersPerRev = 3 * 2.154f; 
   public const float kMilesToKm = 1.60934f;
   public const float kSpinsToMiles = kMetersPerRev / 1000 / kMilesToKm;

   public class Button
   {
      public bool Down { get; private set; }

      public Button()
      {
         Down = false;
      }

      public void Set(bool down)
      {
         if (down == Down)
            return;

         if (down)
         {
            Down = true;
            mDownTime = VZTime.time;
            mPossibleFalseUp = false;
         }
         else if (VZTime.time - mDownTime < 0.1f)
         {
            // Don't update Down yet
            mPossibleFalseUp = true;
         }
         else
         {
            Down = false;
            mPossibleFalseUp = false;
         }
      }

      public void Update()
      {
         // Handle possible false button up
         if (mPossibleFalseUp && VZTime.time - mDownTime > .2f)
         {
            mPossibleFalseUp = false;
            Down = false;
         }

         mChanged = (mLastDown != Down);
         mLastDown = Down;
      }

      public bool Pressed()
      {
         return mChanged && Down;
      }

      public bool Released()
      {
         return mChanged && !Down && mDownTime != 0;
      }

      public float DownTime()
      {
         return mDownTime;
      }

      public bool Held(float period)
      {
         return Down && mDownTime != 0 && VZTime.time - mDownTime > period;
      }

      public void Clear()
      {
         mDownTime = 0;
      }

      float mDownTime;
      bool mPossibleFalseUp = false;
      bool mLastDown = false;
      bool mChanged = false;
   }

   public TextAsset ControllerMap;
   public bool TiltSteering = false;
   public Button LeftButton = new Button();
   public Button RightButton = new Button();
   public Button DpadUp = new Button();
   public Button DpadDown = new Button();
   public Button DpadLeft = new Button();
   public Button DpadRight = new Button();
   public Button RightUp = new Button();
   public Button RightDown = new Button();
   public Button RightLeft = new Button();
   public Button RightRight = new Button();
   public float InputSpeed { get; private set; } 
   public float HeadRot { get; private set; }  // positive counterclockwise
   public float HeadLean { get; private set; } // positive left
   public float HeadBend { get; private set; }
   public bool IsSteamVR { get; private set; }
   public float Spins { get; private set; }
   public Transform Head { get; private set; }

   public string LicenseStatus()
   {
      if (mLicenseState == LicenseState.Init)
         return "Need to check license";
      else if (mLicenseState == LicenseState.Setting)
         return "Checking VZfit license";
      else if (mLicenseState == LicenseState.NotActive)
         return mLicenseOwner + "'s license expired, update at vzfit.com/account";
      else if (mLicenseState == LicenseState.Failed)
         return "Please contact support@virzoom.com about your license";
      else if (mLicenseState == LicenseState.Offline)
         return "Can't retrieve license from server";
      else if (mLicenseState == LicenseState.Unregistered)
         return "Please contact support@virzoom.com about your license";
      else if (mLicenseState == LicenseState.Succeeded)
         return "License verified";
      else
         return "Unknown license status";
   }

   public bool LicenseOffline()
   {
      return mLicenseState == LicenseState.Offline;
   }

   public bool LicenseUnregistered()
   {
      return mLicenseState == LicenseState.Unregistered;
   }

   public virtual void SetBikeLicense()
   {
      StartCoroutine(SettingBikeLicense(null, null, null, true, null));
   }

   public float EstimatedDistance 
   {
      get { return Spins * kSpinsToMiles * kMilesToKm; }
      private set {}
   }

   public bool IsCardboard()
   {
#if UNITY_EDITOR
      return false;
#elif UNITY_IOS
      return true;
#elif VZ_GEARVR
      return false;
#elif VZ_GOOGLEVR
      // For some reason not true on ios
      return GvrSettings.ViewerPlatform == GvrSettings.ViewerPlatformType.Cardboard;
#else
      return false;
#endif
   }

   public float BatteryVolts()
   {
      return mBikeState.BatteryVolts;
   }

   public int BikePulses()
   {
      return mBikeState.Pulses;
   }

   public float HeartRate()
   {
      float heartRate = mBikeState.HeartRate;

      if (IsBetaBike() || IsLifeCycle())
      {
         if (heartRate == 0)
            heartRate = 70; // assume 70 if hands off or alpha bike
         else if (heartRate > 200)
            heartRate = 200;
      }

      return heartRate;
   }

   public void OverrideHeartrate(float heartrate)
   {
      mBikeState.HeartRate = heartrate;
   }

   public bool HasBikeDongle()
   {
      return mBikeState.Type >= 0;
   }

   public float BikeReprogramProgress()
   {
      return mBikeState.ReprogramProgress;
   }

   public bool IsBikeConnected()
   {
      return mBikeState.Connected;
   }

   public bool IsBikeLicensed()
   {
      return mBikeState.Licensed;
   }

   public bool NeedsDongleDriver()
   {
      return mBikeState.Type == (int)VZBikeType.NeedsDriver;
   }
   
   public bool IsUnsupportedBike()
   {
      return mBikeState.Type == (int)VZBikeType.Unsupported;
   }

   public bool IsAlphaBike()
   {
      return mBikeState.Type == (int)VZBikeType.Alpha;
   }

   public bool IsBetaBike()
   {
      return mBikeState.Type == (int)VZBikeType.Beta && mBikeState.BetaVersion == 1;
   }

   public bool IsBikeSensor()
   {
      return mBikeState.Type == (int)VZBikeType.Beta && mBikeState.BetaVersion == 2;
   }

   public virtual bool IsLifeCycle()
   {
      return (mBikeState.Type == (int)VZBikeType.Beta && mBikeState.BetaVersion == 3) || IsWiredLifeCycle();
   }

   public int LifeCycleType()
   {
      byte[] lfid;
      VZPlugin.GetLFID(out lfid);
      return (int)lfid[0]; // 0:unknown, 11:upright, 12:recumbent, 16:elliptical
   }

   public string LifeCycleID()
   {
      byte[] lfid;
      VZPlugin.GetLFID(out lfid);
      int end;
      for (end = 1; end < 22; end++)
         if (lfid[end] == 0)
            break;
      return System.Text.Encoding.UTF8.GetString(lfid, 1, end - 1);
   }

   public virtual bool IsWiredLifeCycle()
   {
      return mBikeState.Type == (int)VZBikeType.WiredLifecycle;
   }

   // For logging
   public float BikeSpeed()
   {
      return mBikeState.Speed;
   }

   public int BikeType()
   {
      return mBikeState.Type;
   }

   public int BikeBetaVersion()
   {
      return mBikeState.BetaVersion;
   }

   public int BikeFirmware()
   {
      return mBikeState.Firmware;
   }

   public int UncalibratedResistance()
   {
      return mBikeState.FilteredResistance;
   }

   public bool HasHmd()
   {
      return mHasHmd;
   }

   public virtual void Recenter()
   {
      if (!IsHeadTracked())
      {
         Debug.LogError("Can't recenter if head isn't tracked");
         return;
      }

      // Reset hmd
      if (mHasHmd)
      {
         // Don't use Unity Recenter, leave Camera uncalibrated and manually offset
         float yawOffset = -mCamera.localEulerAngles.y;

#if UNITY_PS4 && !UNITY_EDITOR
         // Don't reset rotation on ps4 if over 25 from absolute
         if (Mathf.Abs(TrackedHeadYaw()) > 25)
            yawOffset = mCameraOffset.localEulerAngles.y;
#endif

         float ang = -yawOffset * Mathf.Deg2Rad;
         float sinAng = Mathf.Sin(ang);
         float cosAng = Mathf.Cos(ang);
         float xOffset = mCamera.localPosition.z * sinAng - mCamera.localPosition.x * cosAng;
         float zOffset = -mCamera.localPosition.z * cosAng - mCamera.localPosition.x * sinAng;
         float yOffset = -mCamera.localPosition.y;

         mCameraOffset.localPosition = new Vector3(xOffset, yOffset, zOffset);
         mCameraOffset.localEulerAngles = new Vector3(0, yawOffset, 0);

         Head.localPosition = new Vector3(0, 0, 0);

         Vector3 localAngles = mCamera.localEulerAngles;
         localAngles.y = 0;
         Head.localEulerAngles = localAngles;
      }
   }

   public float TrackedHeadYaw()
   {
      // Return local yaw of mCamera, which should be relative to VR tracking system
      return WrapDegrees(mCamera.localEulerAngles.y);
   }

   public bool IsHeadTracked()
   {
      if (!mHasHmd)
         return true;

#if UNITY_PS4 && !UNITY_EDITOR
      int handle = PlayStationVR.GetHmdHandle();

      PlayStationVRTrackingStatus status;
      Tracker.GetTrackedDeviceStatus(handle, out status);
      if (status != PlayStationVRTrackingStatus.Tracking)
         return false;

      PlayStationVRTrackingQuality quality;
      Tracker.GetTrackedDevicePositionQuality(handle, out quality);
      return quality == PlayStationVRTrackingQuality.Full;

#elif VZ_ARCADE && !VZ_MOBILE && !UNITY_WSA_10_0 && UNITY_STANDALONE_WIN
      if (IsSteamVR)
         return !SteamVR.outOfRange;
      else
         return OVRPlugin.positionTracked;

#elif UNITY_WSA_10_0
      return UnityEngine.XR.WSA.WorldManager.state == UnityEngine.XR.WSA.PositionalLocatorState.Active;
#else
      return true;
#endif
   }

   public virtual void Restart()
   {
      LeftButton.Clear();
      RightButton.Clear();
      DpadUp.Clear();
      DpadDown.Clear();
      DpadLeft.Clear();
      DpadRight.Clear();
      RightUp.Clear();
      RightDown.Clear();
      RightLeft.Clear();
      RightRight.Clear();

      TransitionCanvas().GetComponent<CanvasGroup>().alpha = 1;
   }

   public GameObject TransitionCanvas()
   {
      return mTransitionCanvas;
   }

   public virtual void ResetTransitionCanvas()
   {
      mTransitionCanvas.transform.localPosition = mTransitionCanvasPos;
      mTransitionCanvas.transform.localRotation = mTransitionCanvasRot;
      mTransitionCanvas.transform.localScale = mTransitionCanvasScale;
   }

   public string BikeSender()
   {
      if (mBikeState.Type == (int)VZBikeType.Beta)
      {
         // Beta bike
         return mBikeState.Sender();
      }
      else if (mBikeState.Type == (int)VZBikeType.Alpha || mBikeState.Type == (int)VZBikeType.WiredLifecycle)
      {
         // Alpha bike hack
#if UNITY_PS4 && !UNITY_EDITOR
         return Network.player.ipAddress; // MachineName crashes offline!
#else
         return System.Environment.MachineName;
#endif
      }
      else
      {
         return "";
      }
   }

   public virtual void ConnectBike(bool bike, bool sensor, bool lifecycle)
   {
#if VZ_ARCADE && !VZ_MOBILE
      if (VZReplay.Playback())
         mBikeState.Type = (int)VZBikeType.None;
      else
#endif
         VZPlugin.ConnectBike(ref mBikeState, null, bike, sensor, lifecycle);
   }

   public void CloseBike()
   {
      VZPlugin.CloseBike();
      mBikeState.Type = (int)VZBikeType.None;
      mBikeState.Connected = false;
   }

   public Transform Neck()
   {
      return mNeck;
   }

   public void AttachPlayer(VZPlayer player)
   {
      if (player != null)
      {
         // Copy player.Camera settings 
         var cam = GetCamera();
         cam.nearClipPlane = player.NearClipPlane;
         cam.farClipPlane = player.FarClipPlane;

         // Parent ourselves to player
         transform.localPosition = Vector3.zero;
         transform.localRotation = Quaternion.identity;
         transform.localScale = Vector3.one;
         transform.SetParent(player.transform, false);
      }
      else
      {
         // Detach from player so doesn't delete with it
         // Keep in same place
         transform.SetParent(null, true);

         // Need to recall this apparently
         DontDestroyOnLoad(gameObject);
      }
   }

   public string ControllerName()
   {
      if (!IsBikeConnected() || IsBikeSensor())
         return mController.Name;
      else if (IsLifeCycle())
         return "Lifecycle";
      else
         return "Bike";
   }

   public Transform CameraOffset()
   {
      return mCameraOffset;
   }

   public void PickController()
   {
      using (var stream = new StringReader(ControllerMap.text))
      {
         var serializer = new XmlSerializer(typeof(VZControllerMap));
         var controllerMap = serializer.Deserialize(stream) as VZControllerMap;

#if VZ_ARCADE && !UNITY_EDITOR
         if (!IsBikeConnected())
            mController = controllerMap.PickController(VZGamePlayer.Global.LastBike.BetaVersion == 2, IsSteamVR);
         else
#elif VZ_STREETVIEW && !UNITY_EDITOR
         if (!IsBikeConnected())
            mController = controllerMap.PickController(Explorer.Global.BikeBetaVersion == 2, IsSteamVR);
         else
#endif
            mController = controllerMap.PickController(IsBikeSensor(), IsSteamVR);
      }
   }

   public bool IsUserPresent()
   {
      if (!HasHmd())
         return true;

#if UNITY_WSA_10_0 && !UNITY_EDITOR
      if (mWinMRSeenNotPresent)
         return UnityEngine.XR.XRDevice.userPresence != UnityEngine.XR.UserPresenceState.NotPresent; // needs "run in background"
#elif !UNITY_EDITOR && VZ_GEARVR
      return OVRManager.instance.isUserPresent;
#elif VZ_ARCADE && !VZ_MOBILE && !UNITY_WSA_10_0 && UNITY_STANDALONE_WIN
      if (IsSteamVR)
         return OpenVR.System.GetTrackedDeviceActivityLevel(0) == EDeviceActivityLevel.k_EDeviceActivityLevel_UserInteraction;
      else
         return OVRManager.instance.isUserPresent;
#endif
      return VZTime.time < mHmdMovedTime + mMovedPeriod;
   }

   public Camera GetCamera()
   {
      return mCamera.GetComponent<Camera>();
   }

   public void SetDifficulty(int difficulty)
   {
      if (difficulty > 8)
         difficulty = 8;
      else if (difficulty < 1)
         difficulty = 1;

      mDifficulty = difficulty;
   }

   public int GetDifficulty()
   {
      return mDifficulty;
   }

   //***********************************************************************
   // PROTECTED API
   //***********************************************************************

   protected VZControllerMap.Controller mController;
   protected VZBikeState mBikeState = new VZBikeState();
   protected delegate void SaveBike(string bikeSender, int betaVersion, string license, string owner, string offlineDate, string team, string cookie);

#if !(VZ_ARCADE || VZ_STREETVIEW)
   public class License
   {
      public bool verified = false; // everything's good
      public bool unregistered = false; // bike not associated with user
      public bool is_active = false; // user doesn't have active subscription
      public string username = ""; // user name
      public string license = ""; // license key
      public string offline_date = ""; // local expiry of key
      public string team = ""; // bike's team
      public string cookie = "";  // returned cookie 
   }
#endif
   
   protected IEnumerator SettingBikeLicense(string sender, string license, string offlineDate, bool online, SaveBike saveBike)
   {
      float time = Time.time;

      mLicenseState = LicenseState.Setting;
      Log("SettingBikeLicense: start");

      Log(Time.realtimeSinceStartup.ToString());
      if (sender == BikeSender() && license != null && license != "")
      {
         // Send cached license right away
         var daysLeft = ParseServerDate(offlineDate) - DateTime.UtcNow;
         if (daysLeft.TotalDays > 0)
         {
            yield return new WaitForSeconds(0.2f);
            VZPlugin.SetLicense(Convert.FromBase64String(license));
            Log("SettingBikeLicense: send cached");

            time = Time.time;

            while (VZPlugin.SettingLicense() && Time.time < time + 3)
               yield return null;
         }
      }
      Log(Time.realtimeSinceStartup.ToString());

      // Get license
      float backoffPeriod = 1;

      while (mLicenseState == LicenseState.Setting || mLicenseState == LicenseState.NotActive)
      {
      Log(Time.realtimeSinceStartup.ToString());
#if VZ_ARCADE || VZ_STREETVIEW
         VZServer.License response = null;
#else
         License response = null;
#endif

      Log(Time.realtimeSinceStartup.ToString());
         // Request bike auth
         yield return new WaitForSeconds(0.2f);
      Log(Time.realtimeSinceStartup.ToString());
         VZPlugin.RequestToken();
      Log(Time.realtimeSinceStartup.ToString());
         Log("SettingBikeLicense: request token");
      Log(Time.realtimeSinceStartup.ToString());

         // Wait up to 3 sec for auth packet
         byte[] token = null;
         time = Time.time;

         while ((token == null || token[0] == 0) && Time.time < time + 3) 
         {
            VZPlugin.ReturnToken(out token);
            yield return null;
         }

         Log("SettingBikeLicense: waited for token");

         if (token != null && token[0] != 0)
         {
            Log("SettingBikeLicense: got token, getting license");

            // Get license from server (TODO make async)
#if VZ_ARCADE || VZ_STREETVIEW
            response = VZPlayer.Server.GetLicense(BikeSender(), BikeBetaVersion(), BikeFirmware(), token);
#else
            response = GetLicense(BikeSender(), BikeBetaVersion(), BikeFirmware(), token);
#endif

            if (response != null)
            {
               Log("SettingBikeLicense: got license");
               if (response.verified)
               {
                  // Set license if got one
                  Log("SettingBikeLicense: setting license");
                  license = response.license;
                  VZPlugin.SetLicense(Convert.FromBase64String(license));

                  if (saveBike != null)
                     saveBike(BikeSender(), BikeBetaVersion(), license, response.username, response.offline_date, response.team, response.cookie); 

                  time = Time.time;

                  while (VZPlugin.SettingLicense() && Time.time < time + 3)
                     yield return null;

                  if (!VZPlugin.SettingLicense())
                  {
                     // Wait a frame for Controller state to change
                     Log("SettingBikeLicense: done setting license");
                     yield return null;
                  }
               }
               else if (response.unregistered)
               {
                  Log("SettingBikeLicense: unregistered");
                  mLicenseState = LicenseState.Unregistered;
               }
               else if (response.is_active)
               {
                  Log("SettingBikeLicense: active but not verified");
                  mLicenseState = LicenseState.Failed;
               }
               else 
               {
                  // Inactive subscription, try again in 2 sec
                  Log("SettingBikeLicense: inactive subscription");
                  mLicenseOwner = response.username;
                  mLicenseState = LicenseState.NotActive;
               }
            }
            else
            {
               Log("SettingBikeLicense: no response");

               if (!online)
                  mLicenseState = LicenseState.Offline;
            }

            if (mLicenseState == LicenseState.Setting || mLicenseState == LicenseState.NotActive)
            {
               // Wait longer between each retry or we get 429 errors
               backoffPeriod *= 2;

               if (backoffPeriod > 60)
                  mLicenseState = LicenseState.Offline; // treat as offline after a minute

               yield return new WaitForSeconds(backoffPeriod);
            }
         }

         if (IsBikeLicensed())
         {
            // Continue even if success was the result of cached license
            Log("SettingBikeLicense: succeeded");
            mLicenseState = LicenseState.Succeeded;
         }
      }

      Log("SettingBikeLicense: done");
   }

#if !(VZ_ARCADE || VZ_STREETVIEW)
   License GetLicense(string bikeID, int bikeBetaVersion, int bikeFirmware, byte[] token)
   {
      string token64 = Convert.ToBase64String(token);
      string jsonStr = "{\"bikeID\":\"" + bikeID + "\",\"send\":\"" + token64 + "\",\"bikeBetaVersion\":\"" + bikeBetaVersion + "\",\"bikeFirmware\":\"" + bikeFirmware + "\"}";
//      string url = "https://vzfit.com/api/v6/send/";
      string url = "https://vz-fit.herokuapp.com/api/v6/send/";

      var www = UnityWebRequest.Post(url, "");
      www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonStr));
      www.uploadHandler.contentType = "application/json";
      www.SetRequestHeader("Accept", "text/xml");
# if UNITY_2017_2_OR_NEWER
      www.SendWebRequest();
# else
      www.Send();
# endif

      while (!www.isDone);

      if (www.isHttpError)
      {
         Log("Server http error for " + url);
         Log(www.error);
      }
      else if (www.isNetworkError)
      {
         Log("Server network error for " + url);
         Log(www.error);
      }
      else
      {
         string response = www.downloadHandler.text;
         if (response != null && response != "" && response != "<empty/>" && !response.StartsWith("<error>"))
         {
            try
            {
               License license = default(License);

               using (var stream = new StringReader(response))
               {
                  var serializer = new XmlSerializer(typeof(License));
                  license = (License)serializer.Deserialize(stream);
                  license.cookie = www.GetResponseHeader("SET-COOKIE");
               }

               return license;
            }
            catch (Exception ex)
            {
               Log("Deserialization error for " + url + " with " + response);
               Log(ex.Message);
               Log(ex.StackTrace);
            }
         }
      }

      return null;
   }
#endif

   protected virtual void Update()
   {
      // generates a delta time based on realtime
      VZTime.Update();

#if UNITY_PS4 && !UNITY_EDITOR
      if (UnityEngine.PS4.Utility.isInBackgroundExecution != mIsInBackgroundExecution)
      {
         mIsInBackgroundExecution = !mIsInBackgroundExecution;
         VZPlugin.PS4SetBackground(mIsInBackgroundExecution);
         mPS4RenderScale = 2; // to fix scale coming back from dash
      }

      // HACK still not fixed in Unity 5.4
      if (mPS4RenderScale != 0)
      {
         mPS4RenderScale--;
# if UNITY_2017_2_OR_NEWER
         UnityEngine.XR.XRSettings.eyeTextureResolutionScale = 1.4f;
# else
         UnityEngine.VR.VRSettings.renderScale = 1.4f;
# endif
      }
#endif

#if !(!UNITY_EDITOR && VZ_GEARVR)
      // Quit on ESCAPE if not GearVR
      if (Input.GetKey("escape"))
      {
# if VZ_GOOGLEVR && !UNITY_EDITOR
         VRSettingsEnabled(false);
# endif
         Application.Quit();
      }
#endif

#if UNITY_WSA_10_0 && !UNITY_EDITOR
      // The watchdog forces userPresence=true because it has to toy with Win-Y to get Portal focus, and we have 
      // to assume user is wearing headset or else VZButton won't work.  But that means we can't rely on userPresence
      // until we've seen it go NotPresent
      if (UnityEngine.XR.XRDevice.userPresence == UnityEngine.XR.UserPresenceState.NotPresent && Time.realtimeSinceStartup > 10)
         mWinMRSeenNotPresent = true;
#endif

#if VZ_ARCADE && !VZ_MOBILE
      // Get replay record
      if (VZReplay.Playback())
      {
         VZReplay.Record record = VZReplay.Instance.GetRecord();

         if (record != null)
         {
            InputSpeed = record.inputSpeed;

            LeftButton.Set(record.leftButton);
            RightButton.Set(record.rightButton);
            DpadUp.Set(record.dpadUp);
            DpadDown.Set(record.dpadDown);
            DpadLeft.Set(record.dpadLeft);
            DpadRight.Set(record.dpadRight);
            RightUp.Set(record.rightUp);
            RightDown.Set(record.rightDown);
            RightLeft.Set(record.rightLeft);
            RightRight.Set(record.rightRight);

            mCamera.localRotation = record.headRotation;
            mCamera.localPosition = record.headPosition;
         }
      }
      else
#endif
      {
         // Update speed and triggers
         bool received, leftButton, rightButton;

         if (mBikeState.Type > 0)
         {
            // Ignore speed for one second if we see larger than one second gap, to ignore build up of bluetooth packets
            if (Time.realtimeSinceStartup - mUpdateTime > 1)
            {
               if (mUpdateTime != 0)
                  mIgnoreTime = Time.realtimeSinceStartup + 1;
            }

            mUpdateTime = Time.realtimeSinceStartup;

            received = VZPlugin.UpdateBike(ref mBikeState, mUpdateTime);

            if (IsBikeConnected() && mLicenseState == LicenseState.Init)
            {
               SetBikeLicense();
            }

            if (Time.realtimeSinceStartup > mIgnoreTime)
            {
               InputSpeed = mBikeState.Speed;

               if (Mathf.Abs(InputSpeed) > 1 && (IsBikeSensor() || IsLifeCycle()))
               {
                  // Adjust for sensor and lifecycle (now using raw speed) because we found that bike InputSpeeds 
                  // were inflated, due to bias when averaging magnet counts over delta time when packet was received.  
                  // Variations in delta time cause higher averages according to
                  //
                  // 1 / (1 - d^2)
                  //
                  // where d is percent variation. To simulate bluetooth variation on bike, we calculate bike's
                  // a delta time of 1 / (8 * S) where S is revs/sec.  The empirically measured bluetooth variation
                  // is 28 ms.  
                  float S = InputSpeed / kMetersPerRev;
                  float d = 0.028f / (1 / (8*S));
                  InputSpeed *= (1 / (1 - d*d));
               }

               // Adjust speed on non-lifecycles due to difficulty.  
               // On lifecycles that translates into resistance so don't touch speed
               if (!IsLifeCycle())
                  InputSpeed *= (1 - ((mDifficulty - 5) * 0.1f));
            }

            if (!mBikeState.Connected || IsBikeSensor())
            {
               // Get from controller if vz sensor or not connected
               leftButton = mController.LeftTrigger.GetBool();
               rightButton = mController.RightTrigger.GetBool();
            }
            else
            {
               leftButton = mBikeState.LeftTrigger;
               rightButton = mBikeState.RightTrigger;
            }
         }
         else
         {
            // Read from joypad/keyboard if no serial
            InputSpeed = ControllerSpeed();

            received = false;
            leftButton = mController.LeftTrigger.GetBool();
            rightButton = mController.RightTrigger.GetBool();
         }

         // Initialize false, assuming LifeCycle connects right away and headset isn't on
         if (IsLifeCycle() && mInitializeHeadset)
         {
            mInitializeHeadset = false;
            VZPlugin.WearingHeadset(false); 
         }

         // Give subclass a crack before buttons are set
         InputFilter(received, ref leftButton, ref rightButton);

         LeftButton.Set(leftButton);
         RightButton.Set(rightButton);

         // Update face buttons
         bool rightLeft;
         bool rightUp;

#if !UNITY_PS4 || UNITY_EDITOR
         if (mBikeState.Type == (int)VZBikeType.Beta)
         {
            RightUp.Set(mBikeState.RightUp);
            RightDown.Set(mBikeState.RightDown);
            RightLeft.Set(mBikeState.RightLeft);
            RightRight.Set(mBikeState.RightRight);
            DpadUp.Set(mBikeState.DpadUp);
            DpadDown.Set(mBikeState.DpadDown);
            DpadLeft.Set(mBikeState.DpadLeft);
            DpadRight.Set(mBikeState.DpadRight);
         }
         else
#endif
         {
            RightUp.Set(mController.RightUp.GetBool());
            RightDown.Set(mController.RightDown.GetBool());
            RightLeft.Set(mController.RightLeft.GetBool());
            RightRight.Set(mController.RightRight.GetBool());
            DpadUp.Set(mController.LeftUp.GetBool());
            DpadDown.Set(mController.LeftDown.GetBool());
            DpadLeft.Set(mController.LeftLeft.GetBool());
            DpadRight.Set(mController.LeftRight.GetBool());
         }

         // Update camera controls without hmd
         if (!mHasHmd)
         {
            // Rotate head from controller
            float yaw = MakeAxis(mController.LookLeft, mController.LookRight, Mathf.Rad2Deg);
            float pitch = MakeAxis(mController.LookUp, mController.LookDown, Mathf.Rad2Deg * 2);

#if UNITY_EDITOR
            if (ControllerName() == "Keyboard" && Input.GetKey(KeyCode.LeftShift))
            {
               mControllerYaw += yaw / 270;
               mControllerPitch += pitch / 270;
            }
            else
#endif
            {
               float dt = Time.deltaTime;
               if (dt == 0.0f)
                  dt = VZTime.deltaTime;
               //mControllerYaw = Mathf.SmoothDamp(mControllerYaw, yaw, ref mControllerYawVel, 1);
               //mControllerPitch = Mathf.SmoothDamp(mControllerPitch, pitch, ref mControllerPitchVel, 1);
               mControllerYaw = Mathf.SmoothDamp(mControllerYaw, yaw, ref mControllerYawVel, 1, Mathf.Infinity, dt);
               mControllerPitch = Mathf.SmoothDamp(mControllerPitch, pitch, ref mControllerPitchVel, 1, Mathf.Infinity, dt);
            }

            mCamera.localEulerAngles = new Vector3(mControllerPitch, mControllerYaw, 0);

            // Translate head from controller
            float lean = MakeAxis(mController.LeanLeft, mController.LeanRight, 0.25f);
            float bend = MakeAxis(mController.LeanBack, mController.LeanForward, 0.25f);

            mControllerLean = Mathf.SmoothDamp(mControllerLean, lean, ref mControllerLeanVel, 0.5f);
            mControllerBend = Mathf.SmoothDamp(mControllerBend, bend, ref mControllerBendVel, 0.5f);

            if (TiltSteering)
            {
               Vector3 angs = mCamera.localEulerAngles;
               angs.z = -mControllerLean * Mathf.Rad2Deg;
               mCamera.localEulerAngles = angs;
            }
            else
            {
               mCamera.localPosition = new Vector3(mControllerLean, 0, mControllerBend);
            }
         }

         // Update whether user moved
         if (mHasHmd)
         {
            if (mHmdSampleTime == 0)
            {
               mHmdRot = mCamera.localEulerAngles;
               mHmdSampleTime = VZTime.time;
            }
            else if (VZTime.time > mHmdSampleTime + mMovedSample)
            {
               Vector3 delta = mCamera.localEulerAngles - mHmdRot;
               delta.x = WrapDegrees(delta.x);
               delta.y = WrapDegrees(delta.y);
               delta.z = WrapDegrees(delta.z);

               if (delta.sqrMagnitude > mMovedThresh * mMovedThresh)
               {
                  mHmdMovedTime = VZTime.time;
               }

               mHmdRot = mCamera.localEulerAngles;
               mHmdSampleTime = VZTime.time;
            }
         }
      }

      // Update Head from Camera and CameraOffset
      float ang = -mCameraOffset.localEulerAngles.y * Mathf.Deg2Rad;
      float sinAng = Mathf.Sin(ang);
      float cosAng = Mathf.Cos(ang);
      float x = -mCamera.localPosition.z * sinAng + mCamera.localPosition.x * cosAng + mCameraOffset.localPosition.x;
      float z = mCamera.localPosition.z * cosAng + mCamera.localPosition.x * sinAng + mCameraOffset.localPosition.z;
      float y = mCamera.localPosition.y + mCameraOffset.localPosition.y;

      Head.localPosition = new Vector3(x, y, z);

      Vector3 localAngles = mCamera.localEulerAngles;
      localAngles.y += mCameraOffset.localEulerAngles.y;
      Head.localEulerAngles = localAngles;

      // Update button data
      LeftButton.Update();
      RightButton.Update();
      DpadUp.Update();
      DpadDown.Update();
      DpadLeft.Update();
      DpadRight.Update();
      RightUp.Update();
      RightDown.Update();
      RightLeft.Update();
      RightRight.Update();

      // Get head rot from look direction
      Vector3 localForward = mNeck.InverseTransformDirection(Head.forward);
      HeadRot = -Mathf.Atan2(localForward.x, localForward.z);

      if (HeadRot > Mathf.PI)
         HeadRot -= Mathf.PI * 2;

      if (HeadRot < -Mathf.PI)
         HeadRot += Mathf.PI * 2;

      // Get head lean
      if (TiltSteering)
      {
         float roll = Head.localEulerAngles.z;
         if (roll > 180)
            roll -= 360;
         if (roll > 27)
            roll = 27;
         else if (roll < -27)
            roll = -27;
         HeadLean = roll / 2 * Mathf.Deg2Rad;
      }
      else
      {
         HeadLean = -Head.localPosition.x;
      }

      // Get head bend
      if (TiltSteering)
      {
         float yaw = WrapDegrees(Head.localEulerAngles.y);
         float pitch = WrapDegrees(Head.localEulerAngles.x);
         if (pitch < 10 || pitch > 40) // allow to look down at heli
            pitch = 0;
         if (yaw > 20 || yaw < -20)
            pitch = 0;
         mPitch = Mathf.SmoothDamp(mPitch, pitch, ref mPitchVelocity, mPitchSettle);
         HeadBend = mPitch / 2 * Mathf.Deg2Rad; // 45 degs equates to 0.2 m
      }
      else
      {
         HeadBend = Head.localPosition.z;
      }

      // Subtract rot from lean/bend if hmd (controller doesn't simulate head width)
      if (mHasHmd)
      {
         // Adjust lean
         if (Mathf.Abs(HeadLean) < kHeadDead)
         {
            HeadLean = 0;
         }
         else if (HeadLean > 0)
         {
            if (!TiltSteering)
            {
               if (HeadRot > Mathf.PI / 2.0f)
                  HeadLean -= kHeadWidth;
               else 
                  HeadLean -= kHeadWidth * Mathf.Sin(HeadRot); 
            }

            HeadLean -= kHeadDead;

            if (HeadLean < 0)
               HeadLean = 0;
         }
         else
         {
            if (!TiltSteering)
            {
               if (HeadRot < -Mathf.PI / 2.0f)
                  HeadLean += kHeadWidth;
               else 
                  HeadLean -= kHeadWidth * Mathf.Sin(HeadRot); 
            }

            HeadLean += kHeadDead;

            if (HeadLean > 0)
               HeadLean = 0;
         }

         // Adjust bend
         if (!TiltSteering)
         {
            float headPitch = Head.localEulerAngles.x * Mathf.Deg2Rad;

            if (headPitch < -Mathf.PI)
               headPitch += Mathf.PI * 2;

            HeadBend += kHeadWidth * (1 - Mathf.Cos(headPitch)); 
         }
      }

      // Update spins
      if (mBikeState.Type > 0)
      {
         if (mBikePulses >= 0)
         {
            int delta = BikePulses() - mBikePulses;

            if (delta < -128)
               delta += 256;
            else if (delta > 128)
               delta = 256;

            Spins += Mathf.Abs(delta) / 8.0f;
         }

         mBikePulses = BikePulses();
      }
      else
      {
         // Fake for controller
         Spins += InputSpeed / kMetersPerRev * VZTime.deltaTime;
      }
   }

   protected virtual void OnDestroy()
   {
#if !VZ_MOBILE || UNITY_EDITOR
      VZPlugin.CloseBike();
#endif
   }

   protected virtual void Awake()
   {
#if (UNITY_ANDROID && !VZ_SNAPDRAGONVR) || UNITY_IOS
# if VZ_GOOGLEVR
      TiltSteering = !GvrHeadset.SupportsPositionalTracking;
# elif VZ_GEARVR
      TiltSteering = (OVRPlugin.GetSystemHeadsetType() != OVRPlugin.SystemHeadset.Oculus_Quest);
# else
      TiltSteering = true;
# endif
#else
      TiltSteering = false;
#endif

      Spins = 0;

      // Neck/camera/head setup
      mNeck = transform.Find("Neck");
      mCameraOffset = mNeck.Find("CameraOffset");
      mCamera = mCameraOffset.Find("Camera");
      Head = mNeck.Find("Head");

      // Setup transition canvas
      mTransitionCanvas = mNeck.Find("Head/TransitionCanvas").gameObject;
      mTransitionCanvas.SetActive(true);

      mTransitionCanvasPos = mTransitionCanvas.transform.localPosition;
      mTransitionCanvasRot = mTransitionCanvas.transform.localRotation;
      mTransitionCanvasScale = mTransitionCanvas.transform.localScale;

      // SteamVR
      IsSteamVR = (VRSettingsLoadedDeviceName() == "OpenVR");

      // Init plugin
      VZPlugin.Init(Application.dataPath + Path.DirectorySeparatorChar + "Plugins");

      mIsInBackgroundExecution = false;

#if VZ_ARCADE || VZ_STREETVIEW
      // Add OVRManager for hand controls
# if !UNITY_EDITOR && VZ_GEARVR
      GameObject go = new GameObject();
      go.name = "OVRManager";
      go.AddComponent<OVRManager>();
      go.transform.SetParent(transform);
//      OVRManager.cpuLevel = 3;
//      OVRManager.gpuLevel = 3;
//      OVRManager.instance.enableAdaptiveResolution = true;
      OVRManager.tiledMultiResLevel = OVRManager.TiledMultiResLevel.LMSMedium;

      if (OVRPlugin.GetSystemHeadsetType() == OVRPlugin.SystemHeadset.Oculus_Quest)
         OVRManager.display.displayFrequency = 72;
# elif !UNITY_WSA_10_0 && UNITY_STANDALONE_WIN
      if (!IsSteamVR)
      {
         GameObject go = new GameObject();
         go.name = "OVRManager";
         go.AddComponent<OVRManager>();
         go.transform.SetParent(transform);
      }
# endif
#endif

      // Fix framerate
      Application.targetFrameRate = 0;
   }

   protected virtual void Start()
   {
      InputSpeed = 0;
      HeadRot = 0;
      HeadLean = 0;
      HeadBend = 0;

#if !UNITY_PS4 && VZ_ARCADE
      // Redirect Fmod to Rift headphones
      if (GameObject.Find("FMOD_StudioSystem") != null)
      {
         FMOD.System sys = FMODUnity.RuntimeManager.LowlevelSystem;
//         Log(sys);

         int num;
         sys.getNumDrivers(out num);
//         Log(num);

         for (int i = 0; i < num; ++i)
         {
            int namelen = 100;
            StringBuilder name = new StringBuilder(namelen);
            Guid guid;
            int systemrate;
            FMOD.SPEAKERMODE speakermode;
            int speakermodechannels;

            sys.getDriverInfo(i, name, namelen, out guid, out systemrate, out speakermode, out speakermodechannels);

//            Log(i + " " + name + " " + namelen + " " + guid + " " + systemrate + " " + speakermode + " " + speakermodechannels);

            if (name.ToString() == "Headphones (Rift Audio)")
            {
               sys.setDriver(i);
               Log("Redirecting Fmod audio to Rift headphones");
               break;
            }
         }
      }
#endif

      // Setup VR
      SetupVR();

      // Init bike state
      mBikeState.Init();

      // Pick initial controller
      PickController();

      // Log
      String hmd = "None";
      if (mHasHmd)
      {
         if (IsSteamVR)
            hmd = "SteamVR";
         else
         {
#if UNITY_PS4 && !UNITY_EDITOR
            hmd = "PSVR";
#elif UNITY_WSA_10_0
            hmd = "Windows MR";
#elif VZ_GOOGLEVR
            hmd = "GoogleVR";
#else
            hmd = "Oculus";
#endif
         }
      }
      Log(" Hmd:" + hmd + " Controller:" + mController.Name);
   }

   protected virtual void InputFilter(bool received, ref bool leftButton, ref bool rightButton)
   {
      // For subclass to act on received data and filter l/r buttons
   }

   //***********************************************************************
   // PRIVATE API
   //***********************************************************************

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

   // Empirically determined resistance translation
   readonly int[] kLifecycleResistance = new int[]
   {
      0, // vz bike level 1
      5,
      7, // half power (vz bike level 3)
      9,
      11, // nominal (vz bike level 5)
      13,
      15, // double power (vz bike level 7)
      17  // vz bike level 8
   };

   const float kSpeedThreshHi = 0.7f;
   const float kSpeedThreshLo = 0.3f;
   const float kSpeedHoldPeriod = 0.321f;
   const float kSpeedXboneFactor = 23.5f/25f; // xbone controller trigger is a little harder to press
   const float kCountsPerRev = 2f; // compared to 8 for bike magnets
   const float kControllerMaxSpeed = 12.0f;
   const float kHeadDead = 0.02f;
   const float kHeadWidth = 0.1f;

   bool mWinMRSeenNotPresent = false;
   float mMovedThresh = 0.8f;
   float mMovedSample = 0.5f;
   float mMovedPeriod = 3.1f; // little more than multiple of mMovedSample
   bool mInitializeHeadset = true;
   int mDifficulty = 5;
   Transform mCameraOffset;
   Transform mCamera;
   bool mRecentered = false;
   float mIgnoreTime = 0;
   float mUpdateTime = 0;
   float mHmdMovedTime = -999; // less than MovedPeriod
   float mHmdSampleTime = 0;
   Vector3 mHmdRot = Vector3.zero;
   Vector3 mTransitionCanvasPos;
   Quaternion mTransitionCanvasRot;
   Vector3 mTransitionCanvasScale;
   bool mIsInBackgroundExecution = false;
   float mControllerPitch = 0;
   float mControllerYaw = 0;
   float mControllerLean = 0;
   float mControllerBend = 0;
   float mControllerPitchVel = 0;
   float mControllerYawVel = 0;
   float mControllerLeanVel = 0;
   float mControllerBendVel = 0;
   float mControllerSpeed = 0;
   bool mHasHmd;
   GameObject mTransitionCanvas;
   Transform mNeck;
   float mPitch = 0;
   float mPitchVelocity = 0;
   float mPitchSettle = 1;
   int mBikePulses = -1;
   LicenseState mLicenseState = LicenseState.Init;
   string mLicenseOwner;

   float WrapDegrees(float deg)
   {
      while (deg > 180)
         deg -= 360;
      while (deg < -180)
         deg += 360;
      return deg;
   }

   DateTime ParseServerDate(string date)
   {
      // UTC in Euro format 
      // Strip utc notation, since we should be doing with DateKind.Unspecified, because Unity implementation of .NET doesn't recognize timezones
      string stripped = date.Replace("+00:00", "");
      DateTime dt;
      
      try
      {
         dt = DateTime.Parse(stripped);
      }
      catch (Exception e)
      {
         Debug.LogError("Invalid date format: " + date + " (" + stripped + ")");
         dt = new DateTime();
      }

      return dt;
   }

   void Log(string msg)
   {
#if VZ_ARCADE || VZ_STREETVIEW
      VZUtl.Log(msg);
#else
      Debug.Log(msg);
#endif
   }

#if UNITY_PS4 && !UNITY_EDITOR
   int mPS4RenderScale = 2;

   void PSVRCompleted(DialogStatus status, DialogResult result)
   {
      Log("PSVR Completed " + status + " " + result);

      if (result != DialogResult.OK)
      {
         HmdSetupDialog.OpenAsync(0, PSVRCompleted); 
         return;
      }

      StartCoroutine(SetupPSVR());
   }

   IEnumerator SetupPSVR()
   {
      PlayStationVRSettings.reprojectionSyncType = PlayStationVRReprojectionSyncType.ReprojectionSyncVsync;
      PlayStationVRSettings.reprojectionFrameDeltaType = PlayStationVRReprojectionFrameDeltaType.UnityCameraAndHeadRotation;
      PlayStationVRSettings.minOutputColor = new Color(0.006f, 0.006f, 0.006f);

#if UNITY_2017_2_OR_NEWER
      UnityEngine.XR.XRSettings.LoadDeviceByName("PlayStationVR");
#else
      UnityEngine.VR.VRSettings.LoadDeviceByName("PlayStationVR");
#endif
      yield return null; // HACK in Unity example to wait a frame

#if UNITY_2017_2_OR_NEWER
      UnityEngine.XR.XRSettings.showDeviceView = true;

      // Delay setting scale
      UnityEngine.XR.XRSettings.eyeTextureResolutionScale = 1.4f;

      UnityEngine.XR.XRSettings.enabled = true;
#else
      UnityEngine.VR.VRSettings.showDeviceView = true;
      UnityEngine.VR.VRSettings.renderScale = 1.4f;
      UnityEngine.VR.VRSettings.enabled = true;
#endif

      Application.backgroundLoadingPriority = ThreadPriority.BelowNormal;

      mHasHmd = true;
      mPS4RenderScale = 2;
   }
#endif

   void SetupVR()
   {
      Debug.Log("SetupVR " + VRSettingsLoadedDeviceName() + " " + VRDeviceIsPresent());

      // Setup hmd
#if VZ_ARCADE && !VZ_MOBILE
      if (VZReplay.Playback())
      {
         mHasHmd = false;
         VRSettingsEnabled(false);
      }
      else
#endif
      {
#if UNITY_WSA_10_0
         // Seated scale and render scale
         VRDeviceSetTrackingSpaceStationary();
#endif

#if UNITY_PS4 && !UNITY_EDITOR
         mHasHmd = false;

# if UNITY_2017_2_OR_NEWER
         if (UnityEngine.XR.XRSettings.enabled)
# else
         if (UnityEngine.VR.VRSettings.enabled)
# endif
            StartCoroutine(SetupPSVR());
         else
            HmdSetupDialog.OpenAsync(0, PSVRCompleted); 
#elif VZ_SNAPDRAGONVR
# if UNITY_EDITOR
         mHasHmd = false;
# else
         mHasHmd = true;
# endif
#else
         mHasHmd = VRDeviceIsPresent() || IsCardboard();
         VRSettingsEnabled(mHasHmd);
#endif
      }
   }

   float ControllerSpeed()
   { 
      float axis = MakeAxis(mController.Reverse, mController.Forward, 1);
      return axis * kControllerMaxSpeed;
   }

   // Turn opposing control inputs into single axis
   float MakeAxis(VZControllerMap.Control negative, VZControllerMap.Control positive, float multiplier)
   {
      float pos = positive.GetFloat();
      float neg = negative.GetFloat();
      float axis = 0;

      if (pos > neg)
         axis = pos;
      else
         axis = -neg;

      return axis * multiplier;
   }

   // Deal with Unity changing VR to XR namespace
   void VRSettingsEnabled(bool enable)
   {
#if UNITY_2017_2_OR_NEWER
      UnityEngine.XR.XRSettings.enabled = enable;
#else
      UnityEngine.VR.VRSettings.enabled = enable;
#endif
   }

   void VRDeviceSetTrackingSpaceStationary()
   {
#if UNITY_2017_2_OR_NEWER
      UnityEngine.XR.XRDevice.SetTrackingSpaceType(UnityEngine.XR.TrackingSpaceType.Stationary);
#else
      UnityEngine.VR.VRDevice.SetTrackingSpaceType(UnityEngine.VR.TrackingSpaceType.Stationary);
#endif
   }

   string VRSettingsLoadedDeviceName()
   {
#if UNITY_2017_2_OR_NEWER
      return UnityEngine.XR.XRSettings.loadedDeviceName;
#else
      return UnityEngine.VR.VRSettings.loadedDeviceName; 
#endif
   }

   string[] VRSettingsSupportedDevices()
   {
#if UNITY_2017_2_OR_NEWER
      return UnityEngine.XR.XRSettings.supportedDevices;
#else
      return UnityEngine.VR.VRSettings.supportedDevices;
#endif
   }

   void VRSettingsLoadDeviceByName(string device)
   {
#if UNITY_2017_2_OR_NEWER
      UnityEngine.XR.XRSettings.LoadDeviceByName(device);
#else
      UnityEngine.VR.VRSettings.LoadDeviceByName(device);
#endif
   }

   bool VRDeviceIsPresent()
   {
#if UNITY_2017_2_OR_NEWER
      return UnityEngine.XR.XRDevice.isPresent;
#else
      return UnityEngine.VR.VRDevice.isPresent;
#endif
   }
}

