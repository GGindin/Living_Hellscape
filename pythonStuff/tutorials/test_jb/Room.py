import pygame

class Room:
    def __init__(self, worldPos : pygame.Vector2, dimensions : pygame.Vector2, textureSurface : pygame.Surface):
        self.worldPos = worldPos
        self.surface = pygame.Surface(dimensions)

        for x in range(0, int(dimensions.x), textureSurface.get_width()):
            for y in range(0, int(dimensions.y), textureSurface.get_height()):
                self.surface.blit(textureSurface, pygame.Vector2(x, y))

    def getWorldRect(self) -> pygame.Rect:
        rect = self.surface.get_rect()
        rect.move_ip(self.worldPos)
        return rect
    
    def getLocalRect(self) -> pygame.Rect:
        return self.surface.get_rect()