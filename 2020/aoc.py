import io
import re
import sys

# Used to make parsing inputs faster
class Input:
    line = ""
    lines = []

    def __init__(self, line = ''):
        self.lines = []     
        self.line = line   
        if line:
            self.lines.append(line)

    def Join(self):
        self.line = '\n'.join(self.lines)

    def RemoveLineBreaks(self):
        self.line = ' '.join(self.line.splitlines())

#end class Input

# Breaks the file text input distint pieces of input
def ParseInputs(inputStr): 
    inputs = []
    lines = inputStr.splitlines(False)
    if "" in lines: 
        # inputs are split by empty line
        input = Input()
        for line in lines: 
            if line:
                input.lines.append(line)
            else:
                input.Join()
                if input.line: 
                    inputs.append(input)
                    input = Input()

        input.Join()
        if input.line:
            inputs.append( input )

    else: 
        # inputs are each line
        inputs.extend( [Input(line) for line in lines] )

    return inputs
#end ParseInputs


def ImportInput( filename, removeLineBreaks = False ): 
    # open the file
    textInput = ""
    with open (filename, 'r') as inputFile:
        textInput = inputFile.read()

    # as an input structure
    inputs = ParseInputs(textInput)
    print( 'Num Inputs: ' + str(len(inputs)) )
    print()

    # if Inputs contain multiple lines, join them to a single line
    if removeLineBreaks:
        for input in inputs: input.RemoveLineBreaks()  

    # if I want just the strings
    inputLines  = [input.line for input in inputs]

    return inputLines
#end ImportInput
