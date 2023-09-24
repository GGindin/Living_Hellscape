import pygame

#this is pretty straight forward, if a direction is getting pressed we set the direction value 
#stays between -1 and 1 for each axis
#later I will add some smoothing to these inputs so they arent immediate changes
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