## Editor bug

Because we're using .blend files and the unity blender importer:

There's a bug with importing animations that doesn't allow multiple animations to be imported. To overcome this change the following line from false to true:

bake_anim_use_all_actions=True,

In file C:\Program Files\2020.3.18f1\Editor\Data\Tools

## Unity Version

2020.3.18f1

## Notes on unity

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
