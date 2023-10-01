import pygame

from include.GameControllers.inputController import *
from include.GameControllers.IOController import *
from include.GameControllers.ObjectManager import *

class PygameController:

    #Constant Static Vars
    GAME_WIDTH = 800
    ASPECT_RATIO = (4.0/3)
    FRAME_RATE = 60
    PIXELS_PER_UNIT = 32
    SCREEN_CLEAR_COLOR = "black"

    #Static Vars
    instance = None
    Delta_Time = 0.0

    def __init__(self):
        pygame.init()

        currentDisplayInfo = pygame.display.Info()
        self.displayRes = pygame.Vector2(currentDisplayInfo.current_w, currentDisplayInfo.current_h)
        self.screenTarget = pygame.display.set_mode((0, 0), pygame.FULLSCREEN)

        cameraTargetY = PygameController.GAME_WIDTH * (1 / PygameController.ASPECT_RATIO)
        self.cameraTarget = pygame.Surface((PygameController.GAME_WIDTH, int(cameraTargetY)))

        self.cameraToScreenRatio = self.screenTarget.get_height() / float(self.cameraTarget.get_height())

        self.running = True
        self.clock = pygame.Clock()

        PygameController.instance = self

    def update():
        if(PygameController.instance is None):
            return
        
        PygameController.instance.preFrame()

        if(PygameController.instance.running):
            PygameController.instance.frame()
            PygameController.instance.postFrame()

    def preFrame(self):        
        self.endGame()

        if(self.running):
            self.calcDeltaTime()

    def frame(self):
        InputController.pollInput()

        ObjectManager.instance.updateGroup(ObjectManager.instance.objectGroup)
        ObjectManager.instance.updateGroup(ObjectManager.instance.playerGroup)
        ObjectManager.instance.updateGroup(ObjectManager.instance.enemyGroup)

        ObjectManager.instance.collideGroups()

        ObjectManager.instance.drawGroup(ObjectManager.instance.objectGroup, self.cameraTarget)
        ObjectManager.instance.drawGroup(ObjectManager.instance.playerGroup, self.cameraTarget)
        ObjectManager.instance.drawGroup(ObjectManager.instance.enemyGroup, self.cameraTarget)

    def postFrame(self):
        self.BlitToScreen()
        self.swap()

    def endGame(self):
        #test for quit
        input = InputController.getInput()

        if(input.escape):
            self.running = False

        for event in pygame.event.get():
            if event.type == pygame.QUIT:
                self.running = False

        if(not self.running):
            pygame.quit()

    def calcDeltaTime(self):
        PygameController.Delta_Time = self.clock.tick(PygameController.FRAME_RATE) / 1000.0

    def swap(self):
        pygame.display.flip()

    def BlitToScreen(self):
        #clear screen target
        self.screenTarget.fill(PygameController.SCREEN_CLEAR_COLOR)

        #get temp surface at correct scale for screen and clear cameraTarget
        tempTarget = pygame.transform.scale_by(self.cameraTarget, self.cameraToScreenRatio)
        xOffset = (self.screenTarget.get_width() - tempTarget.get_width()) / 2
        self.cameraTarget.fill(PygameController.SCREEN_CLEAR_COLOR)
        
        #draw to screen target
        self.screenTarget.blit(tempTarget, (xOffset, 0))