import io
import re
import sys
import time

import aoc
from itertools import combinations

# config
INPUT_FILE = 'day09.input.txt';
REMOVE_LINE_BREAKS = True

# read input
input = aoc.ImportInput(INPUT_FILE, REMOVE_LINE_BREAKS)

# implementation
numbers = []
idx = 0
for line in input: 
    numbers.append( int(line) )

PL = 25


def IsSum( n, numbers, startIdx, endIdx ):
    for i in range(startIdx, endIdx): 
        for j in range( i + 1, endIdx ): 
            if (n == numbers[i] + numbers[j]): 
                return True

    return False
#end

start = time.time()

PREAMBLE_LENGTH = PL
input_list = numbers

def part01():
    input_list_set = set(input_list[PREAMBLE_LENGTH:])
    passed_digits_set = set()
    for ind, digit in enumerate(input_list):
        if ind < PREAMBLE_LENGTH:
            continue
        prev_combos = combinations(input_list[ind-PREAMBLE_LENGTH:ind], 2)
        for first, second in prev_combos:
            if digit == first + second:
                passed_digits_set.add(digit)

    return input_list_set.difference(passed_digits_set)

start = time.time()
part01()
print( 'Taylor Time {:.2f}ms'.format( 1000.0 * (time.time() - start) ) )

# find sum
start = time.time()
sum = 0
sumIdx = -1
for i in range(PL, len(numbers)): 
    if not IsSum( numbers[i], numbers, i - PL, i ):
        sum = numbers[i]
        sumIdx = i
        break

print( 'No sum at idx: ' + str(sumIdx) + ', value = ' + str(sum) )
print( 'Find Sum took {:.2f}ms'.format( 1000.0 * (time.time() - start) ) )

def DoesSum( val, numbers, startIdx ): 
    sum = 0
    for i in range(startIdx, len(numbers)):
        sum += numbers[i]
        if (sum == val): 
            return i != startIdx, startIdx, i
        elif sum > val: 
            return False, 0, 0

    return False 
#end DoesSum

start = time.time()

# find contiguous set of numbers that sums to it
for i in range(len(numbers)): 
    success, minIdx, maxIdx = DoesSum( sum, numbers, i )
    if success:
        print( str(minIdx) + ', ' + str(maxIdx) )
        minv = min( numbers[minIdx : maxIdx + 1] )
        maxv = max( numbers[minIdx : maxIdx + 1] )
        print( str(minv) + ' + ' + str(maxv) + ' = ' + str(minv + maxv) )
#end for

print( 'Range took {:.2f}ms'.format( 1000.0 * (time.time() - start) ) )



 