import pygame
import math
import numpy

from GameObject import *
from GameState import *
from Room import *

#Camera object, I created this because it makes it way easier to have things moving around if 
#we have a camera that we can just move around also and then draw what is in front of it
#pretty much universal in game dev
class GameCamera:
    #constuctor, I explain most of this in the main file
    def __init__(self, worldPos : pygame.Vector2, screen : pygame.Surface, innerBoundFactor : float):
        self.worldPos = worldPos
        self.screen = screen
        innerBoundFactor = numpy.clip(innerBoundFactor, 0.0, 1.0)
        self.innerBoundFactor = innerBoundFactor
 
    #this get the world space position of the screen rectangle, or our viewing port of the camera
    #in the future I will use this to clip out objects that are not on screen
    #it is also necessary to know this for drawing objects, see the space discussion in the gameobject file
    def getScreenWorldRect(self) -> pygame.Rect: 
        rect = self.screen.get_rect()
        rect.move_ip(self.worldPos)
        return rect
    
    def getScreenLocalRect(self) -> pygame.Rect:
        return self.screen.get_rect()
    
    #get the inner bounding rect, this is explained in the main file
    def getInnerBoundWorldRect(self) -> pygame.Rect:
        screenWorldRect = self.screen.get_rect()
        innerBoundWorldRect = screenWorldRect.scale_by(self.innerBoundFactor, self.innerBoundFactor)
        innerBoundWorldRect.move_ip(self.worldPos)
        return innerBoundWorldRect
    
    def getInnerBoundLocalRect(self) -> pygame.Rect:
        return pygame.Rect(0, 0, self.screen.get_width() * self.innerBoundFactor, self.screen.get_height() * self.innerBoundFactor)

    #some move functions, see comments in gameobject file
    def move(self, vector : pygame.Vector2):
        self.worldPos += vector

    def setPosition(self, position : pygame.Vector2):
        self.worldPos = position

    def focus(self, focus : GameObject):
        self.worldPos = focus.worldPos

    #this takes a game object and moves the camera so that the object is complete on the screen
    #it mostly just looks at the edges of the screen and the object and if one is to far in any direction
    #it moves the camera to keep it on the screen
    def boundByScreen(self, boundable : GameObject):
        rect = self.getScreenWorldRect()
        boundRect = boundable.getWorldRect()
        if not rect.contains(boundRect):
            offset = pygame.Vector2(0, 0)
            if boundRect.top < rect.top:
                dif = boundRect.top - rect.top
                offset += pygame.Vector2(0, dif);
            if boundRect.right > rect.right:
                dif = boundRect.right - rect.right
                offset += pygame.Vector2(dif, 0);
            if boundRect.bottom > rect.bottom:
                dif = boundRect.bottom - rect.bottom
                offset += pygame.Vector2(0, dif);
            if boundRect.left < rect.left:
                dif = boundRect.left - rect.left
                offset += pygame.Vector2(dif, 0);
            self.worldPos += offset

    #same as above but for the inner bounding rect
    def boundByInnerBound(self, boundable : GameObject):
        rect = self.getInnerBoundWorldRect()
        boundRect = boundable.getWorldRect()
        if not rect.contains(boundRect):
            offset = pygame.Vector2(0, 0)
            if boundRect.top < rect.top:
                dif = boundRect.top - rect.top
                offset += pygame.Vector2(0, dif);
            if boundRect.right > rect.right:
                dif = boundRect.right - rect.right
                offset += pygame.Vector2(dif, 0);
            if boundRect.bottom > rect.bottom:
                dif = boundRect.bottom - rect.bottom
                offset += pygame.Vector2(0, dif);
            if boundRect.left < rect.left:
                dif = boundRect.left - rect.left
                offset += pygame.Vector2(dif, 0);
            self.worldPos += offset

    #a lot of this bounding could be better done with the rect.clip method i think
    def boundByRoom(self, boundable : GameObject):
        #always bound by inner first so that the camera is locked on the player
        self.boundByInnerBound(boundable)

        #then if there is a room figure out how to lock the camera in the room
        #this is a way that we could keep the camera view port in the room
        #but the player is also bound by the room so currently it isnt needed
        #when we have walls and doors we will probably want to stop the camera at the walls so
        #you cant see into the other rooms, or just do not draw the other rooms until you are in them
        """if GameState.room is not None:
            localScreenRect = self.getScreenLocalRect()
            localScreenInnerRect = self.getInnerBoundLocalRect()
            localRoomRect = GameState.room.getLocalRect()          
            #is the room bigger or smaller than the inner bound
            #this is smaller or same size, so get set the camera to focus the middle of the room
            if localScreenInnerRect.contains(localRoomRect):
                #first move the so the rects line on in topleft
                worldPosDif = GameState.room.worldPos - self.worldPos
                self.worldPos += worldPosDif
                #then calc how much to move and move
                xHalfDiff = (localScreenRect.width - localRoomRect.width) / 2.0
                yHalfDiff = (localScreenRect.height - localRoomRect.height) / 2.0
                offset = pygame.Vector2(-xHalfDiff, -yHalfDiff)
                self.worldPos += offset
            #the room is bigger in some way than the screen inner bound so more work to figure out how to bound
            else:
                if localRoomRect.width < localScreenInnerRect.width:
                    #if room width is smaller we want to line the screen up with the center of 
                    #the room vertically and then just follow the character horizontally
                    worldXDiff = GameState.room.worldPos.x - self.worldPos.x
                    self.worldPos.x += worldXDiff
                    xHalfDiff = (localScreenRect.width - localRoomRect.width) / 2.0
                    offset = pygame.Vector2(-xHalfDiff, 0)
                    self.worldPos += offset
                if localRoomRect.height < localScreenInnerRect.height:
                    #if room height is smaller we want to line the screen up with the center of 
                    #the room horizontally and then just follow the character vertically
                    worldYDiff = GameState.room.worldPos.y - self.worldPos.y
                    self.worldPos.y += worldYDiff
                    yHalfDiff = (localScreenRect.height - localRoomRect.height) / 2.0
                    offset = pygame.Vector2(0, -yHalfDiff)
                    self.worldPos += offset"""
                


    #this clears the camera target with a color
    def clear(self, color : pygame.Color):
        self.screen.fill(color)

    #transforms a world space position to a view space position
    def worldToViewPos(self, worldPos : pygame.Vector2) -> pygame.Vector2:
        return worldPos - self.worldPos

    #draws the object ot the screen surface
    def drawGameObject(self, gameObject : GameObject):
        viewPos = self.worldToViewPos(gameObject.worldPos)
        self.screen.blit(gameObject.surface, viewPos)

    def drawRoom(self, room : Room):
        if room is None:
            return
        viewPos = self.worldToViewPos(room.worldPos)
        self.screen.blit(room.surface, viewPos)

    #draws the background in an infinite way 
    def drawBackground(self, backGround : pygame.Surface):
        #get the dimensions of the background image
        bgDims = pygame.Vector2(backGround.get_width(), backGround.get_height())

        #these calculations tell us the tile start position of the background
        #ex: camera at (20, 20), bgDims = (800, 800) we need to start our draw at (0, 0) to cover the
        #area we are currently looking at
        xStart = math.floor(self.worldPos.x / bgDims.x)
        yStart = math.floor(self.worldPos.y / bgDims.y)

        #same as above but we account for how big the screen is to know the last tile position
        xEnd = math.floor((self.worldPos.x + self.screen.get_width()) / bgDims.x)
        yEnd = math.floor((self.worldPos.y + self.screen.get_height()) / bgDims.y)

        #for loop through the x and y tile posiitons
        for x in range(xStart, xEnd + 1):
            for y in range(yStart, yEnd + 1):
                #scale the tile positions byt the tile dimensions to get world space position of tiles
                tilePos = pygame.Vector2(bgDims.x * x, bgDims.y * y)
                #convert to view space
                tilePos = self.worldToViewPos(tilePos)
                #draw to screen
                self.screen.blit(backGround, tilePos)



    