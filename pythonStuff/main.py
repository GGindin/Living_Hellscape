<<<<<<< HEAD
import pygame
import os

#lots of ways for including local files
#using the init method
#https://stackoverflow.com/questions/2349991/how-do-i-import-other-python-files
from include.GameControllers.IOController import *
from include.GameControllers.pygameController import *
from include.GameControllers.ObjectManager import *
from include.GameObject.playerObject import *
from include.GameObject.GameObject import *

IOController(os.path.dirname(__file__))
PygameController()
ObjectManager()

go = PlayerObject(IOController.getImage("art", "blob.png"))
ObjectManager.instance.addGameObjectToGroup(go, ObjectManager.instance.playerGroup)

go = GameObject(IOController.getImage("art", "Bat.png"), pygame.Vector2(200, 200))
ObjectManager.instance.addGameObjectToGroup(go, ObjectManager.instance.objectGroup)

<<<<<<< HEAD:project/main.py
while(PygameController.instance.running):
    PygameController.update()
=======
=======
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

>>>>>>> main
    pressed_keys = pg.key.get_pressed()

    if player.alive == False:
        
        tilemap.build()
        all_sprites.add(player)
        player.alive = True

    if enemiesPlaced == False:
        wanderer1 = Wanderer(SCREEN_W * 0.25, SCREEN_H * 0.25)
        wanderer2 = Wanderer(SCREEN_W * 0.25, SCREEN_H * 0.75)
        wanderer3 = Wanderer(SCREEN_W * 0.75, SCREEN_H * 0.25)
        wanderer4 = Wanderer(SCREEN_W * 0.75, SCREEN_H * 0.75)
        enemy_sprites.add(wanderer1, wanderer2, wanderer3, wanderer4)
        all_sprites.add(wanderer1, wanderer2, wanderer3, wanderer4)
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

<<<<<<< HEAD
pg.quit()
>>>>>>> main:pythonStuff/main.py
=======
pg.quit()
>>>>>>> main
