import os.path

import pygame as pg
from pygame.locals import *

SCREEN_W = 1024
SCREEN_H = 768

flags = 0 # pg.FULLSCREEN | pg.RESIZEABLE | pg.SCALED
SCREEN = pg.display.set_mode([SCREEN_W, SCREEN_H], flags, vsync=1)

""" ALL SPRITES GROUP """
floor_tiles = pg.sprite.Group()
all_sprites = pg.sprite.Group()

####################
""" SPRITE SHEET """
####################
class SpriteSheet():
    
    def __init__(self, filename): # Initializes the given strip of images

        self.inSheet = pg.image.load(filename) # Loads uncut sheet into memory
        self.sheetList = [] # List container for our image set
        
    def image_at(self, rectangle): # Returns an image on the strip, inidicated by the input rect object (x, y, w, h)

        rect = pg.Rect(rectangle) # Generates a rect based on the rect argument it was passed
        image = pg.Surface(rect.size, pg.SRCALPHA) # Creates a Surface() based on the rect (using rect.size, which returns a tuple)
                                                   # While we're here, I wanted to touch on pg.SRCALPHA. This allows elements of a surface to be transparent.
                                                   # Otherwise, alpha=0 transparencies will be rendered in straight black, and transparency layering would obviously be impossible.
                                                   # SRCALPHA appears to mean "Source Alpha" - I would imagine this is a flag to indicate that the source image contains alpha values.
                                                   # This is very similar to using 'pg.image.load("image.png").convert_alpha()', which you will see in a lot of tutorials.

        image.blit(self.inSheet, (0,0), rect) # Blits the Surface (thus making it visible)
            
        return image # Returns the now-visible image at the location specified
    
    def unpack(self, div_W, div_H): # Takes the strip of images and cuts them into sections according to div_W/div_H, populates self.sheetList, and then returns the list.
        
        x = 0
        w = self.inSheet.get_rect().width
        while x < w:
            self.sheetList.append( self.image_at( pg.Rect( x, 0, div_W, div_H))) # Uses image_at to append each individual image to self.sheetList.
            x += div_W

        return self.sheetList
    
##################
""" BACKGROUND """
##################

class TileMap():

    def build(self):

        for i in range(0, 8):
            for j in range(0, 6):

                new_tile = Tile( os.path.join( "data", "gfx", "Floorboards.png"), 128 * i, 128 * j)
                floor_tiles.add(new_tile)
                all_sprites.add(new_tile)

class Tile(pg.sprite.Sprite):

    def __init__(self, image, x, y):
        super(Tile, self).__init__()

        self.size = 128
        self.image = pg.image.load(image)
        self.rect = self.image.get_rect()
        self.rect.x = x
        self.rect.y = y


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


#####################
""" INSTANTIATION """
#####################

tilemap = TileMap()