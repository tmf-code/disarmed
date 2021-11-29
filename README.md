# Disarmed
Disarmed is a short (10 minute) experimental documentary about the effects of increased virtual mediation on our bodies.

## Getting Started

Follow these steps to get up and running:
- [Unity Hub](https://unity3d.com/get-unity/download)
- Once Unity Hub is installed you open [this link](unityhub://2020.3.20f1/41c4e627c95f) to start installing the right Unity version
- Make sure to check to install the Android build tools
- Now you can open Unity Hub and add this project. 
- When the project is added you can open it
- You then need to reimport the meshes by right clicking  `Assets > Avatar > Meshes` in the Project panel and select `Reimport All`
- Now you can open the Avatar scene in `Assets > Avatar` in the Project panel and run it (by pressing the play icon)

## Using Oculus Air Link
You can use Air Link (still in beta) to directly see the result of Unity on the Quest. This makes developing a lot easier and quick as you don't have to deploy to the Quest every time you make changes. Unfortunately this only works on Windows.

On Windows: 
- Install and setup the [Oculus software](https://www.oculus.com/setup/)
- Enable Air Link in the Oculus software: `Settings > Beta`
- Allow Unkown Sources in the Oculus software: `Settings > General`

On the Quest:
- Press the clock to enter Quick Settings
- Then go to `Settings > Experimental Features` and enable `Air Link`
- When you go back to Quick Settings you should see Air Link in the top right. Tapping there should allow you to pair and connect to the computer running Oculus software (computer must be running the Oculus software for it to work!) 
- After pairing/connecting you should be able to launch Air Link
- With Air Link launched you can press the play button in Unity. This should play the scene on the Quest.

## Editor bug

Because we're using .blend files and the unity blender importer:

There's a bug with importing animations that doesn't allow multiple animations to be imported. To overcome this change the following line from false to true:

bake_anim_use_all_actions=True,

In file C:\Program Files\2020.3.20f1\Editor\Data\Tools

## Unity Version

2020.3.20f1

## Notes on Unity

Unity supports a subset of features up to c# 8.0 and .NET 4.6.
More info can be found here: https://docs.unity3d.com/Manual/CSharpCompiler.html

Unity made the decision to overload the == operator on checking for existence of things through the results of things like `GetComponent` calls. They fake that the result is null, when it is actually something else.

This means that you can't use null coalescence, or optional chaining.

Thus things like the below do not work. It's best to try and avoid it completely even though it does work for non unity operations.

```c#
var a = b?.method()?.cat;
var a ??= dog;
var a = cat ?? dog;
```

### Serializing

In order for the UI to save variables into the fields, and also resume execution on a hot-code swap, you must mark things as serialisable. This means that Unity can store them in it's asset database to be restored on load/unload of scenes etc.

Examples

```c#
[Serializable]
class Cat {
  public string name = "cat";
}


class Cat {
  [SerializeField]
  private string privateNameButIsStillSerialized = "cat";
}
```

No other data structures can be 'serialized' except primitives, arrays (ie: []), and Lists

A list is:

```C#
var a = new List<String>() {"cat", "dog"};

```

### Getting the language server to work

You need to configure unity to use vscode as it's editor. https://code.visualstudio.com/docs/other/unity

Then when you want to open the project for the first time in vscode, go to Unity -> Assets -> Open C# project
