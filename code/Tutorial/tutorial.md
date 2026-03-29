1. Adding levels
In the tutorial.scene go to the Tutorial Levels object
This is the parent object of all the levels, and has the TutorialGameMode component
When you add a new level, remember to add it to here

2. Tutorial Stage
Add a new empty object inside tutorial levels and name it “Level [n]”
Add the Tutorial Stage component to this
This component holds all the information related to that level

2.1  Instructions
Add the instructions you need and also add a media file if you want. (Assets/Media/…)
The last instruction unlocks ONLY when the objective is completed

2.2 Level Setup
Add the spawn point, 
Checkpoints if you need them
Reset trigger: Resets to the start/last checkpoint if the player hits it
The ResetFloor object in TutorialScene covers the main ground of the map (not perfect)
You can also make your own triggers

IMPORTANT! All objects/checkpoints/etc. that are level specific, place them inside the level object
Also disable them in the inspector (power btn)
All child objects of the tutorial level objects get enabled when the level starts (and disabled when it ends)

3. Tutorial Page
Add new level to the tutorial page in Mainmenu.scene
Add level name, description, media and most importantly the correct level index
When you’re done, right click on the component and copy it
Then go to Assets/scenes/playerScene.scene and select TutorialPage object
Right click the component and “paste values”
This is the same exact page but in the pause menu

also in pdf with images:
https://seafile.utu.fi/f/3c6f49028df544d196ef/



