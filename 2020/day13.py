import io
import re
import sys
import time
from copy import deepcopy 

import aoc


# config
INPUT_FILE = 'day13.input.txt';
REMOVE_LINE_BREAKS = True

# read input
input = aoc.ImportInput(INPUT_FILE, REMOVE_LINE_BREAKS)

# implementation
# for idx, line in enumerate(input): 
# print( str(idx) + ': ' + line )

time = int( input[0] )
ids = [s.strip() for s in input[1].split(',')]
departs = []
for id in ids: 
    if id.isdigit(): 
        departs.append( int(id) )

nextDepartTime = []
nextIdx = 0
for idx, d in enumerate(departs): 
    v = time % d
    n = time + d - v
    if v == 0: 
        n = time
    
    nextDepartTime.append(n)
    # print( '{} departs at {}'.format( d, n ) )

    delay = n - time
    shortestDelay = nextDepartTime[nextIdx] - time 
    if (delay < shortestDelay): 
        nextIdx = idx

id = departs[nextIdx]
depTime = nextDepartTime[nextIdx]
depDelay = depTime - time
    
print( '{} is leaving at {}, answer: {}'.format( id, depTime, depDelay * id ) )


## part02 
values = []

# The number I get will be a multiple of all of these, so get the 

def Factorize( v ):
    factors = []
    f = 2
    while f <= v: 
        if (v % f) == 0: 
            factors.append(f)
            v /= f
        else:
            f += 1 
    
    if (v > 1):
        factors.append(v)

    factors.sort()
    return factors 
    
def CombineFactors( factors0, factors1 ):
    factors = []
    for f in factors0: 
        factors.append(f)
        if f in factors1: 
            factors1.remove(f)
    
    for f in factors1:
        factors.append(f)

    factors.sort() 
    return factors


# So, want to figure out at what point am I 
# equal to the offset in that modulo group
# doing this brute force (for this problem to work out, I'm guaranteed these must be spanning)
def GetCyclesNeeded(val, group, inGroup): 
    # be sure to be in the cycle
    while inGroup < 0:
        inGroup += group 
    inGroup = inGroup % group

    c = 0
    m = 0
    while m != inGroup: 
        m = (m + val) % group
        c += 1

    return c


# First, the first set of constraints
# constraints are all equations that solve to the same value, and must
# be satisfied for a solution to 'work'
# our first solution is some `primary * X`, and we're trying to find that X
primary = int(ids[0])
constraints = []
for idx, id in enumerate(ids): 
    # don't care about the first guy, he's our first control (primary)
    if idx == 0: 
        continue

    # dont care about non digits outside of them upping the offset
    if not id.isdigit(): 
        continue 

    # next, figure out our constraint
    # going to be something of the form
    # v * (i + px) - o
    # where...
    # - p is our primary
    # - v is our id
    # - x is the value we can choose that will solve this constraint (what we're trying to reduce to)
    # - i is initial multiply to get us into the right offset from our kernal (kernal being all values that map to 0 in the primary modulo group)
    # - o is our offset from the primary modulo group
    digit = int(id)  # v 
    offset = idx     # o
    cycle = GetCyclesNeeded( digit, primary, offset )  # i 

    # simplify equation - I know these are part of the modulo group, so group coefficient and consant
    # pvx + (iv - o) -> `pv` and `iv - o`, since p is the variable we can move about
    # Since the constraint is putting this into the modulo group, I know `pv` and `iv - o` are multiples of p, so will factor that out now; 
    coef = digit
    constant = (digit * cycle - offset) / primary 
    constraints.append( [coef, constant] )

    # remember to multiply the final solution by primary again
# end setup

# figure out the contraint c1 would give to c0
def GetConstraint( c0, c1 ): 
    p0 = c0[0]
    k0 = c0[1]
    p1 = c1[0]
    k1 = c1[1]

    k = k0 - k1 
    
    # so to get solutions to c0 and c1 that would solve for each other
    # x1 = (p0 * x0 + k) / p1 
    
    # this equation must be a whole number, so what I'm trying to find
    # what values for 'x0' work for this, that are divisible by py p1
    # so where do p0 lie in p1's group, plus every subsequent cycle
    # ie;  x0 = (-k mod p1) + p1 * y0, where y0 could be anything 
    coef = p1 
    constant = GetCyclesNeeded( p0, p1, p1 - k )
    
    return [coef, constant]
#end GetConstraint

# no keep reducing constraints until there is one, and then solve using that 
def Reduce( eqs ): 
    # test 
    print( '--- Constaints: {} ---'.format( len(eqs) ) )
    print( str(eqs) )
    print( '------' )

    # if there is only one constraint, we just return the constant
    # as it is our lowest solution
    if len(eqs) == 1:
        return eqs[0][1] 

    # if there are multiple constraints, figure out what 
    # the constraints for our first equation in the passed in set 
    setConstraints = []
    first = eqs[0]
    for idx in range(1, len(eqs)): 
        eq = eqs[idx]
        newConstraint = GetConstraint( first, eq )
        setConstraints.append( newConstraint )

    # with my new constraints, find the solution and plug it back
    # into the first equation.  Pass it up!
    val = Reduce( setConstraints )
    return eqs[0][0] * val + eqs[0][1] 
# end reduct
    

k0 = Reduce( constraints )
print( "Final solution: {}".format(primary * k0) )









