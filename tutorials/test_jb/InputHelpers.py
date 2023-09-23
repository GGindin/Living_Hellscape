import pygame

def getDirectionInput():
    keys = pygame.key.get_pressed()
    input = pygame.Vector2(0, 0)
    if keys[pygame.K_w]:
        input.y -= 1
    if keys[pygame.K_s]:
        input.y += 1
    if keys[pygame.K_a]:
        input.x -= 1
    if keys[pygame.K_d]:
        input.x += 1
    return input