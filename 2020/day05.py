import io
import re
import sys

INPUT_FILE = 'day05.input.txt';

testInput = ""
with open (INPUT_FILE, 'r') as inputFile:
    testInput = inputFile.read()

def FindSeatStart(line):
    idx = 0
    while idx < len(line) and (line[idx] == 'F' or line[idx] == 'B'):
        idx += 1
    return idx
# end FindSeatStart

def ParseBinary(line, numBits, lowBit, highBit):
    value = 0
    for c in line: 
        numBits -= 1
        if (c == highBit): 
            value += (1 << numBits)

    return value
# end ParseBinary


class Input:
    rawText = ""
    row = 0
    column = 0
    seatID = 0
    numRows = 0
    numSeats = 0

    def __init__(self, line):
        self.rawText = line
        self.Parse(line)
    #end Input.__init__

    def Parse(self, line):
        seatStart = FindSeatStart(line)
        rowLine = line[0:seatStart]
        seatLine = line[seatStart:len(line)]

        numRowBits = seatStart; 
        numSeatBits = len(line) - seatStart; 

        self.numRows = 1 << numRowBits; 
        self.numSeats = 1 << numSeatBits

        self.row = ParseBinary(rowLine, numRowBits, 'F', 'B')
        self.column = ParseBinary(seatLine, numSeatBits, 'L', 'R')
        self.seatID = self.row * self.numSeats + self.column
        
    # end Input.Parse

# end class Input

# Take a file and parse it into individual entries
def ParseEntries( inputText ):
    lines = inputText.split('\n')

    entries = []
    for line in lines:
        entry = Input(line)
        entries.append(entry)

    return entries
#end ParseEntries
    
# Parse to our entries
entries = ParseEntries( testInput )
print ('Num Inputs: ' + str(len(entries)))



highestSeatID = 0
for entry in entries:
    highestSeatID = max(highestSeatID, entry.seatID)
    
print ('Best Seat ID: ' + str(highestSeatID))

seatsFull = [False] * (highestSeatID + 1)
for entry in entries:
    seatsFull[entry.seatID] = True

seatIdx = 0
previousSeatFull = False
for idx in range(highestSeatID): 
    seatFull = seatsFull[idx]
    if (not seatFull) and previousSeatFull: 
        print( "My Seat: " + str(idx) )
        break
    previousSeatFull = seatFull

