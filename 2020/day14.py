import io
import re
import sys
import time
from copy import deepcopy 

import aoc


# config
INPUT_FILE = 'day14.input.txt';
REMOVE_LINE_BREAKS = True

# read input
input = aoc.ImportInput(INPUT_FILE, REMOVE_LINE_BREAKS)

# implementation
BIT_LENGTH = 36
mask = [2] * BIT_LENGTH
mem = dict()

def Get( idx ): 
    m = mem.get(idx)
    if not m: 
        m = [0] * BIT_LENGTH
        mem[idx] = m 
    return m 

def Mask( a, m ): 
    c = deepcopy(a)
    for i in range(BIT_LENGTH): 
        if (m[i] != 2): 
            c[i] = m[i]
    return c 
    
def ToMaskValue( maskValue ):
    if maskValue.isdigit(): 
        return int(maskValue)
    else:
        return 2 

def ToMemory( dec ): 
    value = int(dec) 
    memValue = [0] * BIT_LENGTH
    idx = BIT_LENGTH - 1

    while (idx >= 0) and (value != 0): 
        r = value % 2
        value = value >> 1
        memValue[idx] = r
        idx -= 1

    return memValue

def ToValue( mem ): 
    idx = BIT_LENGTH - 1
    val = 0
    mul = 1
    while (idx >= 0): 
        val += mul * mem[idx]
        mul *= 2
        idx -= 1

    return val

def Part01():
    for line in input: 
        cmd, value = line.split('=')
        cmd = cmd.strip()
        value = value.strip()

        if 'mask' in cmd: 
            mask = [ToMaskValue(c) for c in list(value)]
            # print( 'mask updated to: ' + str(mask) )
        elif 'mem' in cmd: 
            idx = int( cmd[4:].split(']')[0] )
            # print( 'updating mem[{}] to {}'.format( idx, value ) )
            memValue = ToMemory(value) 
            # print( '- binary: ' + str(memValue) )

            memValue = Mask( memValue, mask )
            mem[idx] = memValue 
            val = ToValue( memValue )
            # print( '- actual value written: {} - {}'.format(val, memValue) )

    sum = 0
    for key, value in mem.items():
        val = ToValue(value)
        # print( 'mem[{}] = {}'.format( key, val ) ); 
        sum += val

    print( 'Sum is {}'.format(sum) )
#end part01

Part01(); 

###########################
## Start Part02
###########################

def Combine( mask0, mask1 ):
    newMask = [0] * BIT_LENGTH
    for idx in range(len(mask0)): 
        newMask[idx] = max( mask0[idx], mask1[idx] )
    
    return newMask

def Intersect( a, b ): 
    ret = []
    for idx in range(len(a)): 
        va = a[idx]
        vb = b[idx]
        if (va == vb): 
            ret.append(va)
        elif (va == 2): 
            ret.append(vb)
        elif (vb == 2): 
            ret.append(va)
        else: 
            # no intersection
            return None
    
    return ret 
# end Intersect

def GetSetSize( x ):
    return 0 if x == None else 1 << x.count(2)

def CountIntersections( x, items, idx ):
    if x == None: 
        return 0

    count = 0
    for idx in range(idx, len(items)): 
        item = items[idx]
        nx = Intersect( x, item )
        count += GetSetSize(nx)

    return count 

# add unique entires to this list if there isn't one already that matches
def AddMask( constants, memIdx ):
    # check that an item that matches this key isn't already in here
    total = GetSetSize( memIdx )

    idx = 0
    for item in constants: 
        x = Intersect( item, memIdx )
        if (x == memIdx): 
            # fully contains this key, meaning it is already in the set
            return 0

        # subtract the intersection count
        # and add in the overlap between that intersection and the remaining sets
        # as they'll get re-subtracted later on
        idx += 1
        total = total - GetSetSize(x) + CountIntersections( x, constants, idx )

    # add my set
    constants.append( memIdx )
    return total


def Part02():
    # 36 bit address space - no way I'm going to store that in memory... so instead
    # going to keep track of masks (since every mask will tell me what is in those cells)

    # also, going to work backwards.  since the final people overwrite, keeping a mask 
    # of what has been written so far, so I can ignore it; 
    entries = []

    # parse input
    for line in input: 
        cmd, value = line.split('=')
        cmd = cmd.strip()
        value = value.strip()

        if 'mask' in cmd: 
            mask = [ToMaskValue(c) for c in list(value)]
            # print( 'mask updated to: ' + str(mask) )
        elif 'mem' in cmd: 
            idx = int( cmd[4:].split(']')[0] )
            memValue = ToMemory(value) 

            entry = [idx, memValue, mask]
            entries.append( entry )
    #end for

    # count
    sum = 0
    constants = []

    entryIdx = len(entries) - 1
    while entryIdx >= 0: 
        # get the entry
        entry = entries[entryIdx]
        idx = entry[0]
        memValue = entry[1]
        mask = entry[2]
        value = ToValue(memValue)

        # whatever there is new wild cards, I'm setting at least that many combinations
        memIdx = Combine( ToMemory( idx ), mask )
        count =  AddMask( constants, memIdx )

        sum += value * count
        entryIdx -= 1
    #end while

    print( 'Part02 Sum = {}'.format(sum) )
#end Part02

Part02()




        

    