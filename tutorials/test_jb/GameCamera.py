import pygame
import math
import numpy

from GameObject import *


class GameCamera:
    def __init__(self, worldPos : pygame.Vector2, screen : pygame.Surface, innerBoundFactor : float):
        self.worldPos = worldPos
        self.screen = screen
        innerBoundFactor = numpy.clip(innerBoundFactor, 0.0, 1.0)
        self.innerBoundFactor = innerBoundFactor
 

    def getScreenWorldRect(self) -> pygame.Rect: 
        rect = self.screen.get_rect()
        rect.move_ip(self.worldPos)
        return rect
    
    def getInnerBoundWorldRect(self) -> pygame.Rect:
        screenWorldRect = self.screen.get_rect()
        innerBoundWorldRect = screenWorldRect.scale_by(self.innerBoundFactor, self.innerBoundFactor)
        innerBoundWorldRect.move_ip(self.worldPos)
        return innerBoundWorldRect


    def move(self, vector : pygame.Vector2):
        self.worldPos += vector

    def setPosition(self, position : pygame.Vector2):
        self.worldPos = position

    def focus(self, focus : GameObject):
        self.worldPos = focus.worldPos

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

    def clear(self, color : pygame.Color):
        self.screen.fill(color)

    def worldToViewPos(self, worldPos : pygame.Vector2) -> pygame.Vector2:
        return worldPos - self.worldPos

    def drawGameObject(self, gameObject : GameObject):
        viewPos = self.worldToViewPos(gameObject.worldPos)
        self.screen.blit(gameObject.surface, viewPos)

    def drawBackground(self, backGround : pygame.Surface):
        bgDims = pygame.Vector2(backGround.get_width(), backGround.get_height())

        xStart = math.floor(self.worldPos.x / bgDims.x)
        yStart = math.floor(self.worldPos.y / bgDims.y)

        xEnd = math.floor((self.worldPos.x + self.screen.get_width()) / bgDims.x)
        yEnd = math.floor((self.worldPos.y + self.screen.get_height()) / bgDims.y)

        for x in range(xStart, xEnd + 1):
            for y in range(yStart, yEnd + 1):
                tilePos = pygame.Vector2(bgDims.x * x, bgDims.y * y)
                tilePos = self.worldToViewPos(tilePos)
                self.screen.blit(backGround, tilePos)



    