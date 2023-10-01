from typing import Any
import pygame

from include.GameObject import GameObject
from include.GameControllers.inputController import InputController
from include.GameControllers.pygameController import PygameController
from include.GameObject.GameObject import GameObject
from include.GameControllers.ObjectManager import *

class PlayerObject(GameObject):

    SPEED = 5.0

    def __init__(self, image : pygame.Surface, position : pygame.Vector2 = pygame.Vector2(0, 0)):
        super().__init__(image, position)
        self.weapon = None

    def giveWeapon(self, image : pygame.surface):
        self.weapon = image.copy()

    def update(self, *args: Any, **kwargs: Any) -> None:
        input = InputController.getInput()
        self.movePosition(input.normDirection * PygameController.Delta_Time * PygameController.PIXELS_PER_UNIT * PlayerObject.SPEED)     
        if(input.normDirection != pygame.Vector2(0, 0)):
            self.forward = input.normDirection     
        super().update()

    def onCollision(self, other: GameObject) -> bool:
        overlap = self.getCollisionOverlap(other)

        #only move the smallest amount
        if(abs(overlap.x) < abs(overlap.y)):
            overlap.y = 0
        else:
            overlap.x = 0

        self.movePosition(overlap)
        return False
