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

while(PygameController.instance.running):
    PygameController.update()
