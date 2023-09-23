import pygame

from GameObject import *
from GameCamera import *
from InputHelpers import *
from SurfaceAnimation import *
from SurfaceKeyFrame import *

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

blobSurface = pygame.image.load('Blob.png').convert_alpha()
blobSurface = pygame.transform.scale(blobSurface, pygame.Vector2(36, 36))

batSurface = pygame.image.load('Bat.png').convert_alpha()
batSurface = pygame.transform.scale(batSurface, pygame.Vector2(36, 36))

bubbleSurface = pygame.image.load('Bubble.png').convert_alpha()
bubbleSurface = pygame.transform.scale(bubbleSurface, pygame.Vector2(36, 36))

snakeSurface = pygame.image.load('snake.png').convert_alpha()
snakeSurface = pygame.transform.scale(snakeSurface, pygame.Vector2(36, 36))

blobKey = SurfaceKeyFrame(0.0, blobSurface)
batKey = SurfaceKeyFrame(0.5, batSurface)
bubbleKey = SurfaceKeyFrame(1.0, bubbleSurface)
snakeKey = SurfaceKeyFrame(1.5, snakeSurface)

animation = SurfaceAnimation()
animation.setDuration(2.0)
animation.loop = True
animation.addKeyFrame(blobKey)
animation.addKeyFrame(batKey)
animation.addKeyFrame(bubbleKey)
animation.addKeyFrame(snakeKey)

playerObj = GameObject(pygame.image.load('Blob.png').convert_alpha())
#playerObj.scaleTo(pygame.Vector2(36, 36))
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

    animation.update(dt)
    animSurf = animation.getCurrentSurface()
    if animSurf is not None:
        playerObj.surface = animation.getCurrentSurface()
    """else:
        print("Current: " + str(animation.currentTime))
        print("Duration: " + str(animation.duration))"""


    gameCamera.boundByInnerBound(playerObj)

    gameCamera.clear("black")
    gameCamera.drawBackground(bg)
    gameCamera.drawGameObject(playerObj)
    gameCamera.getInnerBoundWorldRect()

    pygame.display.flip()

pygame.quit()

