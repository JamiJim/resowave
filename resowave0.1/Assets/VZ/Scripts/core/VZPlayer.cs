//***********************************************************************
// Copyright 2014 VirZOOM
//***********************************************************************

#if VZ_ARCADE
# define VZ_PLAYMAKER
#endif

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
#endif

#if VZ_PLAYMAKER
using HutongGames.PlayMaker;
#endif

public class VZPlayer : MonoBehaviour
{
   //***********************************************************************
   // PUBLIC API
   //***********************************************************************

   public float NearClipPlane = 0.2f;
   public float FarClipPlane = 3000;
   public float DraftSpeed = 0;
   public float DraftFactor = 0;
   public float SpeedFudge = 1.0f;
   public float DownhillFactor = 0.5f;
   public float UphillFactor = 1.414f;
   public float MaxVertSpeed = 4.0f;
   public float MaxTurn = 0.5f;
   public float MaxSpeed = 12.0f;
   public float LeanFudge = 2.0f;
   public float LeanIn = 0.5f;
   public float LandingHardness = 2.0f;
   public float LandingRadius = 0.25f;
   public float NeckHeight = 0.0f;
   public bool AllowRotate = true;
   public bool AllowPitch = true;
   public bool AllowRoll = false;
   public bool AllowDrift = true;
   public bool RotateAtStop = false;
   public GameObject BodyPrefab;
   public float SlowRotateLimit = 0.0f; // amount of turn at zero speed
   public bool AllowReverse = true;
   public bool ForceReverse = false;
   public bool ResistanceAdjust = false;

   [HideInInspector]
   public string BuildDate;

   public static VZPlayer Instance { get; private set; }
   public static VZController Controller { get; private set; }

#if VZ_ARCADE || VZ_STREETVIEW
   public static VZServer Server { get; private set; }
#endif

#if UNITY_EDITOR
   [PostProcessScene(-2)]
   public static void OnPostProcessScene()
   {
      // Put build time into each VZPlayer before it's saved for build/play
      VZPlayer instance = FindObjectOfType<VZPlayer>();
      if (instance != null)
         instance.BuildDate = DateTime.UtcNow.ToString();
   }
#endif

   public void Initialize(Vector3 position, Quaternion rotation)
   {
      if (mRigidbody != null)
      {
         mRigidbody.MovePosition(position);
         mRigidbody.MoveRotation(rotation);
      }

      transform.position = position;
      transform.rotation = rotation;

      mBodyRot = Mathf.Atan2(transform.forward.z, transform.forward.x);

      UpdateNeck();

      mInitialPos = transform.position;
      mInitialRot = Mathf.Atan2(transform.forward.z, transform.forward.x);
      mLiftOffHeight = transform.position.y;
      mColliding = false;
   }

   public int RaycastMask()
   {
      return mRaycastMask;
   }

   public float RotVel()
   {
      return mRotVel;
   }

   public float BodyRot()
   {
      return mBodyRot;
   }

   public float Altitude()
   {
      if (mColliding)
         return 0;
      else
         return transform.position.y - mLiftOffHeight;
   }

   public bool Colliding()
   {
      return mColliding;
   }

   public float Speed()
   {
      return mSpeed;
   }

   public float Lean()
   {
      return mLean;
   }

   public float HeadYaw()
   {
      Vector3 forward = Controller.Head.TransformDirection(Vector3.forward);

      return Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
   }

   public float Turn()
   {
      return mTurn;
   }

   public float Scale()
   {
      // Internal values were tuned for actual scale of 0.5
      return transform.localScale.x * 2.0f;
   }

   public bool NormalMode()
   {
      return State() == kStateNormal;
   }

   public void SetDifficulty(int difficulty)
   {
      Controller.SetDifficulty(difficulty);
   }

   public int GetDifficulty()
   {
      return Controller.GetDifficulty();
   }

   //***********************************************************************
   // PROTECTED API
   //***********************************************************************

   protected const string kStateSetup = "Setup";
   protected const string kStateNormal = "Normal";

   protected float mInitialRot;
   protected int mRaycastMask;
   protected VZColliderInfo mColliderInfo = null;
   protected bool mIsQuitting = false;
   protected VZMotionInput mInput = new VZMotionInput();
   protected string mPrevState = "";

   protected virtual float SpeedMultiplier()
   {
      return SpeedFudge;
   }

   protected string State()
   {
      return mState;
   }

   protected Rigidbody GetRigidbody()
   {
      return mRigidbody;
   }

   protected void SetState(string state)
   {
#if (VZ_ARCADE || VZ_STREETVIEW) && !VZ_SHIP
      VZUtl.Log("SetState: from " + mState + " to " + state);
#endif
      mPrevState = mState;
      mState = state;
   }

   protected virtual void Awake()
   {
      Instance = this;

      // If no rigidbody make one
      mRigidbody = GetComponent<Rigidbody>();

      if (mRigidbody == null)
         mRigidbody = gameObject.AddComponent<Rigidbody>();

      // Instantiate controller prefab
      if (Controller == null)
      {
         GameObject go = Instantiate(BodyPrefab, Vector3.zero, Quaternion.identity) as GameObject;
         Controller = go.GetComponent<VZController>();
         Controller.transform.localScale = Vector3.one;
         Controller.name = "VZController";
         DontDestroyOnLoad(Controller);

#if VZ_ARCADE || VZ_STREETVIEW
         Server = new VZServer();
#endif
      }

      // attach controller to us
      Controller.AttachPlayer(this);

      // Raycast mask
      mRaycastMask = ~(LayerMask.GetMask("VZPlayerCollider") | LayerMask.GetMask("Ignore Raycast") | LayerMask.GetMask("VZObjectCollider") | LayerMask.GetMask("UI"));

#if VZ_PLAYMAKER
      // Cache PlayMaker FSM variable references.
      mPlayMakerVariables.Init();
#endif
   }

   protected virtual void Start()
   {
      mInput.OnMenu = false;

      // Transition canvas
      Controller.TransitionCanvas().GetComponent<Canvas>().enabled = true;

      // Offset raycast for terrain
      Transform raycastOrigin = transform.Find("RaycastOrigin");
      if (raycastOrigin != null)
      {
         mRaycastOffset = raycastOrigin.localPosition;
      }

      // Init neck height
      Controller.Neck().localPosition = new Vector3(0, NeckHeight, 0);

      // Initialize rot and pos
      Initialize(transform.position, transform.rotation);

      mRigidbody.freezeRotation = true;

      // Either calibrate or fade down now
      if (sFirstTime)
      {
         sFirstTime = false;

         // Goto initial state
         SetState(kStateSetup);
      }
      else
         StartLevel();
   }

   protected virtual void StartLevel()
   {
      Restart(true);
   }

   // Update rotation here to avoid blurring fixed objects in oculus
	protected virtual void Update()
   {
#if VZ_ARCADE || VZ_STREETVIEW
      Server.Update();
#endif

      // Update based on state
      if (State() == kStateSetup)
      {
         UpdateSetup();
      }
      else if (State() == kStateNormal)
      {
         UpdateNormal();
      }

#if VZ_PLAYMAKER
      // Update PlayMaker variables.
# if VZ_ARCADE
      if (VZGamePlayer.Instance.State() == kStateSetup || !VZGamePlayer.Instance.IsTransitioning())
# endif
         mPlayMakerVariables.Update();
#endif
   }

   protected virtual void OnDestroy()
   {
      // Reset globals for next play mode
      if (mIsQuitting)
      {
         Destroy(Controller);
         Controller = null;
         sFirstTime = true;
      }
   }

   protected virtual void OnCollisionEnter(Collision collision)
   {
      mColliding = true;
   }

   protected virtual void OnCollisionStay(Collision collision)
   {
      mColliding = true;
   }

   protected virtual void OnCollisionExit(Collision collision)
   {
      mColliding = false;
      mLiftOffHeight = transform.position.y;
   }

   protected virtual void Restart(bool initLevel)
   {
      if (initLevel)
      {
#if VZ_PLAYMAKER
         PlayMakerFSM.BroadcastEvent("restart_game");
#endif
      }

      // Reset controller
      Controller.Restart();

      // Reset initial position and rotation
      transform.position = mInitialPos;
      mRigidbody.velocity = Vector3.zero;
      mBodyRot = mInitialRot;

      VZPlugin.ResetMotion();

      SetState(kStateNormal);
   }

   protected virtual void LandingSmoothed()
   {
      // TODO for subclass to use
   }

   protected virtual void UpdateNormal()
   {
      // Fill input
      mInput.DraftSpeed = DraftSpeed;
      mInput.DraftFactor = DraftFactor;
      mInput.DownhillFactor = DownhillFactor;
      mInput.UphillFactor = UphillFactor;
      mInput.ControllerHeadLean = Controller.HeadLean;
      mInput.LeanFudge = LeanFudge;
      mInput.ApparentLean = LeanIn;
      mInput.MaxTurn = MaxTurn;
      mInput.StoppedTurnFraction = SlowRotateLimit;
      mInput.AllowDrift = AllowDrift;
      mInput.AllowYaw = AllowRotate;
      mInput.AllowPitch = AllowPitch;
      mInput.AllowRoll = AllowRoll;
      mInput.RotateAtStop = RotateAtStop;
      mInput.ControllerHeadRot = Controller.HeadRot;
      mInput.Colliding = mColliding;
      mInput.BodyRot = mBodyRot;
      mInput.Scale = Scale();
      mInput.LandingHardness = LandingHardness;
      mInput.LandingRadius = LandingRadius;
      mInput.ControllerInputSpeed = Controller.InputSpeed;
      mInput.MaxSpeed = MaxSpeed;
      mInput.MaxVertSpeed = MaxVertSpeed;
      mInput.AllowReverse = AllowReverse;
      mInput.ForceReverse = ForceReverse;
      mInput.DeltaTime = Time.deltaTime;
      mInput.PlayerX = transform.position.x;
      mInput.PlayerY = transform.position.y;
      mInput.PlayerZ = transform.position.z;
      mInput.ResistanceAdjust = ResistanceAdjust;

      Vector3 velocity = mRigidbody.velocity;
      mInput.VelocityX = velocity.x;
      mInput.VelocityY = velocity.y;
      mInput.VelocityZ = velocity.z;

      Vector3 rayOrigin = transform.position + transform.TransformVector(mRaycastOffset);
      Vector3 rayDir = Vector3.down;
      RaycastHit hit;
      mInput.RaycastResult = Physics.Raycast(rayOrigin, rayDir, out hit, 1000.0f, mRaycastMask);

      if (mInput.RaycastResult)
      {
         mColliderInfo = hit.collider.GetComponent<VZColliderInfo>();
         mInput.HitDistance = hit.distance;
         mInput.HitNormalX = hit.normal.x;
         mInput.HitNormalY = hit.normal.y;
         mInput.HitNormalZ = hit.normal.z;
      }
      else
      {
         mColliderInfo = null;
      }

      // Set speed multiplier
      mInput.SpeedMultiplier = SpeedMultiplier();

      if (mColliderInfo != null)
         mInput.SpeedMultiplier *= mColliderInfo.SpeedMultiplier;

      // Move state
      VZPlugin.UpdateMotion(ref mInput, ref mOutput);

      // Set resistance on lifecycle
      if (Controller.IsLifeCycle())
      {
         /* Convert difficulty to experimentally determined lifefitness resistance
         0, // vz bike level 1
         5,
         7, // half power (vz bike level 3)
         9,
         11, // nominal (vz bike level 5)
         13,
         15, // double power (vz bike level 7)
         17  // vz bike level 8
         */
         float resistance = 3 + 2 * (Controller.GetDifficulty() - 1) * mOutput.ResistanceAdjust;

         if (resistance < 0)
            resistance = 0;
         else if (resistance > 17)
            resistance = 17;

         VZPlugin.SetResistance((byte)resistance);
      }

      // Handle output
      mBodyRot = mOutput.BodyRot;
      mRotVel = mOutput.RotVel;
      mTurn = mOutput.Turn;
      mSpeed = mOutput.Speed;
      mLean = mOutput.Lean;

      if (mOutput.LandingSmoothed)
         LandingSmoothed();

      transform.eulerAngles = new Vector3(mOutput.TransformEulerX, mOutput.TransformEulerY, mOutput.TransformEulerZ);

      transform.position = new Vector3(mOutput.PlayerX, mOutput.PlayerY, mOutput.PlayerZ);

      // Update velocity
      mRigidbody.velocity = new Vector3(mOutput.VelocityX, mOutput.VelocityY, mOutput.VelocityZ);

#if VZ_ARCADE && !VZ_MOBILE
      // Override with replay
      if (VZReplay.Playback())
      {
         VZReplay.Record record = VZReplay.Instance.GetRecord();

         if (record != null)
         {
            mRigidbody.velocity = record.playerVelocity;
            mBodyRot = record.bodyRot;

            if (VZReplay.Instance.OverridePlayer)
            {
               transform.position = record.playerPosition;
               transform.rotation = record.playerRotation;
            }
         }
      }
#endif

      // Keep neck vertical
      UpdateNeck();
   }

   protected Vector3 InitialPos()
   {
      return mInitialPos; 
   }

   protected float InitialRot()
   {
      return mInitialRot;
   }

   protected virtual void OnApplicationQuit()
   {
      mIsQuitting = true;
   }

   protected virtual void UpdateNeck()
   {
      float rotY = -mBodyRot * Mathf.Rad2Deg + 90;
      Controller.Neck().eulerAngles = new Vector3(0, rotY, 0);

      Controller.Neck().localPosition = new Vector3(0, NeckHeight, 0);
   }

   // Your app should subclass VZPlayer and override this function
   // for custom setup
   protected virtual void UpdateSetup()
   {
      // Lookup release screen
      Text release = Controller.TransitionCanvas().transform.Find("Release").GetComponent<Text>();

      // Update calibration msg
      release.text = "Build: " + BuildDate + "\n\n";
      release.text += kReleaseText + "\n\n";

      // Require bike outside of editor
      if (Controller.NeedsDongleDriver())
      {
         release.text += "PLEASE SEE VIRZOOM.COM/DRIVERS";
      }
      else if (!Controller.HasBikeDongle())
      {
#if UNITY_ANDROID
         release.text += "CHECKING PERMISSIONS";
#else
         release.text += "PLUG IN DONGLE";
#endif
         Controller.ConnectBike(true, true, true);
      }
      else if (Controller.IsUnsupportedBike())
      {
#if UNITY_ANDROID
         release.text += "DENIED PERMISSIONS";
#else
         release.text += "UNSUPPORTED BIKE";
#endif
      }
      else if (!Controller.IsBikeConnected())
      {
         if (Controller.BikeReprogramProgress() > 0 && Controller.BikeReprogramProgress() < 1.0f)
         {
            release.text += "UPDATING FIRMWARE " + (int)(Controller.BikeReprogramProgress() * 100) + "%";
         }
         else
         {
            release.text += "TURN ON BIKE AND PEDAL FORWARD";
         }
      }
      else if (!Controller.IsBikeLicensed())
      {
         var msg = Controller.LicenseStatus().ToUpper();

         if (Controller.LicenseUnregistered())
         {
            msg = "Register your sensor kit (" + Controller.BikeSender() + ") at vzfit.com/account";
            
         }

         release.text += msg;
      }
      else
      {
         release.text += "PRESS R TO BEGIN";
      }

#if UNITY_EDITOR
      if (!Controller.IsBikeConnected())
      {
         if (Controller.ControllerName() == "Keyboard")
            release.text += "\n\n(or press enter to play without bike)";
         else if (Controller.IsCardboard())
            release.text += "\n\n(or press button to play without bike)";
         else if (Controller.ControllerName() == "GearVR")
            release.text += "\n\n(or press touchpad to play without bike)";
         else if (Controller.ControllerName() == "Gvr")
            release.text += "\n\n(or press touchpad to play without bike)";
         else
            release.text += "\n\n(or press R1 to play without bike)";
      }
#endif

      // Switch to VZButton
      if (Controller.IsBikeSensor() && Controller.ControllerName() != "VZButton")
         Controller.PickController();

      // Hold both buttons to calibrate
      bool pressed = Controller.RightButton.Released();

      if (pressed && Controller.IsHeadTracked())
      {
         Controller.LeftButton.Clear();
         Controller.RightButton.Clear();

         if (!Controller.IsBikeConnected())
            Controller.CloseBike();

         Controller.Recenter();
         Restart(true);
         StartCoroutine(FadeDown(1));
      }
   }

   protected virtual IEnumerator FadeDown(float fadeTime)
   {
      // Fade alpha down to zero
      CanvasGroup group = Controller.TransitionCanvas().GetComponent<CanvasGroup>();
      float time = 0;
      float startAlpha = group.alpha;

      while (time < fadeTime)
      {
         time += VZTime.deltaTime > 0.0f ? VZTime.deltaTime : (1.0f / 60.0f);
         float alpha = Mathf.SmoothStep(startAlpha, 0.0f, time / fadeTime);
         group.alpha = alpha;
         yield return null;
      }

      // Deactivate and reset alpha
      Controller.TransitionCanvas().SetActive(false);
      group.alpha = 1.0f;
   }

   //***********************************************************************
   // PRIVATE API
   //***********************************************************************

#if VZ_PLAYMAKER
   // PlayMaker FSM variables.
   class PlayMakerVariables
   {
      public FsmFloat ControllerSpins;
      public FsmVector3 ControllerHeadForward;
      public FsmVector3 ControllerHeadPosition;
      public FsmFloat ControllerHeadBend;
      public FsmFloat ControllerHeadLean;
      public FsmFloat ControllerHeadRot;
      public FsmFloat ControllerInputSpeed;
      public FsmBool ControllerLeftButtonDown;
      public FsmBool ControllerLeftButtonPressed;
      public FsmBool ControllerLeftButtonReleased;
      public FsmBool ControllerRightButtonDown;
      public FsmBool ControllerRightButtonPressed;
      public FsmBool ControllerRightButtonReleased;
      public FsmBool ControllerIsPS4;
      public FsmGameObject Player;
      public FsmFloat PlayerSpeed;

      public int Hold = 0;

      public void Init()
      {
         ControllerSpins = FsmVariables.GlobalVariables.FindFsmFloat("VZController.Spins");
         ControllerHeadForward = FsmVariables.GlobalVariables.FindFsmVector3("VZController.Head.Forward");
         ControllerHeadPosition = FsmVariables.GlobalVariables.FindFsmVector3("VZController.Head.Position");
         ControllerHeadBend = FsmVariables.GlobalVariables.FindFsmFloat("VZController.Head.Bend");
         ControllerHeadLean = FsmVariables.GlobalVariables.FindFsmFloat("VZController.Head.Lean");
         ControllerHeadRot = FsmVariables.GlobalVariables.FindFsmFloat("VZController.Head.Rot");
         ControllerInputSpeed = FsmVariables.GlobalVariables.FindFsmFloat("VZController.InputSpeed");
         ControllerLeftButtonDown = FsmVariables.GlobalVariables.FindFsmBool("VZController.LeftButton.Down");
         ControllerLeftButtonPressed = FsmVariables.GlobalVariables.FindFsmBool("VZController.LeftButton.Pressed");
         ControllerLeftButtonReleased = FsmVariables.GlobalVariables.FindFsmBool("VZController.LeftButton.Released");
         ControllerRightButtonDown = FsmVariables.GlobalVariables.FindFsmBool("VZController.RightButton.Down");
         ControllerRightButtonPressed = FsmVariables.GlobalVariables.FindFsmBool("VZController.RightButton.Pressed");
         ControllerRightButtonReleased = FsmVariables.GlobalVariables.FindFsmBool("VZController.RightButton.Released");
         ControllerIsPS4 = FsmVariables.GlobalVariables.FindFsmBool("VZController.IsPS4");

# if UNITY_PS4 && !UNITY_EDITOR
         ControllerIsPS4.Value = true;
# else
         ControllerIsPS4.Value = false;
# endif

         Player = FsmVariables.GlobalVariables.FindFsmGameObject("VZPlayer");
         PlayerSpeed = FsmVariables.GlobalVariables.FindFsmFloat("VZPlayer.Speed");

         Player.Value = Instance.gameObject;
      }

      public void Update()
      {
         ControllerSpins.Value = Controller.Spins;
         ControllerHeadForward.Value = Controller.Head.forward;
         ControllerHeadPosition.Value = Controller.Head.position;
         ControllerHeadBend.Value = Controller.HeadBend;
         ControllerHeadLean.Value = Instance.Lean();
         ControllerHeadRot.Value = Controller.HeadRot * Mathf.Rad2Deg;
         ControllerInputSpeed.Value = Controller.InputSpeed;
         ControllerLeftButtonDown.Value = Controller.LeftButton.Down;
         ControllerLeftButtonPressed.Value = Controller.LeftButton.Pressed();
         ControllerLeftButtonReleased.Value = Controller.LeftButton.Released();
         ControllerRightButtonDown.Value = Controller.RightButton.Down;
         ControllerRightButtonPressed.Value = Controller.RightButton.Pressed();
         ControllerRightButtonReleased.Value = Controller.RightButton.Released();

         PlayerSpeed.Value = Instance.Speed() * Instance.Scale();
      }
   }

   PlayMakerVariables mPlayMakerVariables = new PlayMakerVariables();
#endif

   const string kReleaseText = "By continuing use of VZfit you agree to the License Agreement at virzoom.com/eula.htm";

   bool mColliding = false;
   float mTurn = 0;
   float mLean = 0;
   float mSpeed = 0;
   float mRotVel = 0;
   VZMotionOutput mOutput = new VZMotionOutput();
   Vector3 mInitialPos;
   float mLiftOffHeight;
   float mBodyRot = 0;
   Vector3 mRaycastOffset = new Vector3(0, 1, 0);
   string mState = "";
   Rigidbody mRigidbody;

   static bool sFirstTime = true;
}

