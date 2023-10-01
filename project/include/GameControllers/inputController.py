import pygame
import copy

from include.structs.input import *

class InputController:
    input = Input()

    def pollInput() -> None: 
        keys = pygame.key.get_pressed()
        InputController.input.rawDirection = pygame.Vector2(0, 0)
        if keys[pygame.K_w]:
            InputController.input.rawDirection.y -= 1
        if keys[pygame.K_s]:
            InputController.input.rawDirection.y += 1
        if keys[pygame.K_a]:
            InputController.input.rawDirection.x -= 1
        if keys[pygame.K_d]:
            InputController.input.rawDirection.x += 1

        InputController.input.escape = True if keys[pygame.K_ESCAPE] == True else False 

        if(InputController.input.rawDirection.magnitude_squared() > 0.0):
            InputController.input.normDirection = InputController.input.rawDirection.normalize()
        else:
            InputController.input.normDirection = pygame.Vector2(0,0)

    def getInput() -> Input:
        return copy.copy(InputController.input)