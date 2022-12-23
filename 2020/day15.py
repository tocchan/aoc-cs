import io
import re
import sys
import time
from copy import deepcopy 

import aoc

# inputStr = '0,3,6'
inputStr = '14,1,17,0,3,20'
input = [int(v) for v in inputStr.split(',')]
print( str(input) )

start = time.time()

# total = 30000000
total = 2020
seen = [-1] * total
for i in range(len(input) - 1): 
    seen[input[i]] = i 

lastInput = input[-1]
for i in range(len(input), total): 
    newVal = 0
    if seen[lastInput] >= 0:
        lastIdx = seen[lastInput]
        newVal = i - lastIdx - 1
    else: 
        newVal = 0

    seen[lastInput] = i - 1
    lastInput = newVal 

    # if i % 100000 == 0: 
    #    print( 'step ' + str(i) )

print( lastInput )
print( 'Part02 took {:.4f}s'.format( (time.time() - start) ) )