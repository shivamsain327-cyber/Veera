# CITY LIFE INDIA
## 3D Open World Android Game - Game Design Document & Roadmap

---

### Executive Summary
- **Title**: CITY LIFE INDIA
- **Genre**: 3D Open World Life Simulation
- **Target Platform**: Android Mobile (PC for development/testing)
- **Engine**: Unity + C#
- **Render Pipeline**: Universal Render Pipeline (URP)
- **Theme**: Fictional Indian city inspired by Jaipur and Rajasthan (Hawa Mahal landmark, pink sandstone architecture, bustling spice/tea markets, dhabas, modern apartments, and scenic hill viewpoints).

---

### Core Gameplay Pillars
1. **Freedom**: Players choose where to go and what activity to pursue.
2. **Everyday Indian City Life**: Shops, roads, dhabas, tea stalls, residential neighbourhoods, bus terminals, and landmarks.
3. **Progression**: Complete 5 MVP missions to earn virtual currency (₹ Coins), XP, level unlocks, and custom vehicle paint finishes.
4. **Social & Multiplayer (Roadmap Phase 8)**: Meet friends in 2–4 player private exploration rooms.
5. **Mobile Performance**: Optimized for 30+ FPS on budget and mid-range Android devices.

---

### World Map Districts (1 km × 1 km)
1. **City Center Plaza**: Grand boulevard, roundabout with fountain, civic offices.
2. **Old Johari Market**: Colorful dhabas, spice stalls, traditional Rajasthani textiles.
3. **Hawa Palace Heritage**: Majestic pink terracotta sandstone palace with ornate jharokha arched windows.
4. **Green Meadows Residential**: Modern apartment complexes, neighborhood alleys, quiet residential streets.
5. **Transport Hub**: Central bus stand and taxi terminal.
6. **Amber Hill Overlook**: High scenic viewpoint overlooking the entire city.

---

### Vehicle & Physics System
- **Desi Cruiser (Sports Car)**: 4-wheel physics, steering angle, acceleration, handbrake drift, functional headlights, and horn.
- **Rajputana 350 (Motorcycle)**: 2-wheel balance physics with dynamic lean angle during cornering.
- **Enter/Exit Mechanics**: Smooth transfer between character and vehicle modes on button press or key (`F` / `E`).

---

### 5 MVP Missions
1. **Delivery**: *Jaipur Chai & Parcel Delivery* (Collect from Market, deliver to Tech Hub within time limit).
2. **Taxi**: *City Taxi: Tourist to Palace* (Pick up passenger at Bus Terminal, drop at Palace Hotel).
3. **Driving Challenge**: *Rajasthan Speedway Sprint* (Slalom through 4 neon checkpoints under 60 seconds).
4. **Exploration**: *Discover Amber Hill Viewpoint* (Navigate to scenic hilltop lookout).
5. **Photography**: *Hawa Mahal Tourist Snap* (Align in front of Hawa Mahal facade and capture photo).

---

### Technical Architecture
- `GameManager`: Central finite state machine (Boot, MainMenu, Gameplay, InVehicle, Paused, MissionActive).
- `PlayerController`: Third-person character movement, sprint, jump, vehicle interaction trigger.
- `VehicleController` & `BikeController`: Vehicle physics, wheel colliders, headlights, and engine audio.
- `MissionManager`: Objective queue, holographic 3D waypoint beacon, distance calculation, countdown timer, reward payout.
- `SaveManager`: Local persistent JSON data storage for Coins, XP, Level, unlocked vehicles, and custom paint.
- `HUDManager` & `UIManager`: Real-time minimap radar, digital speedometer, coin/XP counter, garage paint picker, pause modal.
- `AudioManager`: Engine audio pitch modulation with RPM, horn, footsteps, checkpoint chimes, and victory fanfares.
