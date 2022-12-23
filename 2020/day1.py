import sys
import io

INPUT_FILE = 'day01.input0.txt';

testInput = ""
with open (INPUT_FILE, 'r') as inputFile:
    testInput = inputFile.read()

textInputs = testInput.split()
numInputs = []

for textInput in textInputs:
    if textInput.isnumeric(): 
        numInputs.append( int(textInput) )

def FindSumA( inputs, searchSum ):
    for i in range(0, len(numInputs)): 
        for j in range(i + 1, len(numInputs)): 
            sum = numInputs[i] + numInputs[j]
            if sum == searchSum: 
                product = numInputs[i] * numInputs[j]
                print ('Product: ' + str(product))
                return

def FindSumB( inputs, searchSum ):
    for i in range(0, len(numInputs)): 
        for j in range(i + 1, len(numInputs)): 
            for k in range(j + 1, len(numInputs)):
                sum = numInputs[i] + numInputs[j] + numInputs[k]
                if sum == searchSum: 
                    product = numInputs[i] * numInputs[j] * numInputs[k]
                    print ('Product: ' + str(product))
                    return
            
FindSumA( numInputs, 2020 )
FindSumB( numInputs, 2020 )




