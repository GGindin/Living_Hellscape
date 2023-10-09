from typing import Any
import pygame

class GameObject(pygame.sprite.Sprite):
    def __init__(self, image : pygame.Surface, position : pygame.Vector2 = pygame.Vector2(0, 0)):
        super().__init__()
        self.position = position
        self.forward = pygame.Vector2(0, 0)
        self.rotation = 0
        self.image = image.copy()
        self.drawingImage = self.image
        self.rect = self.image.get_rect()
        self.rect.center = position

    def movePosition(self, delta : pygame.Vector2) -> None:
        self.position += delta

    def setPosition(self, position : pygame.Vector2) -> None:
        self.position = position

    def rotate(self, delta : float) -> None:
        self.rotation += delta

    def setDrawingImage(self) -> None:
        self.drawingImage = pygame.transform.rotate(self.image, self.rotation)
    
    def updateRect(self) -> None:
        self.rect = self.drawingImage.get_rect()
        self.rect.center = self.position

    def getCollisionOverlap(self, other : 'GameObject') -> pygame.Vector2:
        overlap = pygame.Vector2(0, 0)
        diff = self.position - other.position
        
        #we are above
        if(diff.y < 0 and self.rect.bottom > other.rect.top):
            overlap.y += other.rect.top - self.rect.bottom
        #we are below
        elif(diff.y > 0 and self.rect.top < other.rect.bottom):
            overlap.y += other.rect.bottom - self.rect.top

        #we are to the right
        if(diff.x > 0 and self.rect.left < other.rect.right):
            overlap.x += other.rect.right - self.rect.left
        #we are to the left
        elif(diff.x < 0 and self.rect.right > other.rect.left):
            overlap.x += other.rect.left - self.rect.right

        return overlap

    #https://www.sqlshack.com/understanding-args-and-kwargs-arguments-in-python/
    def update(self, *args: Any, **kwargs: Any) -> None:
        self.setDrawingImage()
        self.updateRect()

    def onCollision(self, other : 'GameObject') -> bool:
        pass



