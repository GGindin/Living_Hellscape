import pygame

from GameObject import *
from GameCamera import *
from InputHelpers import *

pygame.init();

#https://stackoverflow.com/questions/19954469/how-to-get-the-resolution-of-a-monitor-in-pygame
#monitorInfo = pygame.display.Info()

screenWidth = 800
screenHeight = 800 * (3/4)

playerSpeed = 5.0

screen = pygame.display.set_mode((screenWidth, screenHeight))
clock = pygame.time.Clock();
running = True

dt = 0

playerObj = GameObject(pygame.image.load('Blob.png').convert_alpha())
playerObj.scaleTo(pygame.Vector2(36, 36))
playerObj.setPosition(pygame.Vector2(screenWidth / 2, screenHeight / 2))


gameCamera = GameCamera(pygame.Vector2(0, 0), screen, 0.5)

bg = pygame.image.load('TestTexture.jpg').convert()

while running:
    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            running = False

    dt = clock.tick(60) / 1000

    input = getDirectionInput()
    if input != pygame.Vector2(0, 0):
        input.normalize_ip()
    input *= playerSpeed

    playerObj.move(input)
    gameCamera.boundByInnerBound(playerObj)

    gameCamera.clear("black")
    gameCamera.drawBackground(bg)
    gameCamera.drawGameObject(playerObj)
    gameCamera.getInnerBoundWorldRect()

    pygame.display.flip()

pygame.quit()

