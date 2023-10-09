import pygame

#key frame object for the animation system
class SurfaceKeyFrame:
    #tell it an update time, and the surface to display
    def __init__(self, time : float, surface : pygame.Surface):
        self.time = time
        self.surface = surface
        self.animationIndex = -1

    #when added to an animation it get the index of itself in the animation
    def setAnimationIndex(self, index : int):
        self.animationIndex = index

