using UnityEngine;
using System;

public sealed class VZTime
{
   public VZTime()
   {
      time = deltaTime = 0.0f;
   }

   public static float time { get; private set; }
   public static float deltaTime { get; private set; }

   static public void Update()
   {
      float t = Time.realtimeSinceStartup;

      if (time != 0.0f)
         deltaTime = t - time;

      if (deltaTime > 0.1f)
         deltaTime = 0.1f;

      time = t;
   }
}

