# factorygame
Controls:
WASD - movement

M - open map

Left Click - Place items/interact with entities and menus

Right Click - Mine items/ split item stack in inventory

Scroll Wheel - Zoom in and out

E - Open Inventory

Q - Return held item to inventory

F - Drop held item on ground

ESC - Open pause menu

This is a demonstration of the game engine we developed over the semester.  The game engine is built out of several distinct parts,
some of which are dependent on previous parts.  The entire engine was built using SFML.NET, an OpenGL multimedia library which simplifies
window creation, loading textures, checking whether some input has ocurred, and loading audio,  but does not offer any game engine specific features.

The main features of our engine that are generic:
  
  -Dynamic Runtime texture atlasing
  
    -Automatically packs textures into a texture atlas at runtime
    
    -Uses scanline algorithm
    
    -Stores bounds of individual textures in the new texture atlas for easy retrieval
    
    -Automatically creates new texture atlases
    
    -Functionally simple: create the texture atlas, call load textures with the texture path, and now when you need a texture, call gettexture()
    
    -Can also specify priority texture paths, which the algorithm will try to pack together, or into as few texture atlases as possible.
  
  -Input management system
  
    -The input management system we implemented supports realtime input (not locked to framelimit) with consumption techniques
    
      -Input is processed in order over a list of subscribers after being captured
      
      -Subscribers can consume an input, meaning that other subscribers will not see the input
      
    -The input system is somewhat simple to use: create the InputManager, implement an Entity/Camera/etc with the IInputSubscriber interface, now
      you can subscribe the Entity/Camera/etc you created to the InputManager.  The subscriber is in charge of determining what to do with the input,
      its handleInput() function will be called once per frame.
      
  -Sprite batching System
  
    -Accelerates 2D drawing massively
    -By calling draw and passing in a sprite or a supported drawable(see below), the spritebatcher determines if a drawable shares the same texture as
      the previous drawable, and if they do, then the spritebatcher automatically appends the new drawable to the current vertexarray.
      -This greatly reduces the number of texture binds OpenGL performs in addition to the number of draw calls needed.
  -2D drawables
  
    -Our engine implements two typical styles of 2D drawables, a simple animation and a static sprite, however, it also implements a special
    and powerful animation we call AnimationRotated.
      -Animation Rotated greatly simplifies the complexity behind drawing a topdown/isometric entity by automatically computing the proper
      subset of provided animations based on a provided rotation.  It supports a variable number of evenly rotated animations, for example it
      can support 4-way and 8-way with a single implementation.
    -All of our engines drawables are implemented with a Draw function specifically for our spritebatching system, but animation and static sprite can
    also return the generic sprite provided by SFML.
  -BoundingBox
  
    -Our engine implements a bounding box class that supports both rotated and axis aligned bounding boxes.  There are several auxilliary methods that are built
    specifically for our game, but they can be removed without affecting the functionality of the bounding box.
    -The BoundingBox is designed to automatically center around a specific point, but can support non-centered bounding boxes as well.
    -The BoundingBox implements a checkcollision method that returns a pushback vector and one that just checks for collision
    -The BoundingBox initially uses a circle culling check using precomputed radii approximations for each bounding box, which is very fast.
    -If the circle culling check does not remove the possibility of a collision, then if the bounding boxes are not rotated, a modified version of
    a simple AABB check is ran.  Our version of the simple AABB check takes into account the possibility of a non-center bounding box.
    -Otherwise, if the bounding boxes are rotated, then an implementation of separating axis theorem (SAT) is ran.  This version of SAT
    uses the assumption that our bounding boxes are rectangles to its advantage to simplify the check.
  -Vector2
  
    -Our engine includes a simple vector2 implementation for operations like dot products, addition, subtraction, and magnitudes.
    -We use Vector2 for our BoundingBox and Entity implementations.  We use SFML's Vector2f for draw operations (as they do not support just
    passing x and y s into their draw methods).
  -FontContainer
  
    -Our engine includes an automatic font loader which automatically loads fonts from a specified directory.
  -Menu System
  
    -We have implemented a menu system from scratch in our game engine.  The menu system supports many common menu elements such
    as buttons, panels, dynamic and static text, sliders, dropdown lists, and more.  It also supports features specific to our game
    such as a minimap, inventory, menu drawable (a drawable on a menu), and more.
    -The menu system uses a stack/tree style, where elements are attached to other elements, and input is handled starting with the lowest element
    in the tree and then bubbles up.  Rendering is handled by rendering the highest elements down to the lowest.
Main features of our engine that are more specific to our game:
  -Surfaces
  
    -Our game implements a spatial partioning system for our world. Each surface is built up out of a collection of chunks.  Specific to our game,
    not all chunks are loaded immediately, but instead generated as needed using a procedural noise library called FastNoise, however the spatial
    partitioning system would also working pregenerated surfaces, but would require a way to load the pregenerated surface from storage.
    -Lists of entities are kept by each chunk, representing the entities present in that chunk.  In order to solve the issue of entities that are
    larger than a single tile not registering collisions when across a chunk boundary, we also keep track of entities in another list that 
    are colliding with the chunk's bounding box.
 -Procedural generation
 
    -Our procedural generation is partially taken care of by a noise library called FastNoise, however our engine also features an
    implementation of PoissonDiskSampling, a method which creates a procedural distribution of points in a 2d space.
      -This PoissonDiskSampling is used to place entities that will generated in the world, for example: trees, resources.
    -In order to simulate biomes, we use 3 noise maps each representing one of elevation, moisture, and temperature.  Based on these results,
    a tile is selected to be placed in our game world at a specific position by selecting the tile with the smallest difference between said tile's
    preffered noise values and the noise map's values.
    -The map generator also supports the placement of any entity when generating the surface using a class called GeneratorEntityAffinity.  This class
    can place any entity on the map.  In order to prevent runaway terrain generation when an entity is placed on an edge, our engine keeps
    track of entities which require a neighboring chunk to be generated, and finishes initializing said entity when the chunk exists.
 -Fake Lighting
 
    -We have a simple "fake" lighting system in the game.  We have implementated directional and radial lights as part of this system.
      -The lighting system also takes advantage of the spatial partioning system to prevent iterating over every light in the world when drawing.
      -The lighting system just uses alpha multiply blendmode drawing to subtract from a dark layer.
 
 -Prototype system
 
    -Our implementation of entities uses the prototype system to instantiate entities.  Variation of entities are created at the start of the program,
    and then instantiated at runtime by cloning the variation.  This is similar to a factory method but cleans up slightly better.
 -Flyweight system
 
    -Recipes, items, and tiles all use the flyweight pattern in our game engine.  This is to reduce the amount of data required by large numbers
    of items, and tiles.  Retrieving recipes and items is done using a string.  Retrieving tiles is done using a byte, as the chunks in the game use bytes
    to store the tile data.
  -A Star pathing Algorithm

    -Our engine implements A star pathing.  It uses the typical euclidian distance heuristic, but also takes into account each tile's friction/speed modifier.
    
  -Rendering system

    -Our engine implements an orthographic bird's eye view style of rendering, where sprites are layered with sprites farther down the screen overlap sprites farther up the screen.  It also supports several different draw layers including a shadow draw layer, item draw layer, entity draw layer, etc.
    
  -Tile transition system
    
    -Our engine implements a solution to layering different types of tiles in a visually pleasing way.  The algorithm we developed roughly follows the Marching Squares algorithm: https://en.wikipedia.org/wiki/Marching_squares .  The rendering also supports cliffs into the void and beachs into water.  There is 13 different variants of tiles, 12 variants of cliffs and 12 variants of beaches.
