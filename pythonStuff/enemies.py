import random as rand

import pygame as pg
from pygame.locals import *

from display import *
from player import *

""" ENEMY SPRITES GROUP """
enemy_sprites = pg.sprite.Group()

class Wanderer(pg.sprite.Sprite):
    def __init__(self, startX, startY):
        super(Wanderer, self).__init__()

        self.size = 32
        self.imageSet = SpriteSheet( os.path.join( "data", "gfx", "EnemyGhostProto.png")).unpack(self.size, self.size)
        self.image = self.imageSet[0]
        self.imageSet.append( pg.transform.flip( self.imageSet[1], True, False))

        self.rect = self.image.get_rect()
        self.rect.centerx = startX
        self.rect.centery = startY

        self.waiting = True
        self.waitInterval = 60
        self.waitTimer = 0

        self.speed = 4
        self.direction = 0
        self.currentMove = 0
        self.moveAmount = 0

    def update(self):

        if self.waiting == True:

            if self.waitTimer >= self.waitInterval:

                self.moveAmount = 32 * rand.randint(1, 6)
                self.direction = rand.randint(0, 3)
                self.waiting = False

            else:
                self.waitTimer += 1

        if self.waiting == False:

            if self.currentMove < self.moveAmount:
                self.currentMove += self.speed

                match(self.direction):

                    # Clockwise from bottom
                    case 0:
                        if self.image != self.imageSet[0]:
                            self.image = self.imageSet[0]

                        self.rect.move_ip(0, self.speed)

                    case 1:
                        if self.image != self.imageSet[1]:
                            self.image = self.imageSet[1]

                        self.rect.move_ip(-self.speed, 0)

                    case 2:
                        if self.image != self.imageSet[2]:
                            self.image = self.imageSet[2]

                        self.rect.move_ip(0, -self.speed)

                    case 3:
                        if self.image != self.imageSet[3]:
                            self.image = self.imageSet[3]

                        self.rect.move_ip(self.speed, 0)

            else:
                self.waiting = True
                self.waitTimer = 0
                self.currentMove = 0
                self.moveAmount = 0

        if self.rect.x <= 0:
            self.rect.x = 0

        if self.rect.x + self.rect.width >= SCREEN_W:
            self.rect.x = SCREEN_W - self.rect.width

        if self.rect.y <= 0:
            self.rect.y = 0

        if self.rect.y + self.rect.height >= SCREEN_H:
            self.rect.y = SCREEN_H - self.rect.height


class Rambler(pg.sprite.Sprite):

    def __init__(self, startX, startY):
        super(Rambler, self).__init__()

        self.size = (32, 32)

        self.image = pg.Surface(self.size, pg.SRCALPHA)
        self.image.fill(MAX_RED)

        self.rect = self.image.get_rect()
        self.rect.centerx = startX
        self.rect.centery = startY

        self.speed = 4

    def update(self):

        # Random movement algorithm - random integer from 0-3 determines 0) Up 1) Right 2) Down 3) Left (clockwise).
        # Everything else (4-20) does nothing. Pads out the movement so they're not as jarringly shaky.
        num = rand.randint(0, 20)

        if num == 0:
            self.rect.move_ip(0, -self.speed)

        elif num == 1:
            self.rect.move_ip(self.speed, 0)

        elif num == 2:
            self.rect.move_ip(0, self.speed)

        elif num == 3:
            self.rect.move_ip(-self.speed, 0)

        # WINDOW CONSTRAINTS
        if self.rect.x <= 0:
            self.rect.x = 0

        if self.rect.x + self.rect.width >= SCREEN_W:
            self.rect.x = SCREEN_W - self.rect.width

        if self.rect.y <= 0:
            self.rect.y = 0

        if self.rect.y + self.rect.height >= SCREEN_H:
            self.rect.y = SCREEN_H - self.rect.height