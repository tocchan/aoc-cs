import io
import re
import sys
import time

import aoc

# config
INPUT_FILE = 'day10.input.txt';
REMOVE_LINE_BREAKS = True

# read input
input = aoc.ImportInput(INPUT_FILE, REMOVE_LINE_BREAKS)

# implementation
values = [int(line) for line in input]
sortedValues = sorted( values )

maxValue = max( values )
rating = maxValue + 3

# """
print( 'max value: {}'.format( maxValue ) )
print( 'max joltage: {}'.format( rating ) )

differences = [0, 0, 0, 0]
differences[sortedValues[0]] += 1
differences[3] += 1

for i in range(len(sortedValues) - 1): 
    diff = sortedValues[i + 1] - sortedValues[i]
    differences[diff] += 1

for i, v in enumerate(differences):
    print( '{} has {} differences'.format(i, v) )

print( 'answer is {} * {} = {}'.format( differences[1], differences[3], differences[1] * differences[3] ) )
# """

def part02(): 
    workingSet = [0] + sortedValues + [rating]

    setCount = len(workingSet)
    counts = [0] * setCount

    # now work backwards
    for i in range(setCount): 
        idx = setCount - i - 1
        if (i == 0): 
            counts[idx] = 1
        else: 
            # from idx, count all sets I can reach
            val = workingSet[idx]
            count = 0
            for j in range(idx + 1, setCount): 
                nextVal = workingSet[j]
                if (nextVal - val) <= 3: 
                    count += counts[j]
            
            counts[idx] = count 

    print( 'total = ' + str(counts[0]) )

    # for i, count in enumerate(counts): 
    #    print( 'values[{}] = {} and has {} possible way(s) to end'.format( i, workingSet[i], counts[i] ) )
#end part02

start = time.time()
part02()
dur = time.time() - start
print( 'part02 took {:.4f}ms'.format( dur * 1000.0 ) )