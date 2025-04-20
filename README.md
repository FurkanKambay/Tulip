# Project Tulip

[<img src="https://static.itch.io/images/badge-color.svg" height="48px"></img>](https://furkankambay.itch.io/tulip)

[<img src="https://img.itch.zone/aW1hZ2UvMjgyOTg2OS8xNzI4NjkzMS5naWY=/347x500/3zsHza.gif" width=400 alt="combat in Project Tulip">](https://furkankambay.itch.io/tulip)
[<img src="https://img.itch.zone/aW1hZ2UvMjgyOTg2OS8xNzI4Njk3My5naWY=/347x500/UrMrAz.gif" width=400 alt="mining in Project Tulip">](https://furkankambay.itch.io/tulip)

> [!TIP]
> Play it on itch.io! 💚 [furkankambay.itch.io/tulip](https://furkankambay.itch.io/tulip)

**Project Tulip** is a platformer game, and it's still a work in progress and mainly a portfolio piece for now. I plan for it to have a compelling and immersive hand-crafted world and unique gameplay mechanics, and I'm very excited about all of it.

## Features

### Systems & Mechanics
- [World terraforming](Assets/Code/GameWorld/World.cs) (breakable & placeable tiles)
- [Grappling hook](Assets/Code/Gameplay/HookLauncher.cs) mechanic
- [Entity spawn constraints](Assets/Code/Data/SpawnConditionSO.cs): "only on safe blocks", "needs headroom", etc.
- [Item swing motion](Assets/Code/Gameplay/ItemWielder.cs) config w/ editor tooling
- [Melee combat](Assets/Code/Gameplay/WeaponWielder.cs): spear, axe
- [Invulnerability frames](Assets/Code/Character/Health.cs)
- [Status effects](Assets/Code/Data/StatusEffect.cs): health regen, bleed
- [Enemy AI](Assets/Code/AI): walking, flying, attacking
- [Hotbar](Assets/Code/UI/HotbarPresenter.cs) w/ slot locking
- [Loot items](Assets/Code/Data/EntitySO.cs) on entities and tiles

### Content
- [Three-layer world](Assets/Prefabs/Maps/Realm%20Visuals.prefab) tilemap (background walls, blocks, foreground curtains)
- [Custom Rule Tiles](Assets/Resources/Tiles): Blocks, walls, curtains
- [Entities](Assets/Prefabs/Characters): Treant mimic, flying skull, trees, generic walking enemy
- [Items](Assets/Resources/Items): Weapons, tools, materials (ore, wood), tiles

### Shaders & VFX
- [Portal Shader](Assets/Shaders/Realm.shadergraph): Shader for rendering two worlds through a portal *(not used for now)*
- [Parallax Shader](Assets/Shaders/Parallax.shadergraph): Parallax background with cloud movement
- [Sprite outline and dissolve](Assets/Shaders/Color%20Tint.shadergraph) shader
- [Rain VFX](Assets/VFX/Rain.vfx): Rain in VFX Graph, configurable *(unavailable on WebGL)*

### User Interface (UI Toolkit)
- [Menus](Assets/UI/Documents) | [Hotbar HUD](Assets/UI/Documents/Hotbar.uxml) | [Death overlay](Assets/UI/Documents/DeathOverlay.uxml)
- [Data bindings](Assets/UI/Documents/SettingsMenu.uxml) using the MVP pattern
- [Global converters](Assets/Code/UI/GlobalConverters.cs) for data bindings
- [Template](Assets/UI/Templates/HotbarSlot.uxml) for hotbar item slots
- [Custom styling](Assets/UI/Styles) for default Unity UITK controls like `DropdownField`, `TabView` via `.unity-` USS classes

### Audio (FMOD)
- [FMOD project source](FMODProject) included
- [Footstep sounds](Assets/Code/Audio/FootstepAudio.cs) for the player and enemies
- [Muffled music](Assets/Code/Audio/BiomeMusic.cs) when player is indoors
- [Audio volume options](Assets/Code/Core/Settings.Audio.cs) in game
- **Positional audio** in 2D space

### Misc.
- **Async** and `Awaitable` usage
- [Input System](Assets/Settings/Input%20Actions.inputactions) usage
- Fixed data persistence bug on web builds using [a custom WebGL template](https://github.com/FurkanKambay/Tulip/commit/c4d97ec0718cf6d3dfbc1e7d04e74d5d5c943c87)
- [Event channels](Assets/Code/Common/EventChannelSO.cs) for some UI events (`ScriptableObject`-based)

<details><summary><h3>Plans</h3></summary>

- hand-crafted world
- ranged weapon
- rain mechanic
- a couple more enemies
- better enemy AI
- NPCs

</details>

## Inspirations

Project Tulip was originally inspired by Terraria, but I'm moving away from the procedurally generated sandbox idea in favor of a hand-crafted world after I had some exciting ideas for worldbuilding. Games like Hollow Knight, Ori and the Blind Forest, V Rising, Divinity: Original Sin 2, Core Keeper, Wall World, Laika: Aged Through Blood, SteamWorld Dig, and Fallout *(even though I haven't played it—shout out Tim Cain)*, are some of the biggest inspirations for it. I can't say for certain that the final game will be inspired by these games in any way, but I'm sure the inspirations will show themselves once the game reaches a certain point in development. I'm aiming for compelling worldbuilding, satisfying combat, and interesting characters.

## Game Design

I have a lot of notes on different design aspects like narrative, system mechanics, NPCs, quests, enemies/bosses, and more. I might share the plans in more detail once I've made good progress on something.

## Art & Sound

The art direction/style is not finalized. I'm using placeholder assets that seem to go well enough together for the time being. Same with music and sound effects. I made some placeholder art and paid for some others (see [ATTRIBUTIONS](ATTRIBUTIONS.md)), but the paid assets aren't available on GitHub because I can't share the source files. The itch.io builds use the paid assets as expected, though! I bought a MIDI keyboard to learn music, which may not have been the smartest investment, but surely I'll make some progress one day.
