import pygame

#from include.GameObject.GameObject import *

class ObjectManager:
    
    instance = None

    def __init__(self):
        self.playerGroup = pygame.sprite.Group()
        self.enemyGroup = pygame.sprite.Group()
        self.objectGroup = pygame.sprite.Group()

        ObjectManager.instance = self

    def addGameObjectToGroup(self, gameObject : pygame.sprite.Sprite, group : pygame.sprite.Group) -> None:
        group.add(gameObject)

    def removeGameObjectFromGroup(self, gameObject : pygame.sprite.Sprite, group : pygame.sprite.Group) -> None:
        group.remove(gameObject)

    def updateGroup(self, group : pygame.sprite.Group):
        group.update()

    def drawGroup(self, group : pygame.sprite.Group, target : pygame.Surface):
        group.draw(target)

    def collideGroups(self):
        dict = pygame.sprite.groupcollide(self.playerGroup, self.objectGroup, False, False)
        for key in dict:
            for value in dict[key]:
                if key.onCollision(value):
                    self.playerGroup.remove(key)
                    break
