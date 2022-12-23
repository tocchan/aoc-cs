import copy
import io
import re
import sys
import time

import aoc

# config
INPUT_FILE = 'day11.forseth.txt';
REMOVE_LINE_BREAKS = True

# read input
input = aoc.ImportInput(INPUT_FILE, REMOVE_LINE_BREAKS)

# implementation
newInput = []
for line in input: 
    newInput.append( list(line) )
input = newInput 

# '.' = floor
# 'L' = empty seat
# '#' = occupied seat

def GetNeighbor( map, x, y, dx, dy ):
    width = len(map[0])
    height = len(map)

    nx = x + dx
    ny = y + dy

    if (nx < 0) or (nx >= width):
        return '.'

    if (ny < 0) or (ny >= height): 
        return '.'

    return map[ny][nx]
#end GetNeighbor

def CastNeighbor( map, x, y, dx, dy ):
    width = len(map[0])
    height = len(map)
    
    nx = x + dx
    ny = y + dy

    while True: 
        if (nx < 0) or (nx >= width):
            return '.'

        if (ny < 0) or (ny >= height): 
            return '.'

        seat = map[ny][nx]
        if (seat != '.'): 
            return seat

        nx = nx + dx 
        ny = ny + dy

    return map[ny][nx]
#end CastNeighbor

def StepCell( map, x, y ):
    seat = map[y][x]
    if seat == '.': 
        return '.'

    ## count neighbors
    count = 0; 
    for dx in range(-1,2):
        for dy in range(-1,2):
            if (dx == 0) and (dy == 0): 
                continue 
            # n = GetNeighbor( map, x, y, dx, dy )
            n = CastNeighbor( map, x, y, dx, dy )
            if (n == '#'): 
                count += 1

    if seat == 'L' and (count == 0): 
        return '#'
    if seat == '#' and (count >= 5): 
        return 'L'

    return seat
#end Step

def Copy( map ): 
    return copy.deepcopy(map)
#end Copy

def RunPass( map ):
    copy = Copy( map )
    width = len(map[0])
    height = len(map)

    for y in range(height):
        for x in range(width):
            copy[y][x] = StepCell( map, x, y )

    return copy;
#end RunStep

def PrintMap( step, map ):
    print (' ')
    print( 'Step ' + str(step) )
    for line in map: 
        print (''.join(line))


def Part01( map ):
    steps = 0
    # PrintMap( steps, map )
    copy = RunPass( map )
    while copy != map:
        steps += 1
        map = copy
        
        # PrintMap( steps, map )
        copy = RunPass( map )
        
    count = 0
    for line in copy: 
        for c in line: 
            if (c == '#'):
                count += 1

    print( 'Total occupied: {}'.format(count) )
#end Part01

start = time.time()
Part01( input )
dur = time.time() - start

print( 'Part02 took {:.4f}ms'.format(dur * 1000.0) )
