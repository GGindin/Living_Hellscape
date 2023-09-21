import pygame as pg
from pygame.locals import *

from display import *

##############
""" PLAYER """
##############
class Player(pg.sprite.Sprite):

    def __init__(self):
        super(Player, self).__init__()

        self.size = 32

        self.image = pg.Surface( (self.size, self.size), pg.SRCALPHA)
        self.rect = self.image.get_rect()

        self.image.fill(MAX_BLUE)

        self.rect.centerx = SCREEN_W / 2
        self.rect.centery = SCREEN_H / 2

        self.alive = False

    def update(self, pressed_keys):

        # PLAYER CONTROLS
        if pressed_keys[K_UP] or pressed_keys[K_w]:
            self.rect.move_ip(0, -5)

        if pressed_keys[K_DOWN] or pressed_keys[K_s]:
            self.rect.move_ip(0, 5)

        if pressed_keys[K_LEFT] or pressed_keys[K_a]:
            self.rect.move_ip(-5, 0)

        if pressed_keys[K_RIGHT] or pressed_keys[K_d]:
            self.rect.move_ip(5, 0)

        # PLAYER CONSTRAINTS
        if self.rect.x <= 0:
            self.rect.x = 0

        if self.rect.x + self.rect.width >= SCREEN_W:
            self.rect.x = SCREEN_W - self.rect.width

        if self.rect.y <= 0:
            self.rect.y = 0

        if self.rect.y + self.rect.height >= SCREEN_H:
            self.rect.y = SCREEN_H - self.rect.height

        # Unworking exprimental prototype for vector-based movement.
        # Would be essential for diagonal movement that matches the speed of U/D/L/R movement. Disregard for now.
        """
        vel = pg.Vector2()

        if pressed_keys[K_UP] or pressed_keys[K_w]:
            vel.y += -1

        if pressed_keys[K_DOWN] or pressed_keys[K_s]:
            vel.y += 1

        if pressed_keys[K_LEFT] or pressed_keys[K_a]:
            vel.x += -1

        if pressed_keys[K_RIGHT] or pressed_keys[K_d]:
            vel.x += 1

        self.rect.move_ip(vel.normalize() * 5)
        """

""" INSTANTIATION """
player = Player()