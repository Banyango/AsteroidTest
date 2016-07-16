# AsteroidTest
Project Goal: 
Create the foundation for an Asteroids-like game by following the spec below exactly. 

Some notes:

The Bullet classes use inheritance to extend the standard bullet behaviour. I would probably switch that up to composition if there were lots of bullet types planned.
The ICharacterController is a class I wrote before this test. It is a heavily modified version of Prime31's Character controller. There were a number of issues with the original controller. The biggest being that TK2D collision generates line colliders and with the raycasting method you can pass through the wall in one timestep. Be inside on the collision test and not collide with anything, thereby falling through a surface. I added box casting, where you more or less do a sweep test between the old point and new point. And set the collider back if there is a tunnel detected. I also added the concept of IMovementModifiers which are bits of logic that you can use to move an object around that can be enabled and disabled when needed. It allows for clean fine grained control when you have state changes on an object. I also added with this test the collision callback handling.
I've included an extra way to move around. I felt that it was more fun with WASD boosting you in certain directions


Features:

- WASD moves your ship DONE
- Space bar fires a bullet from the nose of your ship DONE
- Your ship orients itself so the nose faces the direction you're going DONE
- Your ship has momentum in the direction of travel DONE
- The ship cannot travel off the edge of the screen DONE
- If a bullet collides with the ship, the ship is destroyed 
- If it is destroyed, the ship respawns at the center of the screen after a 1 second delay
#Toggle-ables:
- Using a radio button, you can toggle the screen boundary behavior between three settings: Wall, Bumper, and Wrap
 - Wall is the standard behavior. The screen boundary prevents your ship moving past it.
 - Bumper causes the ship and bullet's momentum to be reflected when it collides with the wall at a 1 to 1 scale.
 - Wrap causes the ship and bullet to appear at the same location on the opposite edge of the screen.

Using a radio button, you can toggle between two bullet types: Standard, and Wave
 - Standard travels in a straight line from the nose of the ship.
 - Wave travels in a Sine wave pattern along a straight line from the nose of the ship.

At run time, the player can change the number of seconds before the ship respawns through a text entry or radio button selector.

Assets:
Use a white triangle primitive for the ship
Spawn a white cylinder for your bullets
Leave the background black
Implementation Guidelines:
Don’t hard code systems. 
Use clean architecture such that it would be easy to make changes and/or add features in the future. 

