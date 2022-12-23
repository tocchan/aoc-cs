import io
import re
import sys

INPUT_FILE = 'day03.input.txt';

testInput = ""
with open (INPUT_FILE, 'r') as inputFile:
    testInput = inputFile.read()

treeMap = testInput.split('\n')


def CountTrees( map, slopeX, slopeY ):
    width = len(map[0]); 
    height = len(map)
    x = 0
    y = 0

    treeCount = 0
    while y < height:
        isTree = (map[y][x] == '#')
        if isTree: 
            treeCount += 1

        y += slopeY
        x = (x + slopeX) % width

    return treeCount


count0 = CountTrees( treeMap, 1, 1 )
count1 = CountTrees( treeMap, 3, 1 )
count2 = CountTrees( treeMap, 5, 1 )
count3 = CountTrees( treeMap, 7, 1 )
count4 = CountTrees( treeMap, 1, 2 )
count = count0 * count1 * count2 * count3 * count4;
print( str(count0) + ', ' + str(count1) + ', ' + str(count2) + ', ' + str(count3) + ', ' + str(count4) )

print ("NumTrees: " + str(count))

