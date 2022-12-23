import io
import re
import sys

INPUT_FILE = 'day02.input.txt';

testInput = ""
with open (INPUT_FILE, 'r') as inputFile:
    testInput = inputFile.read()

textInputs = testInput.split('\n')
groups = []

## parsing inputs
for textInput in textInputs:
    match = re.match( r'(\d+)-(\d+) (\w+): (\w+)$', textInput)
    groups.append( match.groups() )

def CountChar(s, sc): 
    count = 0
    for c in s: 
        if c == sc: 
            count += 1
    
    return count

def IsValid(group):
    min = int(group[0]) - 1
    max = int(group[1]) - 1
    searchChar = str(group[2])
    pattern = str(group[3])

    if (min >= len(pattern)) and (max >= len(pattern)):
        return False
        
    aMatch = (pattern[min] == searchChar)
    bMatch = (pattern[max] == searchChar)
    return (aMatch != bMatch)

# Check it!
validGroupCount = 0
for group in groups: 
    if IsValid( group ):
        validGroupCount += 1

print ('Valid Group Count: ' + str(validGroupCount))

