# EARIN EX4 - Checkers
Checkers game with bot AI using Minimax algorithm with Alpha-Beta pruning.  
Created in Unity and C# by Oskar Hącel & Marcin Lisowski  

Politechnika Warszawska, 04.2021

## Installation
1. Download release package [here](https://github.com/KlivenPL/EARIN_EX4/releases)(```EARIN_EX4_CHECKERS.zip```) or build project yourself
1. Unzip ```.zip``` file with Windows extracting tool, ```7Zip``` or similar
2. Run ```EARIN_EX4_CHECKERS.exe``` to play the game

## Build
1. Download ```Unity 2020.3.0f1 (64-bit)``` game engine. Make sure to install ```Unity Hub``` as well. ```Visual Studio 2019``` highly recommended.
2. Inside Unity Hub, in ```Projects``` tab, press ```Add``` button, and select main folder of the source code (containing e.g. ```.gitignore``` file)
3. Press play button to play the game inside the Editor or use ```Ctrl + b``` to build and play in standalone application

## Gameplay
1. Press ```play``` button
2. Using slider, choose desired ```depth``` parameter. It describes the depth parameter of Minimax algorithm using which AI bot operates (higher value = harder bot to beat)
3. Press the ```play``` button again
4. Player always plays with pawns located on the lower part of the checkerboard
5. Game can be restarted inside ```escape menu``` that appears when ```Escape``` button is pressed. One can also go back to ```Main menu```
6. Player make moves by selecting available desired pawn and then selecting available desired position
   * If pawn cannot be selected it means that player cannot make move with given pawn or AI bot still makes his move
7. Player is notified when he or the bot wins by ```end game``` screen. End game screen allows to restart the game or to quit to ```Main Menu```
