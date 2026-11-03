# CITY LIFE INDIA 🇮🇳
### 3D Open World Life Simulation Game (Android & PC)

Inspired by the heritage and vibrancy of Jaipur and Rajasthan.

---

## 🎮 How to Play Immediately on Your PC (1-Click Run)

You can play the complete 3D game immediately in your web browser (Chrome, Edge, Firefox) without needing any installation:

1. Go to the project folder:
   ```
   C:\Users\dell\.gemini\antigravity\scratch\CITY_LIFE_INDIA\
   ```
2. **Double-click `Run_City_Life_India.bat`**
   *(Or directly open `Playable_3D_Prototype/index.html` in your browser!)*

### Controls:
- **Move / Steer**: `W`, `A`, `S`, `D` or Arrow Keys (or on-screen Virtual Touch Joystick on mobile)
- **Sprint / Run**: `Left Shift` (or on-screen `RUN` button)
- **Jump / Handbrake**: `Spacebar` (or on-screen `JUMP` button)
- **Enter / Exit Vehicle**: `F` or `E` (or on-screen `DRIVE CAR` / `RIDE BIKE` button)
- **Horn**: `H` (or on-screen `HORN` button)
- **Toggle Camera View**: `C` (Switch between 3rd-person & 1st-person)
- **Toggle Day / Night**: Click `Day / Night` in top bar or press `L`
- **Missions Menu**: Click `🎯 Missions` to choose any of the 5 MVP missions
- **Garage Menu**: Click `🚗 Garage` to switch vehicles and change paint colors in real-time!

---

## 📁 Complete Project Contents

```
CITY_LIFE_INDIA/
│
├── Run_City_Life_India.bat            # 1-Click Game Launcher for PC
│
├── Playable_3D_Prototype/             # Standalone 3D Game (Three.js WebGL)
│   ├── index.html                     # Complete 3D Game with Jaipur city, car/bike physics, 5 missions, audio & save
│   └── three.min.js                   # 100% Offline 3D Engine (no internet required)
│
├── Assets/Scripts/                    # Full Unity C# Production Architecture
│   ├── Core/
│   │   └── GameManager.cs             # State machine, session flow & scene transitions
│   ├── Player/
│   │   ├── PlayerController.cs        # CharacterController movement, jump, sprint, vehicle interaction
│   │   └── CharacterCustomizer.cs     # Skin, traditional kurta/shirt, pajama colors
│   ├── Vehicles/
│   │   ├── VehicleController.cs       # 4-wheel car physics, headlights, engine RPM sound, enter/exit
│   │   ├── BikeController.cs          # Motorcycle 2-wheel physics, cornering lean angle, balance
│   │   └── VehicleManager.cs          # Garage spawning, active vehicle tracking, paint application
│   ├── Camera/
│   │   └── CameraController.cs        # Smooth 3rd-person follow, touch look, obstacle occlusion check
│   ├── Missions/
│   │   ├── MissionData.cs             # ScriptableObject for Delivery, Taxi, Speedway, Explore, Photo
│   │   └── MissionManager.cs          # 3D waypoint beacon, countdown timer, checkpoint triggers, reward payout
│   ├── World/
│   │   ├── WorldManager.cs            # 1km x 1km Jaipur district triggers & boundaries
│   │   └── DayNightCycle.cs           # Dynamic sun rotation, twilight gradients, night streetlights
│   ├── SaveSystem/
│   │   ├── PlayerData.cs              # Serializable JSON container for Coins, XP, Level, Paint
│   │   └── SaveManager.cs             # Local persistent JSON storage in Application.persistentDataPath
│   ├── UI/
│   │   ├── VirtualJoystick.cs         # On-screen floating touch joystick for Android
│   │   ├── HUDManager.cs              # Radar minimap, speedometer gauge, mission banner, coins/XP
│   │   ├── UIManager.cs               # Screen transitions (MainMenu, HUD, Garage, Pause, Settings)
│   │   └── GarageUI.cs                # Vehicle paint swatch picker & vehicle stats
│   └── Audio/
│       └── AudioManager.cs            # Engine pitch modulation, horns, chimes, victory jingles
│
└── Documentation/
    ├── GDD/CITY_LIFE_INDIA_GDD.md     # Complete Game Design Document
    └── Unity_Setup_Guide.md           # Step-by-step Unity Hub & Android APK export guide
```

---

## 🇮🇳 Features Implemented (Based on GDD)

1. **Jaipur / Rajasthan 3D Open World**:
   - Iconic Hawa Mahal Palace with tiered pink sandstone architecture and arched windows
   - Central Roundabout & Grand Fountain
   - Old Johari Bazaar with spice & tea stalls (Dhabas)
   - Modern residential apartments & Tech Park
   - Central Transport Bus Hub
   - Amber Hill Viewpoint with scenic overlook
   - Dynamic Day & Night lighting with illuminated streetlamps

2. **Drivable 3D Vehicles**:
   - **Desi Cruiser (Sports Car)**: Working wheels, headlights, brake lights, physical steering, acceleration, reverse, and drifting handbrake.
   - **Rajputana 350 (Motorcycle)**: Authentic bike chassis, turning lean angle, and handle animation.
   - Enter/Exit interaction with proximity button.

3. **5 Full Missions (GDD MVP Scope)**:
   - **Delivery**: Jaipur Chai & Parcel Delivery
   - **Taxi**: City Taxi - Tourist to Palace Hotel
   - **Driving Challenge**: Rajasthan Speedway Sprint (4 checkpoints)
   - **Exploration**: Discover Amber Hill Viewpoint
   - **Photography**: Hawa Mahal Tourist Snap

4. **Economy & Progression**:
   - ₹ Coins currency earned from missions
   - XP and Level Progression (Level 1, 2, 3...)
   - Persistent saving to system storage (progress is never lost)

5. **Audio Engine**:
   - Real-time synthesized engine revving with pitch ramp
   - Car horn on demand
   - Mission victory and checkpoint chimes
   - Footstep sounds while exploring
