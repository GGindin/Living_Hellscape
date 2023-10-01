import pygame as pg
from pygame.locals import *

SCREEN_W = 1024
SCREEN_H = 768

flags = 0 # pg.FULLSCREEN | pg.RESIZEABLE | pg.SCALED
SCREEN = pg.display.set_mode([SCREEN_W, SCREEN_H], flags, vsync=1)

""" ALL SPRITES GROUP """
all_sprites = pg.sprite.Group()

#####################
""" COLOR PALETTE """
#####################

# BASICS
BLACK = pg.Color(0,0,0)
WHITE = pg.Color(255,255,255)

GRAPHITE = pg.Color(50, 50, 50)
DARK_GREY = pg.Color(100, 100, 100)
GREY = pg.Color(130, 130, 130)
LIGHT_GREY = pg.Color(160, 160, 160)
SILVER = pg.Color(200, 200, 200)

MAX_RED = pg.Color(255, 0, 0)
MAX_GREEN = pg.Color(0, 255, 0)
MAX_BLUE = pg.Color(0, 0, 255)

MAX_YELLOW = pg.Color(255, 255, 0)
MAX_CYAN = pg.Color(0, 255, 255)
MAX_MAGENTA = pg.Color(255, 0, 255)

# CUSTOM COLORS
# Recommended convention: THING_COLOR
BLOOD_RED = pg.Color(150, 0, 20)
GRASS_GREEN = pg.Color(80, 180, 30)
PAPER_WHITE = pg.Color(230, 230, 200)