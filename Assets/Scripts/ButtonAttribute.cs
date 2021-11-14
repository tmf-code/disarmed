using UnityEngine;

#if UNITY_EDITOR


public class ButtonAttribute : PropertyAttribute
{
  public string MethodName { get; }
  public ButtonAttribute(string methodName)
  {
    MethodName = methodName;
  }
}

#endif