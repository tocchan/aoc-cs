import io
import re
import sys

INPUT_FILE = 'day08.input.txt';

class Input:
    def __init__():
        self.line = ""

    def __init__(line):
        self.line = line.strip()

    def Join(self):
        lines = self.line.splitlines(False)
        self.line = ' '.join(lines)
        self.line = str.join()

#end class Input

def ParseInputs(inputStr): 
    inputs = []
    lines = inputStr.splitlines()
    if "" in lines: 
        # inputs are split by empty line
        input = Input()
        for line in lines: 
            if line:
                input.line.append(line)
            else:
                if input.line: 
                    inputs.append( input )
                input = Input()

        if input.line:
            inputs.append(input)

    else: 
        # inputs are each line
        inputs.extend( [Input(line) for line in lines] )

    return inputs
#end ParseInputs


textInput = ""
with open (INPUT_FILE, 'r') as inputFile:
    testInput = inputFile.read()

inputs = ParseInputs(textInput)
for input in inputs: input.Join()  

ops = []
values = []

lines = testInput.splitlines(False)
for line in lines:
    op, val = line.split(' ', 1)
    ops.append(op)
    values.append(int(val))
    # print( op + ', ' + str(int(val)) )
    
def RunProgram(ops, values):
    acc = 0
    pid = 0
    visited = [False] * len(ops)

    while (pid < len(ops)): 
        if (visited[pid]): 
            return False, acc

        visited[pid] = True
        op = ops[pid]
        val = values[pid]

        if (op == 'acc'): 
            acc += val
            pid += 1
        elif op == 'nop':
            # nothing
            pid += 1
        elif op == 'jmp':
            pid += val

    return True, acc

print( 'OpLen: ' + str(len(ops)) )
success, acc = RunProgram(ops, values)
print ( 'First Run: acc=' + str(acc) )

for i in range(len(ops)): 
    op = ops[i]
    if (op == 'nop') or (op == 'jmp'): 
        oldOp = ops[i]
        ops[i] = 'nop' if (oldOp == 'jmp') else 'jmp'
        success, acc = RunProgram(ops, values)
        ops[i] = oldOp

        if success:
            print( 'line ' + str(i) + ' changed;  acc = ' + str(acc) )
            break


