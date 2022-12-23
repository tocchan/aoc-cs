import io
import re
import sys

INPUT_FILE = 'day04.input.txt';

testInput = ""
with open (INPUT_FILE, 'r') as inputFile:
    testInput = inputFile.read()


# Take a file and parse it into individual entries
def ParseEntries( inputText ):
    lines = inputText.split('\n')

    entries = []
    entry = []
    for line in lines: 
        line = line.strip()
        if len(line) == 0:
            entries.append(entry)
            entry = []
        else: 
            items = line.split()
            entry.extend(items)

    if len(entry) != 0:
        entries.append(entry)

    return entries


entries = ParseEntries( testInput )
print ('Num Inputs: ' + str(len(entries)))

def ValidNumber(value, min, max):
    if not value.isnumeric():
        return False

    val = float(value)
    return (val >= min) and (val <= max)

#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#
def IsValidA( entry ):
    requiredTags = ['byr', 'iyr', 'eyr', 'hgt', 'hcl', 'ecl', 'pid']
    optionalTags = ['cid']

    for item in entry: 
        key, value = item.split(':')

        if (key in requiredTags):
            requiredTags.remove(key)
        
    if len(requiredTags) == 0: 
        return True
    else: 
        print ("Failed, missing required fields")

#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#-#
def IsValid( entry ):
    requiredTags = ['byr', 'iyr', 'eyr', 'hgt', 'hcl', 'ecl', 'pid']
    optionalTags = ['cid']


    for item in entry: 
        key, value = item.split(':')

        if key == 'byr': 
            if not ValidNumber(value, 1920, 2002):
                print (key + " failed: " + value )
                return False

        elif key == 'iyr':
            if not ValidNumber(value, 2010, 2020):
                print (key + " failed: " + value )
                return False

        elif key == 'eyr':
            if not ValidNumber(value, 2020, 2030):
                print (key + " failed: " + value )
                return False

        elif key == 'hgt':
            isCM = value.find('cm')
            isIN = value.find('in')
            if (isCM >= 0):
                if (isCM != len(value) - 2): 
                    print (key + " failed: " + value )
                    return False
                heightValue = value[0:isCM]
                if not ValidNumber(heightValue, 150, 193):
                    print (key + " failed: " + value )
                    return False
            elif (isIN >= 0):
                if (isIN != len(value) - 2): 
                    print (key + " failed: " + value )
                    return False
                heightValue = value[0:isIN]
                if not ValidNumber(heightValue, 59, 76):
                    print (key + " failed: " + value )
                    return False
            else:
                print (key + " failed: " + value )
                return False

        elif key == 'hcl':
            colorMatch = r'^#(?:[0-9a-f]{6})$'
            match = re.match( colorMatch, value )
            if not match: 
                print (key + " failed: " + value )
                return False

        elif key == 'ecl':
            validColors = ['amb', 'blu', 'brn', 'gry', 'grn', 'hzl', 'oth']
            if value not in validColors:
                print (key + " failed: " + value )
                return False 

        elif key == 'pid':
            if (len(value) != 9) or not value.isnumeric(): 
                print (key + " failed: " + value )
                return False 

        if (key in requiredTags):
            requiredTags.remove(key)
        
    if len(requiredTags) == 0: 
        return True
    else: 
        print ("Failed, missing required fields")

    
numValid = 0
for entry in entries:
    if IsValidA(entry): 
        numValid += 1
    else: 
        print( "Entry Failed: " + str(entry))

print( 'Valid Count: ' + str(numValid) )
