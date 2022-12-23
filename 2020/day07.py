import io
import re
import sys

INPUT_FILE = 'day07.sample.txt';

testInput = ""
with open (INPUT_FILE, 'r') as inputFile:
    testInput = inputFile.read()

class Bag:
    def __init__(self): 
        self.name = ""
        self.subBags = []
        self.subBagCounts = []
    #end __init__

    def Parse(self, line):
        name, contents = line.split('contain')
        self.name = name.split('bag')[0].strip()

        contents = contents.strip()
        if contents != 'no other bags.':
            bagTypes = contents.split(',')
            for bagType in bagTypes: 
                bagType = bagType.split('bag')[0].strip()
                count, bagType = bagType.split(' ', 1)

                self.subBags.append( bagType.strip() )
                self.subBagCounts.append( int(count.strip()) )
    #end Parse

    def GetChildTypeCount(self):
        return len(self.subBags)

    def GetChildCount(self):
        return sum(self.subBagCounts)

    def CanContain(self, bagName):
        return bagName in self.subBags

    def CanContainRecursive(self, bagName, bagDict):
        if self.CanContain(bagName): 
            return True

        for subBagName in self.subBags: 
            bag = bagDict[subBagName]
            if bag.CanContainRecursive(bagName, bagDict): 
                return True; 

        return False; 
    #end CanContainRecursive

    def GetChildCountRecursive(self, bagDict):
        count = 0
        for idx in range(len(self.subBags)):
            bagName = self.subBags[idx]
            subCount = self.subBagCounts[idx]
            bag = bagDict[bagName]
            count += (bag.GetChildCountRecursive(bagDict) + 1) * subCount 

        return count
    #end GetChildCountRecursive

    def PrintChildren(self): 
        for i in range(len(self.subBags)): 
            print( '- "' + self.subBags[i] + '", Count: ' + str(self.subBagCounts[i]))
#end Bag


# Take a file and parse it into individual entries
def ParseEntries( inputText ):
    lines = inputText.split('\n')

    bags = []
    for line in lines:
        bag = Bag()
        bag.Parse(line)
        bags.append(bag)

    return bags;
#end ParseEntries
    
# Parse to our entries
bags = ParseEntries( testInput )
print ('Num Inputs: ' + str(len(bags)))

bagDict = {}
for bag in bags: 
    bagDict[bag.name] = bag
    # print( 'bag "' + bag.name + '" contains ' + str(bag.GetChildTypeCount()) 
    #    + ' sub types.  Total children: ' + str(bag.GetChildCount()) )
    # bag.PrintChildren()

myBagName = "shiny gold"
optionCount = 0
for bag in bags: 
    if bag.CanContainRecursive(myBagName, bagDict): 
        optionCount += 1

myBag = bagDict[myBagName]; 
subBagCount = myBag.GetChildCountRecursive(bagDict)

print("My options: " + str(optionCount))
print("Total sub bags needed: " + str(subBagCount))