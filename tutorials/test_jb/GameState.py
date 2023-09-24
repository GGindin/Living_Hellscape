import pygame

from Room import *

class GameState:
    room : Room = None
    rooms : list[Room] = []

    def isObjectMovingToOtherRoom(worldSpaceRect : pygame.Rect):
        if GameState.room is not None:
            roomsCopy = GameState.rooms.copy()
            roomsCopy.remove(GameState.room)
            rectList = []
            for i in roomsCopy:
                rectList.append(i.getWorldRect())
            index = worldSpaceRect.collidelist(rectList)
            if index < 0:
                return False
            else:
                currentRoomRect = GameState.room.getWorldRect()
                nextRoomRect = roomsCopy[index].getWorldRect()

                if nextRoomRect.contains(worldSpaceRect):
                    GameState.room = roomsCopy[index]
                    return True

                if not currentRoomRect.collidepoint(worldSpaceRect.topleft) and not nextRoomRect.collidepoint(worldSpaceRect.topleft):
                    return False
                elif not currentRoomRect.collidepoint(worldSpaceRect.topright) and not nextRoomRect.collidepoint(worldSpaceRect.topright):
                    return False
                elif not currentRoomRect.collidepoint(worldSpaceRect.bottomright) and not nextRoomRect.collidepoint(worldSpaceRect.bottomright):
                    return False
                elif not currentRoomRect.collidepoint(worldSpaceRect.bottomleft) and not nextRoomRect.collidepoint(worldSpaceRect.bottomleft):
                    return False
                else:
                    return True
        else:
            return False

        