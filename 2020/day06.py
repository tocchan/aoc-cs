import io
import re
import sys

INPUT_FILE = 'day06.input.txt';

testInput = ""
with open (INPUT_FILE, 'r') as inputFile:
    testInput = inputFile.read()

class Entry:
    def __init__(self): 
        self.answers = []
        self.allAnswers = []
        self.uniqueAnswers = []
    #end __init__

    def AppendLine(self, line):
        self.answers.append(line)

        for c in line: 
            if (c not in self.allAnswers): 
                self.allAnswers.append(c)

        if (len(self.answers) == 1):
            for c in line: 
                if c not in self.uniqueAnswers: 
                    self.uniqueAnswers.append(c)
        else: 
            # keep all unique answers that also exist in our new line
            newAnswers = []
            for c in self.uniqueAnswers:
                if c in line and c not in newAnswers: 
                    newAnswers.append(c)

            self.uniqueAnswers = newAnswers
    #end AppendLine

    def IsValid(self):
        return len(self.answers) > 0
    #end IsValid

    def GetAnswerCount(self): 
        return len(self.allAnswers)

    def GetUniqueAnswerCount(self):
        return len(self.uniqueAnswers)
    #end GetUniqueAnswerCount


# Take a file and parse it into individual entries
def ParseEntries( inputText ):
    lines = inputText.split('\n')

    entry = Entry()
    entries = []

    entries = []
    for line in lines:
        if len(line) > 0: 
            entry.AppendLine(line)
        else:
            entries.append(entry)
            entry = Entry()

    if (entry.IsValid()):
        entries.append(entry)

    return entries
#end ParseEntries
    
# Parse to our entries
entries = ParseEntries( testInput )
print ('Num Inputs: ' + str(len(entries)))

sum = 0
for entry in entries: 
    cnt = entry.GetUniqueAnswerCount()
    print( 'entry count: ' + str(cnt))
    sum += cnt

print( "Unique Answer Count: " + str(sum) )

