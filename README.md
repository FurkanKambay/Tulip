<h1 align="center">
  <img width="48px" src="Assets\Art\Sprites\Misc\TulipIcon_64px.png" alt="Tulip icon" />
 Tulip
</h1>

[<img align="right" width="400px" src="https://img.itch.zone/aW1hZ2UvMjgyOTg2OS8xNzI4NjkzMS5naWY=/347x500/3zsHza.gif" alt="combat in Tulip">](https://furkankambay.itch.io/tulip)

**Tulip** is an action-adventure platformer made in Unity. It is still a *work in progress* and currently serves primarily as a portfolio project. My goal is to create a compelling game world with satisfying combat and exploration.

Originally, Tulip was heavily inspired by Terraria and featured a procedurally generated world with mining and building. However, I decided to move away from a sandbox in favor of a hand-crafted world after developing some exciting ideas for worldbuilding. Some of my biggest inspirations include games like Hollow Knight, Ori and the Blind Forest, V Rising, Divinity: Original Sin 2, Core Keeper, Wall World, Laika: Aged Through Blood, SteamWorld Dig, and Fallout *(even though I haven't played it—shout out to Tim Cain)*.

<a href="https://furkankambay.itch.io/tulip">
  <img width="180px" src="https://static.itch.io/images/badge-color.svg" alt="available on itch.io">
</a>

## ⭐ Features

[![Ask DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/FurkanKambay/Tulip)

### ⚙️ Systems & Mechanics

- [Melee combat](Assets/Code/Gameplay/WeaponWielder.cs): spear, axe
- [Grappling hook](Assets/Code/Gameplay/HookLauncher.cs) mechanic
- [Spawn constraints](Assets/Code/Data/SpawnConditionSO.cs): rules like "only on safe blocks", "needs headroom", etc.
- [Item swing motion](Assets/Code/Gameplay/ItemWielder.cs) config with editor tooling
- [Invulnerability frames](Assets/Code/Character/Health.cs)
- [Status effects](Assets/Code/Data/StatusEffect.cs): health regen, bleed
- [Enemy AI](Assets/Code/AI): walking, flying, attacking
- [Loot items](Assets/Code/Data/EntitySO.cs) for entities

### 👾 Content

- [LDtk project](Assets/Level/LDtk): Blocks, walls, curtains
- [Three-layer world](Assets/Code/GameWorld/World.cs) tilemap (background walls, blocks, foreground curtains)
- [Entities](Assets/Prefabs/Characters): Treant mimic, flying skull, trees, generic walking enemy
- [Items](Assets/Resources/Items): Weapons, tools, materials (ore, wood), tiles

### ✨ Shaders & VFX

- [Portal Shader](Assets/Shaders/Realm.shadergraph): Shader for rendering two worlds through a portal *(unused)*
- [Parallax Shader](Assets/Shaders/Parallax.shadergraph): Parallax background shader with moving cloud
- [Sprite outline and dissolve](Assets/Shaders/Color%20Tint.shadergraph) shader
- [Rain VFX](Assets/VFX/Rain.vfx): Rain in VFX Graph, configurable *(not supported on WebGL)*

### 📱 User Interface (UI Toolkit)

- [Menus](Assets/UI/Documents) | [Death overlay](Assets/UI/Documents/DeathOverlay.uxml)
- [Data bindings](Assets/UI/Documents/SettingsMenu.uxml) using the MVP pattern
- [Global converters](Assets/Code/UI/GlobalConverters.cs) for data bindings
- [Custom styling](Assets/UI/Styles) for default Unity UITK controls like `DropdownField`, `TabView` via `.unity-` USS classes

### 🎵 Audio (FMOD)

- [FMOD project source](FMODProject) included
- [Footstep sounds](Assets/Code/Audio/FootstepAudio.cs) for the player and enemies
- [Muffled music](Assets/Code/Audio/BiomeMusic.cs) when player is indoors
- [Audio volume options](Assets/Code/Core/Settings.Audio.cs) in game
- **Positional audio** in 2D space

### 🔧 Miscellaneous

- **Async** and `Awaitable` usage
- [Input System](Assets/Settings/Input%20Actions.inputactions) usage
- [Custom WebGL template](Assets/WebGLTemplates/Custom) to fix data persistence bug on web builds
- [Event channels](Assets/Code/Common/GameEvent.cs) for some UI events (`ScriptableObject`-based)

<details><summary><h3>🗑️ Abandoned</h3></summary>

These features have been abandoned either because they are out of scope or don't fit the design anymore:

- [World Terraforming](../v7-itchio/Assets/Code/Player/Terraformer.cs) (breakable & placeable tiles)
- [Custom Rule Tiles](../v7-itchio/Assets/Code/Data/Tiles/CustomRuleTileData.cs) for the world
- [Hotbar](../v7-itchio/Assets/Code/UI/HotbarPresenter.cs) with slot locking, [HUD](../v7-itchio/Assets/UI/Documents/Hotbar.uxml), and [UI Template](../v7-itchio/Assets/UI/Templates/HotbarSlot.uxml) for hotbar item slots

</details>

<details><summary><h3>💡 Planned</h3></summary>

- A hand-crafted world
- Rain system
- More equipments, weapon types
- More enemies, NPCs
- Better enemy AI

</details>

## Game Design

I have a lot of notes on different design aspects like narrative, system mechanics, NPCs, quests, enemies/bosses, and more. I might share the plans in more detail once I've made good progress on something.

## Art & Sound

The art direction/style is not finalized. I'm using placeholder assets that seem to go well enough together for the time being. Same with music and sound effects. I purchased some assets ([ATTRIBUTIONS](ATTRIBUTIONS.md)) and made some placeholder art myself, but the paid assets aren't available on the public repo since I can't share the source files. The itch.io builds use the paid assets as expected, though! I also bought a MIDI keyboard to learn music, which may not have been the smartest investment, but surely I'll make some progress one day.
