# CITY LIFE INDIA - Unity Project Setup & Android Build Guide

This guide explains how to open the **CITY LIFE INDIA** project in Unity, configure the Universal Render Pipeline (URP), connect all C# systems, and build an Android APK / AAB.

---

## 1. Prerequisites (Unity Hub & Modules)

1. **Install Unity Hub**: Download from [unity.com](https://unity.com/download).
2. **Install Unity Editor**: Recommended version `2022.3 LTS` or `2023.2+`.
3. **Required Modules** (Install via Unity Hub -> Installs -> Gear Icon -> Add Modules):
   - **Android Build Support**
   - **Android SDK & NDK Tools**
   - **OpenJDK**

---

## 2. Project Folder Structure

The project has been initialized according to Section 8 of the GDD:

```
CITY_LIFE_INDIA/
├── Assets/
│   ├── Scripts/
│   │   ├── Core/           # GameManager.cs (State machine, session flow)
│   │   ├── Player/         # PlayerController.cs, CharacterCustomizer.cs
│   │   ├── Vehicles/       # VehicleController.cs, BikeController.cs, VehicleManager.cs
│   │   ├── Camera/         # CameraController.cs (Smooth 3rd person follow & occlusion)
│   │   ├── Missions/       # MissionManager.cs, MissionData.cs (5 MVP missions)
│   │   ├── World/          # WorldManager.cs, DayNightCycle.cs
│   │   ├── SaveSystem/     # SaveManager.cs, PlayerData.cs (JSON persistent save)
│   │   ├── UI/             # VirtualJoystick.cs, HUDManager.cs, UIManager.cs, GarageUI.cs
│   │   └── Audio/          # AudioManager.cs
├── Documentation/
│   ├── GDD/                # CITY_LIFE_INDIA_GDD.md
│   └── Unity_Setup_Guide.md
├── Playable_3D_Prototype/  # Fully playable 3D WebGL / HTML5 standalone version
└── Run_City_Life_India.bat # One-click desktop launcher
```

---

## 3. Opening the Project in Unity

1. Open **Unity Hub**.
2. Click **Add** -> **Add project from disk**.
3. Select `C:\Users\dell\.gemini\antigravity\scratch\CITY_LIFE_INDIA`.
4. Unity will import the C# scripts and create the Library cache.

---

## 4. Scene Setup Steps

### Scene 1: Boot / MainMenu
1. Create a Canvas with UI elements: Play, Garage, Settings, Exit.
2. Attach `UIManager.cs` and `GameManager.cs`.

### Scene 2: CityWorld
1. Add an empty GameObject named `[MANAGERS]` and attach:
   - `GameManager.cs`
   - `SaveManager.cs`
   - `WorldManager.cs`
   - `MissionManager.cs`
   - `VehicleManager.cs`
   - `AudioManager.cs`
2. Player Setup:
   - Create a Capsule / 3D Character model with a `CharacterController`.
   - Attach `PlayerController.cs` and `CharacterCustomizer.cs`.
   - Tag the GameObject as `Player`.
3. Camera Setup:
   - Attach `CameraController.cs` to the Main Camera.
   - Assign the Player Transform to `currentTarget`.
4. Vehicles:
   - Create vehicle prefabs with `Rigidbody` and `VehicleController.cs` (or `BikeController.cs`).
   - Assign wheel colliders, headlight objects, and seat/exit transforms.
5. Canvas HUD:
   - Add `HUDManager.cs`, `VirtualJoystick.cs`, Minimap, and Enter/Exit buttons.

---

## 5. Android Build Settings

1. In Unity, go to **File -> Build Settings**.
2. Select **Android** and click **Switch Platform**.
3. Click **Player Settings**:
   - **Company Name**: Your Studio Name
   - **Product Name**: City Life India
   - **Package Name**: `com.yourcompany.citylifeindia`
   - **Minimum API Level**: Android 9.0 (API Level 28)
   - **Target API Level**: Android 14 / Automatic (highest installed)
   - **Scripting Backend**: IL2CPP
   - **Target Architectures**: ARM64 (mandatory for Google Play)
4. Connect an Android phone with **USB Debugging** enabled.
5. Click **Build and Run** to install the APK directly onto your phone!
