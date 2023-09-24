import pygame

#game object class, the docs recommend inheriting from the sprite class but I really do not like how
#the sprite class has a Surface type object called image when pygame also has an Image type. Thats confusing
#so I wrote my own class, and you can do anything that sprites can do with Rects so you lose nothing
#and gain bliss
class GameObject:
    #constructor takes a surface (thats the image rectangle essentially)
    def __init__(self, surface : pygame.Surface):
        #we copy the surface so that we do not have a bunch of things referencing the same object
        self.surface = surface.copy()
        #set world pos to 0
        self.worldPos = pygame.Vector2(0, 0)

    #This and the camera class use lots of things call coordinate spaces. 
    #Generally you have several different spaces: local, world, view, clip, and screen space

    #If you thing about the pixel coordinates of the surface objects thats like local space
    #It doesn't matter where in the world the sprite is row 16 col 16 equals this pixel

    #if you think about where each of those pixels would be in the world, this is world space
    #this is helpful because this is how objects can communicate their local spaces to each other
    #and is how we think of the world

    #then for drawing we need to know where in relation to the camera the object is, this is view space
    # if the object is at (10, 0) but the camera is at (15, 0), the object is at (-5,0) in view space
    #this is what we want because we want to draw the object on the left side of the screen

    #and since we are in 2D clip space isnt really necessary, but I do play to make the camera only
    #draw things that it can see, which is the purpose of clip space, it clips out non visible objects

    #Screen space is essentially are screen surface that the camera tracks, which pygame makes pretty easy
    #with the surface class. This is which pixel on the screen has what thing

    #generally when drawing you take an object in local space and transform it to world > view > clip > screen
    #and then you can draw it

    #any way all of that is to same that we need to be able to get the rect of our sprite in world space 
    #not local space and this function does it
    def getWorldRect(self) -> pygame.Rect:
        rect = self.surface.get_rect()
        rect.move_ip(self.worldPos)
        return rect
    
    #some scale functions for changing the size of te surfaces, we will need to decide on sizes for things
    #at some point
    def scaleTo(self, scale : pygame.Vector2):
        self.surface = pygame.transform.scale(self.surface, scale)

    def scaleBy(self, factor : float):
        self.surface = pygame.transform.scale_by(self.surface, factor)
    
    #some move functions, I plan to put all of these into a different class called transform
    #which will track and provide methods for changing postion, rotation, scale as well as doing space changes
    def move(self, vector : pygame.Vector2):
        self.worldPos += vector

    def setPosition(self, position : pygame.Vector2):
        self.worldPos = position