import pygame

class SurfaceKeyFrame:
    def __init__(self, time : float, surface : pygame.Surface):
        self.time = time
        self.surface = surface
        self.animationIndex = -1

    def setAnimationIndex(self, index : int):
        self.animationIndex = index

