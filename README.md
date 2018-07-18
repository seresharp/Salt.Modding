Preface
=======

Salt and Sanctuary modding project using MonoMod. https://github.com/0x0ade/MonoMod

Building The API
============================

1. Clone this
2. Go to your game installation and copy the game exe and dlls there to a folder called "Vanilla" in the base directory of this repo (This will work for either a Windows or OSX game install, untested with Linux)
3. Build MonoMod and place the exe and dlls in a folder called "MonoMod" in the base directory of this repo
4. If you are building for Mac, change the second instance of `salt.ModdingApi.mm.dll` in the post-build events to `Salt.ModdingApi.mm.dll` (Capital S)
5. Open the solution in Visual Studio 2017 (May work in other versions, only tested on 2017)
6. Set the build configuration to Debug.
7. Build the project and move the new game exe from `%RepoPath%/Output/` to your game folder
8. Place mods in a folder called "Mods" in your game folder