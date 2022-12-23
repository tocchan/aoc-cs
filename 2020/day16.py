import io
import re
import sys
import time
from copy import deepcopy 

import aoc
from utils import time_func


# config
INPUT_FILE = 'day16.input.txt'


# read input
textInput = ''
with open(INPUT_FILE, 'r') as inputFile:
    textInput = inputFile.read()

lines = textInput.splitlines()
lineCount = len(lines)

categories = []

i = 0
while i < lineCount: 
    line = lines[i].strip()
    i += 1 

    if not line: 
        break

    cat, values = line.split(':')
    r0, r1 = values.split('or') 

    min0, max0 = r0.split('-')
    min1, max1 = r1.split('-')

    categories.append( [cat, int(min0), int(max0), int(min1), int(max1)] )


# skip 'your ticket'
i += 1
myValues = [int(v) for v in lines[i].strip().split(',')]

# move past my values, blank line, and 'nearby tickets'
i += 3
nearbyValues = []
while i < lineCount: 
    line = lines[i]
    nearbyValues.append( [int(v) for v in line.strip().split(',')] )
    i += 1

# implementation #######################################
# massage input

def cat_contains( cat, val ): 
    return (val >= cat[1] and val <= cat[2]) or (val >= cat[3] and val <= cat[4]) 

def in_any( val, cats ): 
    for cat in cats: 
        if cat_contains( cat, val ):
            return True 
    return False 
    

def add_invalids( values, cats ): 
    sum = 0
    for v in values:
        if not in_any( v, cats ): 
            sum += v

    return sum 


@ time_func
def part01( cats, nearby ):
    sum = 0
    for values in nearby: 
        sum += add_invalids( values, cats )

    print( 'part01 -> ' + str(sum) )
# end part01


def is_valid( values, cats ):
    for value in values: 
        if not in_any( value, cats ): 
            return False 
    return True 


def get_valid( nearby, cats ):
    ret = []
    for values in nearby: 
        if is_valid( values, cats ): 
            ret.append( values )
    return ret


def row_works( cat, row, tickets ): 
    for ticket in tickets: 
        if not cat_contains( cat, ticket[row] ): 
            return False
    return True 


def done(rows): 
    for row in rows: 
        if len(row) > 1: 
            return False 
    return True


def remove_rest( rows, val ):
    for row in rows: 
        if len(row) > 1: 
            try: 
                row.remove(val)
            except ValueError: 
                pass



# will reduce rows with multiple answers
# may need to be recursive to find something that works
# but may not need to do anything
def pick_rows( rows ): 
    while not done(rows): 
        for row in rows: 
            if len(row) == 1: 
                remove_rest( rows, row[0] )


@time_func
def part02( cats, nearby ):
    valid = get_valid( nearby, cats )
    print( '{} -> {} sets'.format( len(nearby), len(valid) ) )

    # for each category, find the index that all values fall in that range
    # for each category, will contain a list of rows that would work for it
    validRows = []
    catCount = len(cats)
    for cat in cats: 
        rows = []
        for row in range(catCount): 
            if row_works( cat, row, valid ): 
                rows.append(row)
        validRows.append(rows) 

    pick_rows( validRows ) 

    prod = 1
    for idx, cat in enumerate(cats): 
        row = validRows[idx][0]
        # print( '{} is row {}'.format( cat[0], row ) )
        if 'departure' in cat[0]: 
            prod *= myValues[row]

    print( 'part02 result -> ' + str(prod) )
    # for those rows, anything with one option takes it, and removes
    # that row from other sets.  Repeat until all sets are figured out

# end part02


part01( categories, nearbyValues )
part02( categories, nearbyValues )

