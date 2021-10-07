# Editor bug

Because we're using .blend files and the unity blender importer:

There's a bug with importing animations that doesn't allow multiple animations to be imported. To overcome this change the following line from false to true:

bake_anim_use_all_actions=True,

In file C:\Program Files\2020.3.18f1\Editor\Data\Tools

# Unity Version

2020.3.18f1
