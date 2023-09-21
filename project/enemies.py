import random as rand

import pygame as pg
from pygame.locals import *

from display import *
from player import *

""" ENEMY SPRITES GROUP """
enemy_sprites = pg.sprite.Group()

class Rambler(pg.sprite.Sprite):

    def __init__(self, startX, startY):
        super(Rambler, self).__init__()

        self.size = 32
        
        self.image = pg.Surface( (self.size, self.size), pg.SRCALPHA)
        self.image.fill(MAX_RED)
        self.rect = self.image.get_rect()
        self.rect.centerx = startX
        self.rect.centery = startY

    def update(self):

        # Random movement algorithm - random integer from 0-3 determines 0) Up 1) Right 2) Down 3) Left (clockwise).
        # Everything else (4-20) does nothing. Pads out the movement so they're not as jarringly shaky.
        num = rand.randint(0, 20)

        if num == 0:
            self.rect.move_ip(0, -5)

        elif num == 1:
            self.rect.move_ip(5, 0)

        elif num == 2:
            self.rect.move_ip(0, 5)

        elif num == 3:
            self.rect.move_ip(-5, 0)

        # WINDOW CONSTRAINTS
        if self.rect.x <= 0:
            self.rect.x = 0

        if self.rect.x + self.rect.width >= SCREEN_W:
            self.rect.x = SCREEN_W - self.rect.width

        if self.rect.y <= 0:
            self.rect.y = 0

        if self.rect.y + self.rect.height >= SCREEN_H:
            self.rect.y = SCREEN_H - self.rect.height