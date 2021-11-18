using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public static class GrabOperations
{
  /**

  Flow:
  GrabbingAbility
  
  AwaitingGrabbing
  GrabbingIntent 
  Grabbing
  
  Flow 1:
  ReleaseGrabbingIntent
  AwaitingGrabbing
  
  Flow 2:
  AttachGrabbingIntent
  AwaitingGrabbing
  
  **/

  public enum GrabState
  {
    None,
    AwaitingGrabbedAndGrabbing,
    AwaitingGrabbing,
    GrabbingIntent,
    Grabbing,
    ReleaseGrabbingIntent,
    AttachGrabbingIntent,
    AwaitingGrabbed,
    GrabbedIntent,
    Grabbed,
    ReleaseGrabbedIntent,
    AttachGrabbedIntent,
  }

  public static void LogMethodCall(params object[] callingMethodParamValues)
  {

    var method = new StackFrame(skipFrames: 1).GetMethod();
    var methodParams = method.GetParameters();
    var methodCalledBy = new StackFrame(skipFrames: 2).GetMethod();
#if UNITY_EDITOR
    var methodCaller = "";
    if (methodCalledBy != null)
    {
      methodCaller = $"{methodCalledBy.DeclaringType.Name}.{methodCalledBy.Name}()";
    }

    if (methodParams.Length == callingMethodParamValues.Length)
    {
      List<string> paramList = new List<string>();
      foreach (var param in methodParams)
      {
        paramList.Add($"{param.Name}={callingMethodParamValues[param.Position]}");
      }

      UnityEngine.Debug.Log($"{method.Name}, {string.Join(", ", paramList)}, {methodCaller}");

    }
    else
    {
      UnityEngine.Debug.Log($"{method.Name}, \"/* Please update to pass in all parameters */\", {methodCaller}");
    }
#endif
  }

  public static bool CanPerformGrab(GameObject gameObject) => gameObject.HasComponent<AwaitingGrabbing>();
  public static bool CanGebGrabbed(GameObject gameObject) => gameObject.HasComponent<AwaitingGrabbed>();

  /// <summary>
  /// Grabbing can grab grabbed
  /// </summary>
  /// <param name="grabbing"></param>
  /// <param name="grabbed"></param>
  /// <returns></returns>
  public static bool ValidateGrabbingPair(GameObject grabbing, GameObject grabbed) =>
    CanPerformGrab(grabbing) && CanGebGrabbed(grabbed);

  public static bool TryGetGrabbingPair(
    GameObject grabbing,
    GameObject grabbed,
    out AwaitingGrabbing awaitingGrabbing,
    out AwaitingGrabbed awaitingGrabbed)
  {
    var canPerformGrab = grabbing.TryGetComponent(out awaitingGrabbing);
    var canBeGrabbed = grabbed.TryGetComponent(out awaitingGrabbed);
    return canPerformGrab && canBeGrabbed;
  }

  public static void AddAbilities(GameObject gameObject, bool grabbing, bool grabbed)
  {
    RemoveAbility(gameObject);

    if (grabbing)
    {
      gameObject.AddIfNotExisting<GrabbingAbility>();
      gameObject.AddIfNotExisting<AwaitingGrabbing>();
    }
    else
    {
      gameObject.RemoveComponent<GrabbingAbility>();
      gameObject.RemoveComponent<AwaitingGrabbing>();
    }

    if (grabbed)
    {
      gameObject.AddIfNotExisting<GrabbedAbility>();
      gameObject.AddIfNotExisting<AwaitingGrabbed>();
    }
    else
    {
      gameObject.RemoveComponent<GrabbedAbility>();
      gameObject.RemoveComponent<AwaitingGrabbed>();

    }
  }

  public static void RemoveAbility(GameObject gameObject)
  {
    gameObject.RemoveComponent<GrabbingIntent>().Map(component => component.other.Remove());
    gameObject.RemoveComponent<GrabbedIntent>().Map(component => component.other.Remove());
    gameObject.RemoveComponent<Grabbing>().Map(component => component.other.Remove());
    gameObject.RemoveComponent<Grabbed>().Map(component => component.other.Remove());
    gameObject.RemoveComponent<ReleaseGrabbingIntent>().Map(component => component.other.Remove());
    gameObject.RemoveComponent<ReleaseGrabbedIntent>().Map(component => component.other.Remove());
    gameObject.RemoveComponent<AttachGrabbingIntent>().Map(component => component.other.Remove());
    gameObject.RemoveComponent<AttachGrabbedIntent>().Map(component => component.other.Remove());
  }

  // On first touch
  public static Tuple<GrabbingIntent, GrabbedIntent> AwaitingToGrabIntent(
    AwaitingGrabbing awaitingGrabbing,
    AwaitingGrabbed awaitingGrabbed,
    Func<bool> isColliding)
  {
    LogMethodCall(awaitingGrabbing, awaitingGrabbed, isColliding);

    var grabbingGameObject = awaitingGrabbing.gameObject;
    var grabbedGameObject = awaitingGrabbed.gameObject;


    awaitingGrabbing.Remove();
    awaitingGrabbed.Remove();

    var grabbingIntent = grabbingGameObject.AddIfNotExisting<GrabbingIntent>();
    var grabbedIntent = grabbedGameObject.AddIfNotExisting<GrabbedIntent>();

    grabbingIntent.other = grabbedIntent;
    grabbingIntent.isColliding = isColliding;

    grabbedIntent.other = grabbingIntent;

    return new Tuple<GrabbingIntent, GrabbedIntent>(grabbingIntent, grabbedIntent);
  }

  // touch is released
  public static Tuple<AwaitingGrabbing, AwaitingGrabbed> CancelGrabIntent(
    GrabbingIntent grabbingIntent,
    GrabbedIntent grabbedIntent)
  {
    LogMethodCall(grabbingIntent, grabbedIntent);

    var grabbingGameObject = grabbingIntent.gameObject;
    var grabbedGameObject = grabbedIntent.gameObject;

    grabbingIntent.Remove();
    grabbedIntent.Remove();

    var awaitingGrabbing = grabbingGameObject.AddIfNotExisting<AwaitingGrabbing>();
    var awaitingGrabbed = grabbedGameObject.AddIfNotExisting<AwaitingGrabbed>();

    return new Tuple<AwaitingGrabbing, AwaitingGrabbed>(awaitingGrabbing, awaitingGrabbed);
  }

  // Touch is complete
  public static Tuple<Grabbing, Grabbed> GrabIntentToGrab(
    GrabbingIntent grabbingIntent,
    GrabbedIntent grabbedIntent)
  {
    LogMethodCall(grabbingIntent, grabbedIntent);

    var grabbingGameObject = grabbingIntent.gameObject;
    var grabbedGameObject = grabbedIntent.gameObject;

    //If it's a player change it to world being grabbed
    if (grabbedGameObject.TryGetComponent<PlayerArmBehaviour>(out var playerArmBehaviour))
    {
      var playerArms = GameObject.Find("Player").GetComponentOrThrow<PlayerArms>();
      playerArms.RemoveArm(playerArmBehaviour);
    }

    // Remove all the movement sources
    grabbedGameObject.GetComponentOrThrow<WorldArmBehaviour>().UpdateBehaviour(WorldArmBehaviour.WorldArmBehaviours.None);

    // Make sure grabbed algorithm is going to work properly
    var pivotPoint = grabbedGameObject.GetComponentOrThrow<PivotPoint>();
    pivotPoint.pivotPointType = PivotPoint.PivotPointType.Wrist;
    pivotPoint.LateUpdate();


    grabbingIntent.Remove();
    grabbedIntent.Remove();

    var grabbing = grabbingGameObject.AddIfNotExisting<Grabbing>();
    var grabbed = grabbedGameObject.AddIfNotExisting<Grabbed>();

    grabbing.other = grabbed;
    grabbing.isHandOpen = () => grabbingGameObject.GetComponent<DataSources>().gestureData.handOpen;

    grabbed.other = grabbing;

    grabbed.selectedStrategy = Grabbed.GetStrategy(grabbedGameObject, grabbingGameObject);

    return new Tuple<Grabbing, Grabbed>(grabbing, grabbed);
  }

  // User has open hands - wants to let go
  public static Tuple<ReleaseGrabbingIntent, ReleaseGrabbedIntent> GrabToReleaseIntent(
    Grabbing grabbing,
    Grabbed grabbed)
  {
    LogMethodCall(grabbing, grabbed);

    var grabbingGameObject = grabbing.gameObject;
    var grabbedGameObject = grabbed.gameObject;

    var strategy = grabbed.selectedStrategy;

    grabbing.Remove();
    grabbed.Remove();

    var releaseGrabbingIntent = grabbingGameObject.AddIfNotExisting<ReleaseGrabbingIntent>();
    var releaseGrabbedIntent = grabbedGameObject.AddIfNotExisting<ReleaseGrabbedIntent>();

    releaseGrabbingIntent.other = releaseGrabbedIntent;
    releaseGrabbingIntent.isHandOpen = () => grabbingGameObject.GetComponent<DataSources>().gestureData.handOpen;

    releaseGrabbedIntent.other = releaseGrabbingIntent;
    releaseGrabbedIntent.selectedStrategy = strategy;

    return new Tuple<ReleaseGrabbingIntent, ReleaseGrabbedIntent>(releaseGrabbingIntent, releaseGrabbedIntent);
  }

  // User closes hands - cancels release
  public static Tuple<Grabbing, Grabbed> CancelReleaseIntent(
    ReleaseGrabbingIntent releaseGrabbingIntent,
    ReleaseGrabbedIntent releaseGrabbedIntent)
  {
    LogMethodCall(releaseGrabbingIntent, releaseGrabbedIntent);

    var grabbingGameObject = releaseGrabbingIntent.gameObject;
    var grabbedGameObject = releaseGrabbedIntent.gameObject;

    var strategy = releaseGrabbedIntent.selectedStrategy;


    releaseGrabbingIntent.Remove();
    releaseGrabbedIntent.Remove();

    var grabbing = grabbingGameObject.AddIfNotExisting<Grabbing>();

    var grabbed = grabbedGameObject.AddIfNotExisting<Grabbed>();

    grabbing.other = grabbed;
    grabbing.isHandOpen = () => grabbingGameObject.GetComponent<DataSources>().gestureData.handOpen;

    grabbed.selectedStrategy = strategy;
    grabbed.other = grabbing;

    return new Tuple<Grabbing, Grabbed>(grabbing, grabbed);
  }

  // User held hands open long enough
  public static void ReleaseIntentToAwaiting(
    ReleaseGrabbingIntent releaseGrabbingIntent,
    ReleaseGrabbedIntent releaseGrabbedIntent)
  {
    LogMethodCall(releaseGrabbingIntent, releaseGrabbedIntent);

    var grabbingGameObject = releaseGrabbingIntent.gameObject;
    var grabbedGameObject = releaseGrabbedIntent.gameObject;

    releaseGrabbingIntent.Remove();
    releaseGrabbedIntent.Remove();

    AddAbilities(grabbingGameObject, true, grabbingGameObject.HasComponent<GrabbedAbility>());
    if (grabbingGameObject.HasComponent<GrabbedAbility>())
    {
      grabbingGameObject.AddIfNotExisting<AwaitingGrabbed>();
    }

    grabbedGameObject.GetComponent<WorldArmBehaviour>().UpdateBehaviour(WorldArmBehaviour.WorldArmBehaviours.Ragdoll);
    // Goes to ragdoll, so can never grab
    AddAbilities(grabbedGameObject, false, true);
  }

  // User is trying to attach to shoulder
  public static Tuple<AttachGrabbingIntent, AttachGrabbedIntent> GrabToAttachIntent(
    Grabbing grabbing,
    Grabbed grabbed,
    Func<bool> isColliding)
  {
    LogMethodCall(grabbing, grabbed, isColliding);

    var grabbingGameObject = grabbing.gameObject;
    var grabbedGameObject = grabbed.gameObject;

    var strategy = grabbed.selectedStrategy;

    grabbing.Remove();
    grabbed.Remove();

    var attachGrabbingIntent = grabbingGameObject.AddIfNotExisting<AttachGrabbingIntent>();
    var attachGrabbedIntent = grabbedGameObject.AddIfNotExisting<AttachGrabbedIntent>();

    attachGrabbingIntent.other = attachGrabbedIntent;
    attachGrabbedIntent.other = attachGrabbingIntent;
    attachGrabbedIntent.selectedStrategy = strategy;

    attachGrabbingIntent.isColliding = isColliding;

    return new Tuple<AttachGrabbingIntent, AttachGrabbedIntent>(attachGrabbingIntent, attachGrabbedIntent);
  }

  // User moves arm away - cancels attach intent
  public static Tuple<Grabbing, Grabbed> CancelAttachIntent(
    AttachGrabbingIntent attachGrabbingIntent,
    AttachGrabbedIntent attachGrabbedIntent)
  {
    var grabbingGameObject = attachGrabbingIntent.gameObject;
    var grabbedGameObject = attachGrabbedIntent.gameObject;

    var strategy = attachGrabbedIntent.selectedStrategy;


    attachGrabbingIntent.Remove();
    attachGrabbedIntent.Remove();

    var grabbing = grabbingGameObject.AddIfNotExisting<Grabbing>();
    grabbing.isHandOpen = () => grabbingGameObject.GetComponent<DataSources>().gestureData.handOpen;
    var grabbed = grabbedGameObject.AddIfNotExisting<Grabbed>();

    grabbing.other = grabbed;
    grabbed.other = grabbing;
    grabbed.selectedStrategy = strategy;

    return new Tuple<Grabbing, Grabbed>(grabbing, grabbed);
  }

  // User held arm to shoulder long enough
  public static void AttachIntentToAwaiting(
    AttachGrabbingIntent attachGrabbingIntent,
    AttachGrabbedIntent attachGrabbedIntent)
  {
    LogMethodCall(attachGrabbingIntent, attachGrabbedIntent);

    var grabbingGameObject = attachGrabbingIntent.gameObject;
    var grabbedGameObject = attachGrabbedIntent.gameObject;

    attachGrabbingIntent.Remove();
    attachGrabbedIntent.Remove();

    var behaviour = grabbedGameObject.GetComponent<WorldArmBehaviour>();
    var playerArms = GameObject.Find("Player").GetComponentOrThrow<PlayerArms>();
    playerArms.AddArm(behaviour);

    // If you can put the shoulder on you can do both actions again
    AddAbilities(grabbedGameObject, true, true);
    AddAbilities(grabbingGameObject, true, true);
  }
}
