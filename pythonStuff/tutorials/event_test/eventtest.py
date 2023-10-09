import events

#https://python-utilities.readthedocs.io/en/latest/events.html
#https://pypi.org/project/Events/

class EventHolder:
    event = events.Events()

class Sub:
    def __init__(self, name):
        self.name = name

    def fire(self, other):
        print(other.name + " function called")
        print(self.name + " is firing")

class Caller:
    def __init__(self, name):
        self.name = name

    def someFunc(self):       
        EventHolder.event.on_change(self)


sub = Sub("Subscriber")
caller = Caller("Caller")

EventHolder.event.on_change += sub.fire

caller.someFunc()