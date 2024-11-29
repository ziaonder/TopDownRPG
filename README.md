### 2D Open World RPG Game

## Features
- 3 different terrain(forest, desert, arctic). Each terrain has 2 distinct enemy type. All enemies have some 
lock up time to the target(player in their case) before attacking.
  - **Forest enemies;**
    - Glutterfly attack pattern: Throws a projectile to that position.
    - Booby attack pattern: Leaps into the air in a smooth arc, following a projectile motion trajectory.
  - **Desert enemies;**
    - Kamikazzy attack pattern: Follows target to a distance then explode on the area after a brief moment.
    ![gif](gifs/kamikazzy.gif)
    - Tartil attack pattern: Shoots a projectile in a smooth arc targeted to player position. Projectile covers 
      an area for some time and damages the player while the player is on it.
  - **Arctic enemies;**
    - SlidingThing attack pattern: Slides to the player position and damaging the player.
    - Mushroom attack patterns;
      - Throws 2 projectiles to the opposite directions. One is targeted to player.
      - Holds for a moment to blast within a radius, and damaging all in that area.