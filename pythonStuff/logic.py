import pygame as pg
from pygame.locals import *

from player import *
from enemies import *

##################
""" GAME STATE """
##################

class GameState():

    RUNNING = True
    state = "active"


##################
""" COLLISIONS """
##################

class Collisions():

    # PLAYER & ENEMY
    def player_w_enemy(self):

        if pg.sprite.spritecollide(player, enemy_sprites, False, pg.sprite.collide_mask):
            player.kill()

    # PLAYER & ENEMY WEAPON (Projectiles)

    # PLAYER & BOUNDARY

    # ENEMY & PLAYER WEAPON

    # ENEMY & BOUNDARY

    # PROCESS
    def process(self):

        self.player_w_enemy()
        

######################
""" UPDATE MANAGER """
######################

class UpdateManager():

    def __init__(self):

        self.ticks = 0
        self.seconds = 0

    def process(self, pressed_keys):

        self.ticks += 1
        if self.ticks % 60 == 0:
            self.seconds += 1

        player.update(pressed_keys)
        enemy_sprites.update()

        collisions.process()

        for item in all_sprites:
            SCREEN.blit(item.image, item.rect)


#####################
""" INSTANATIATION"""
#####################

game = GameState()
collisions = Collisions()
updater = UpdateManager()