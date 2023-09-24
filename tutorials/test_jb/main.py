# bunch of imports to get modules needed for demo
import pygame

from GameState import *
from GameObject import *
from GameCamera import *
from InputHelpers import *
from SurfaceAnimation import *
from SurfaceKeyFrame import *

#main code needs to call this first, it sets up everything pygame needs to work
pygame.init();

#this is just for getting the monitor resolution, I want to set it up so we can have full screen at a 
#particular aspect ratio 4:3 if we want to go retro
#https://stackoverflow.com/questions/19954469/how-to-get-the-resolution-of-a-monitor-in-pygame
#monitorInfo = pygame.display.Info()

#just setting some basic variables
#a lot of these will eventually live in different files or we will have a variables file for things like 
#screen resolution, etc.

#this is the window resolution, Im using a 4:3 aspect ratio so the height is 3/4ths the width
screenWidth = 800
screenHeight = 800 * (3/4)

#how many pixels/sec the char moves
#We will want to set up some sort of base unit, like a unit equals 5 pixels and then scale that
#to what ever screen resolution we end up using
playerSpeed = 5.0

#this creates our window Surface object that we draw to
#there are lots of different versions of this method, one that makes full screen
#I read about creating a surface seperate from this that we draw to that we could have at the resolution we want
#and then we could scale it to be the monitor resolution and blit that to the final surface target
screen = pygame.display.set_mode((screenWidth, screenHeight))

#adding a room
roomTextrueSurface = pygame.image.load('brick.png').convert_alpha()
room = Room(pygame.Vector2(0, 0), pygame.Vector2(200 * 4, 100 * 4), roomTextrueSurface)
GameState.room = room
GameState.rooms.append(room)

room = Room(pygame.Vector2(200  * 4, 25  * 4), pygame.Vector2(100  * 4, 50  * 4), roomTextrueSurface)
GameState.rooms.append(room)

room = Room(pygame.Vector2(300  * 4, -50  * 4), pygame.Vector2(100  * 4, 250  * 4), roomTextrueSurface)
GameState.rooms.append(room)

room = Room(pygame.Vector2(200  * 4, -200  * 4), pygame.Vector2(300  * 4, 150  * 4), roomTextrueSurface)
GameState.rooms.append(room)

room = Room(pygame.Vector2(0  * 4, 200  * 4), pygame.Vector2(400  * 4, 100  * 4), roomTextrueSurface)
GameState.rooms.append(room)

room = Room(pygame.Vector2(100  * 4, 100  * 4), pygame.Vector2(50  * 4, 100  * 4), roomTextrueSurface)
GameState.rooms.append(room)

#room = Room(roomPos + pygame.Vector2(roomSize.x, roomSize.y / 2), roomSize / 2, roomTextrueSurface)
#GameState.rooms.append(room)

#clock is used for setting the frame rate and getting delta times
#will probably move to a static class
clock = pygame.time.Clock();

#the game is running
running = True

#init a delta time var, once again will move to the static class
dt = 0

#this is creating the player object. It is a little out of step with the animation class
#but originally it took the surface it was going to display for its constructor
#because the animation changes this anyway it isnt really needed but all that still needs more work
#this is a proof of concept
playerObj = GameObject(pygame.image.load('Blob.png').convert_alpha())
playerObj.scaleTo(pygame.Vector2(36, 36))

#center the player, I still want to create an offset that each game object tracks internally
#because in pygame each surface is positioned from its topleft corner. It would be nice if we could
#center them from their center
playerObj.setPosition(pygame.Vector2(0, 0))

#create the game camera, it takes a world space position, the target surface (the screen) and the inner bounding
#ratio. This last thing creates a rectangle that is the screen scaled by this ratio.
#The idea is that when the play is in the center of the screen the camera does perfectly follow the player
#but if they get close to the edges the camera will start to track the player or what ever object
gameCamera = GameCamera(pygame.Vector2(0, 0), screen, 0.5)

#this loading the background for this demo
#this is what I am calling a "SkyBox"
bg = pygame.image.load('TestTexture.jpg').convert()

#main program loop
while running:
    #test for quit
    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            running = False

    #advance the clock by 60 fps, divide by 1000 because the return value is in mili secods
    #this converts it to seconds
    dt = clock.tick(60) / 1000

    #get input vector see InputHelpers for more info
    input = getDirectionInput()
    
    #if the input is not zero, normalize the input vector
    if input != pygame.Vector2(0, 0):
        input.normalize_ip()

    #multiple by player speed to get a velocity
    #Iwant to make all of this input stuff have acceleration to it, not to hard to set up
    input *= playerSpeed

    #moves the player
    playerObj.move(input)

    #this keeps the player in the inner bound of the camera
    #will be working on getting this working with a room object
    #gameCamera.boundByInnerBound(playerObj)
    gameCamera.boundByRoom(playerObj)

    #the draw loop, a lot of this will eventually live in the camera class 
    #and be able to draw groups of objects all at once 

    #clear out the camera target
    gameCamera.clear("black")
    #draw the backgground
    gameCamera.drawBackground(bg)
    #draw room
    for i in GameState.rooms:
        gameCamera.drawRoom(i)
    
    #draw the player
    gameCamera.drawGameObject(playerObj)

    #swap chain, this is how graphics work
    #you have a front and back buffer, you issue all draws to the back buffer and when they are done
    #you swap the buffers, putting the back buffer to the screen, so the everything shows up at once
    pygame.display.flip()

#if out of game loop quit
pygame.quit()

