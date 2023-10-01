import pygame

class Input:
    def __init__(self):
        self.rawDirection = pygame.Vector2()
        self.normDirection = pygame.Vector2()
        self.escape = False