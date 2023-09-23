import pygame
import numpy
from SurfaceKeyFrame import *
from GameObject import *

class SurfaceAnimation:
    
    def __init__(self):
        self.currentTime = 0.0
        self.currentKeyFrame = 0
        self.duration = 0.0
        self.loop = False
        self.keyFrames = []
        

    def setDuration(self, duration : float):
        self.duration = duration

    def addKeyFrame(self, keyFrame : SurfaceKeyFrame):
        self.keyFrames.append(keyFrame)
        keyFrame.setAnimationIndex(len(self.keyFrames) - 1)

    def getNextKeyFrame(self) -> SurfaceKeyFrame:
        if len(self.keyFrames) <= 0:
            return None

        if self.currentKeyFrame + 1 < len(self.keyFrames):
            return self.keyFrames[self.currentKeyFrame + 1]
        elif self.loop:
            return self.keyFrames[0]
        
        return None

    def update(self, dt : float):
        nextKeyFrame = self.getNextKeyFrame()

        if nextKeyFrame is None:
            print("wtf")
            return

        self.currentTime += dt

        if self.currentTime >= self.duration:
            self.currentTime -= self.duration

        if nextKeyFrame.animationIndex == 0:
            if self.currentTime < self.keyFrames[self.currentKeyFrame].time:
                self.currentKeyFrame = nextKeyFrame.animationIndex 
        elif self.currentTime >= nextKeyFrame.time:
            self.currentKeyFrame = nextKeyFrame.animationIndex        

    def getCurrentSurface(self) -> pygame.Surface:
        if self.currentKeyFrame < len(self.keyFrames):
            return self.keyFrames[self.currentKeyFrame].surface
        
        return None
        