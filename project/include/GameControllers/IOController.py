import pygame
import os

#https://stackoverflow.com/questions/25389095/python-get-path-of-root-project-structure
class IOController:

    ROOT_DIR = None
    instance = None

    def __init__(self, PROJ_ROOT):
        IOController.ROOT_DIR = PROJ_ROOT
        IOController.instance = self

    def getImage(dir, fileName):
        imgName = os.path.splitext(fileName)[0]
        relPath = os.path.join(dir, fileName)
        absPath = os.path.join(IOController.ROOT_DIR, relPath)
        return pygame.image.load(absPath, imgName)

