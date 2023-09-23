import pygame

class GameObject:
    def __init__(self, surface : pygame.Surface):
        self.surface = surface.copy()
        self.worldPos = pygame.Vector2(0, 0)

    def getWorldRect(self) -> pygame.Rect:
        rect = self.surface.get_rect()
        rect.move_ip(self.worldPos)
        return rect
    
    def scaleTo(self, scale : pygame.Vector2):
        self.surface = pygame.transform.scale(self.surface, scale)

    def scaleBy(self, factor : float):
        self.surface = pygame.transform.scale_by(self.surface, factor)
    
    def move(self, vector : pygame.Vector2):
        self.worldPos += vector

    def setPosition(self, position : pygame.Vector2):
        self.worldPos = position