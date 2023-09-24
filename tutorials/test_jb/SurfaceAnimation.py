import pygame
import numpy
from SurfaceKeyFrame import *
from GameObject import *


#this is the start of a animation library that we can use, it still needs a lot of work
#but it is a great proof of concept
class SurfaceAnimation:
    
    #set up a bunch of vars in the contructor
    def __init__(self):
        self.currentTime = 0.0
        self.currentKeyFrame = 0
        self.duration = 0.0
        self.loop = False
        self.keyFrames = []
        
    #set how long the total animation lasts for
    def setDuration(self, duration : float):
        self.duration = duration

    #add a key frame to the animation, each surfaceKeyFrame has a surface (sprite) and a time that it occurs at
    #see that file for more info
    def addKeyFrame(self, keyFrame : SurfaceKeyFrame):
        #add to the list and tell the key frame which index it is
        self.keyFrames.append(keyFrame)
        keyFrame.setAnimationIndex(len(self.keyFrames) - 1)

    #get the next key frame we need to know this to check if it is time to switch key frames
    def getNextKeyFrame(self) -> SurfaceKeyFrame:
        if len(self.keyFrames) <= 0:
            return None

        if self.currentKeyFrame + 1 < len(self.keyFrames):
            return self.keyFrames[self.currentKeyFrame + 1]
        elif self.loop:
            return self.keyFrames[0]
        
        return None

    #update the animation. We get the next key frame, advance the time of the animation
    #and then check if it is time to switch keyframes
    def update(self, dt : float):
        nextKeyFrame = self.getNextKeyFrame()

        if nextKeyFrame is None:
            return

        #update time
        self.currentTime += dt

        #restart time if past duration
        if self.currentTime >= self.duration:
            self.currentTime -= self.duration

        #check if its time for next key frame, if the next frame index is 0 the check is kind of backwards
        #because we have looped back around
        if nextKeyFrame.animationIndex == 0:
            if self.currentTime < self.keyFrames[self.currentKeyFrame].time:
                self.currentKeyFrame = nextKeyFrame.animationIndex 
        elif self.currentTime >= nextKeyFrame.time:
            self.currentKeyFrame = nextKeyFrame.animationIndex        

    #get the current keyframe surface
    def getCurrentSurface(self) -> pygame.Surface:
        if self.currentKeyFrame < len(self.keyFrames):
            return self.keyFrames[self.currentKeyFrame].surface
        
        return None
        