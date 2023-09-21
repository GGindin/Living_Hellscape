"""
  /////////////////////////////////
 ///  BARE-BONES PGYAME MODEL  ///
/////////////////////////////////
"""
"""
This is a very basic demonstration and explanation of the essential components and minimumum requirements to get a game online and running using Pygame.
We can use this as a boilerplate for our initial basic prototype, but we will eventually need to split the programming across several files that reference one another.
This will keep us from writing one singular, 97,231-line document, and obviously, that will make our code easier to reference, repair, and modify.
We will be writing those separate files, and then importing them to their adjacent documents as needed, in the same way that we import modules and libraries.
"""

# Imports. For those who don't know, Python operates on numerous modules/libraries that we import into the program.
# This is very much the same as Java, but without the crap.poop.crud.WhatYouActuallyWanted syntax.
# Additionally, modules can be imported as a shorthand alias to keep things wonderfully simple.
import random as rand

# Pygame must be imported into the program before it can be referenced.
# I typically import pygame as "pg" - more shorthand, easier to wield. We'll be using it a lot.
import pygame as pg
from pygame.locals import * # Not completely clear what this means (I will in time), but it's wholly necessary for keypresses to be recognized.
                            # IMPORTANT: Do not try to write this as pg.locals, even if you just imported pygame as "pg" - it will not work, and I don't know why (yet).

# Designated color objects. We can construct these as we go; they will help with color consistency, and will eliminate much of the need for custom colors.
# Artistically-speaking, I believe that we should follow these colors we designate, and use them as a color guide in the pixel art we create.
# It also technically makes things faster to have the color objects loaded into memory only once for other objects to reference.
# This is in lieu of loading new (same) colors into memory each time they are needed.
MAX_RED = pg.Color(255, 0, 0)
MAX_GREEN = pg.Color(0, 255, 0)
MAX_BLUE = pg.Color(0, 0, 255)
MAX_YELLOW = pg.Color(255, 255, 0)
MAX_CYAN = pg.Color(0, 255, 255)
MAX_MAGENTA = pg.Color(255, 0, 255)
BLACK = pg.Color(0,0,0)
WHITE = pg.Color(255,255,255)
GRASS_GREEN = pg.Color(80, 180, 30)

# Screen dimensions. Useful not just for setting up the main display, but also for positioning.
# For example, if we want an object to start from the right side of the screen, we can use SCREEN_W instead of manually typing in the position.
SCREEN_W = 1024
SCREEN_H = 768

# Flags are tags we can give to the set_mode() function below, which include things like FULLSCREEN, RESIZEABLE, and SCALABLE.
# We'll be adding these later-on, but for now we'll put down '0' to tell the compiler that we have no flags.
flags = 0

# Screen display. Sets the parameters of the main drawing surface. For those who know Java FX, you can think of this as the Main Stage.
# I recommend looking at display.set_mode() in the Pygame CE documentation, if for no other reason than to understand the arguments.
SCREEN = pg.display.set_mode([SCREEN_W, SCREEN_H], flags, vsync=1)

# Sprite groups. These categorize the various sprites on the screen, differentiating them between the player, enemies, objects, projectiles, etc.
# When done like this, we can set collision rules for "player_weapons" and "enemy_sprites", or "player" and "enemy_weapons", just as an example.
enemy_sprites = pg.sprite.Group()
all_sprites = pg.sprite.Group()

# Player sprite class. This is obviously the player object that is currently controlled with the keyboard.
class Player(pg.sprite.Sprite):

    # Initialize function. Derived from the pg.sprite.Sprite() superclass.
    # This function determines base variables for the spite object, such as its image, its rect (sprite bounds), and its starting position.
    # More or less, it is the constructor for the sprite object.
    # At minimum, a sprite MUST have an image and a rect defined. You'll see why later.
    def __init__(self):
        super(Player, self).__init__() # This is to designate the sprite as a subclass of the pg.sprite.Sprite() class (thus granting us use of the self.image and self.rect variables).

        self.size = 32 # "self" is the python equivalent of "this". It's weird, I know - you'll get used to it.

        # pg.Surface() objects are essential to pygame - these are the surfaces that our various images will be drawn on, which will then be added to the main surface.
        # This right now is simply a surface filled with a background color, but later on, these surfaces can be created directly from the images themselves.
        self.image = pg.Surface( (self.size, self.size), pg.SRCALPHA)
        self.rect = self.image.get_rect()

        # Color fill value for the player surface.
        self.image.fill(MAX_BLUE)

        # Starting position. Not necessary, but recopmmended; otherwise the sprite will first appear at position (0,0) (upper-left corner).
        self.rect.centerx = SCREEN_W / 2
        self.rect.centery = SCREEN_H / 2

        # This variable helps ensure that the player sprite is only added to the sprite list once.
        # The name may need to change as we develop our concept, since being alive or dead will be an actual mechanic.
        self.alive = False

    # Update function. Another function derived from the pg.sprite.Sprite() superclass.
    # We call the update() function for any sprite that we want to animate, based on the instructions it's given.
    # In this case, we are animating with player control - movement is determined by both the arrow keys and WASD.
    # One final thing to note: notice that this method contains the argument "pressed_keys"; this is how we import player keypresses into the player sprite. More on that later.
    def update(self, pressed_keys):

        # These controls are all 'if' statements, rather than 'if/elif' - this is so that more than one direction can be pressed at once.
        # Note also that these pressed_keys conditionals do not use the typical x==y structure, and instead use x[y].
        # This is because pressed_keys is passed as the full set of key-value pairs, and if any keys are true (i.e. K_UP=1), the associated operation is applied.
        if pressed_keys[K_UP] or pressed_keys[K_w]:
            self.rect.move_ip(0, -5) # 'move_ip' simply means "move in place"

        if pressed_keys[K_DOWN] or pressed_keys[K_s]:
            self.rect.move_ip(0, 5)

        if pressed_keys[K_LEFT] or pressed_keys[K_a]:
            self.rect.move_ip(-5, 0)

        if pressed_keys[K_RIGHT] or pressed_keys[K_d]:
            self.rect.move_ip(5, 0)

        # Movement constraints. If the player sprite exceeds the boundaries of the screen, this conditional prevents them from moving off-screen.
        # This is also written as all 'if' statements, so that one constraint doesn't override another in the case of corners.
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

# Enemy sprite I slapped together. Wanders about randomly with jittery movements.
class Rambler(pg.sprite.Sprite):

    # Note that this __init__() definition includes the arguments 'startX' and 'startY' - this is how we create arguments for our sprite constructors.
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

        # Rambler constraints.
        if self.rect.x <= 0:
            self.rect.x = 0

        if self.rect.x + self.rect.width >= SCREEN_W:
            self.rect.x = SCREEN_W - self.rect.width

        if self.rect.y <= 0:
            self.rect.y = 0

        if self.rect.y + self.rect.height >= SCREEN_H:
            self.rect.y = SCREEN_H - self.rect.height

# Create an instance of the player sprite.
# Note that it has not yet been added to the all_sprites group, and therefore will not be displayed.
player = Player()
enemiesPlaced = False # Simple boolean to ensure that the enemy sprites are only added once.

# Primary loop constant, RUNNING. So long as RUNNING is true, the game loop will keep iterating.
RUNNING = True

# Clock object. Absolutely necessary to control framerate; without it, everything onscreen would process at lightning speed.
# This also helps with counting game ticks, which is essential for in-game timings, as well as other dynamics.
clock = pg.time.Clock()

# Main game loop. Will keep running so long as RUNNING == True.
while RUNNING == True:
    
    SCREEN.fill(GRASS_GREEN) # Fills the main surface with a background color.
    pressed_keys = pg.key.get_pressed() # Gets all keys that are pressed at this iteration of the loop.
                                        # This is what is passed to the player.update() method, or whatever else we want to be based on player control.

    # This is where the player.alive boolean comes into play.
    if player.alive == False:
        all_sprites.add(player) # Note that NOW the player sprite has been added to the 'all_sprites' group, which sets it up to be displayed.
        player.alive = True

    if enemiesPlaced == False:
        rambler1 = Rambler(SCREEN_W * 0.25, SCREEN_H * 0.25) # Note that unlike the 'Player()' class, these constructors have agruments that are processed by _init_().
        rambler2 = Rambler(SCREEN_W * 0.25, SCREEN_H * 0.75)
        rambler3 = Rambler(SCREEN_W * 0.75, SCREEN_H * 0.25)
        rambler4 = Rambler(SCREEN_W * 0.75, SCREEN_H * 0.75)
        enemy_sprites.add(rambler1, rambler2, rambler3, rambler4)
        all_sprites.add(rambler1, rambler2, rambler3, rambler4)
        enemiesPlaced = True

    # Event loop. This loop iterates through all of the events that are currently on the event stack.
    # These events can include keypresses, as well as system events such as quitting the program.
    for event in pg.event.get():

        # Keydown commands. When the player presses the key down, no mater how long they press it, it will only process once.
        # This can be essential for things such as pause menus or menu cursors; otherwise, the keys will be registered numerous times in a fraction of a second. 
        if event.type == KEYDOWN:

            # IMPORTANT: the event.key is compared differently from 'pressed_keys'; here, you will be using the standard x==y structure.
            # This is because you are accessing the specific key of the KEYDOWN event, not obtaining a full list of key-value pairs.
            if event.key == K_ESCAPE:
                RUNNING = False # Our first instance of the RUNNING constant being used to end the game. This allows us to exit by pressing ESC.

        # This event ensures that when the system passes this program a quit command, it is executed and the game loop is stopped.
        # Without this, the program will not exit unless we manually kill the program via Task Manager or what have you.
        if event.type == pg.QUIT:
            RUNNING = False

    # Player/enemy sprite collision. If the player bumps into any of the ramblers, the player sprite is removed, and the game is essentially over.
    # I highly recommend taking a close look at sprite.spritecollide() and related functions, if for no other reason than to understand the arguments.
    # Additionally, sprite.collide_mask is very useful, because it uses the outer boundaries of the sprite as its hitbox, instead of using a rect value, as is done traditionally.
    if pg.sprite.spritecollide(player, enemy_sprites, False, pg.sprite.collide_mask):
        player.kill() # This call removes the player sprite from all sprite groups.

    # Update loop. Calls the update() functions for every indicated sprite group.
    # Which sprites are updated will later depend on the game state (i.e. active vs. paused).
    # As we can see, the player update method is being handed player keys, while the enemy update method is not, because of how we set up our update() arguments.
    player.update(pressed_keys)
    enemy_sprites.update()

    # Blit-to-screen loop. This is why every sprite requires an image and a rect.
    # The 'all_sprites' group is iterated over, each image being blit to the screen.
    for item in all_sprites:
        SCREEN.blit(item.image, item.rect)

    # In order to see anything, the display must be flipped.
    # This is to say that a new frame of video needs to be created using any modifications we made when updating the sprites.
    pg.display.flip()

    # This is where the Clock() object comes into play.
    # The method below constrains video processing to 60 FPS, which is the standard FPS of the NTSC (North America/Japan) video format.
    clock.tick(60)

# Quit call. Once everything is finished (i.e. the game loop has ended), the game shuts the window down and closes the program.
pg.quit()
