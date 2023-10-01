import pygame as pg
from pygame.locals import *

from display import *
from logic import *
from player import *
from enemies import *

clock = pg.time.Clock()

enemiesPlaced = False

while game.RUNNING == True:
    SCREEN.fill(BLACK)

    pressed_keys = pg.key.get_pressed()

    if player.alive == False:
        all_sprites.add(player)
        player.alive = True

    if enemiesPlaced == False:
        rambler1 = Rambler(SCREEN_W * 0.25, SCREEN_H * 0.25)
        rambler2 = Rambler(SCREEN_W * 0.25, SCREEN_H * 0.75)
        rambler3 = Rambler(SCREEN_W * 0.75, SCREEN_H * 0.25)
        rambler4 = Rambler(SCREEN_W * 0.75, SCREEN_H * 0.75)
        enemy_sprites.add(rambler1, rambler2, rambler3, rambler4)
        all_sprites.add(rambler1, rambler2, rambler3, rambler4)
        enemiesPlaced = True

    for event in pg.event.get():

        if event.type == KEYDOWN:

            if event.key == K_ESCAPE:
                game.RUNNING = False

        if event.type == pg.QUIT:
            game.RUNNING = False

    updater.process(pressed_keys)
    pg.display.flip()
    clock.tick(60)

pg.quit()