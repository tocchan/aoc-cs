import io
import re
import sys
import time
from copy import deepcopy

import aoc
from utils import time_func

# config
INPUT_FILE = 'day18.input.txt';
REMOVE_LINE_BREAKS = True


# read input
input_sets = aoc.ImportInput(INPUT_FILE, REMOVE_LINE_BREAKS)




# implementation


def get_close_index(line):
    count = 1
    for i, c in enumerate(line):
        if c == '(':
            count += 1
        elif c == ')':
            count -= 1
            if count == 0:
                return i
    return -1


def solve_eq(line, is_part2):
    while '(' in line:
        parts = line.split('(', 1)
        end_idx = get_close_index(parts[1])
        inner = parts[1][:end_idx]
        end = parts[1][end_idx + 1:]

        line = parts[0] + str(solve_eq(inner, is_part2)) + end

    # pieces should be number op number op number
    pieces = line.split(' ')
    if len(pieces) == 0:
        return 0

    if is_part2:
        nums = []
        ops = []

        for p in pieces:
            if p.isdigit():
                nums.append(int(p))
            else:
                if p == '*':
                    # list is all pluses, evaluate them first
                    op_count = len(ops)
                    while (len(nums) > 1) and (op_count > 0) and (ops[-1] == '+'):
                        ops.pop()
                        a = nums.pop()
                        b = nums.pop()
                        c = a + b
                        nums.append(c)
                    ops.append(p)
                elif p == '+':
                    ops.append(p)

        while len(ops) > 0:
            a = nums.pop()
            b = nums.pop()
            op = ops.pop()
            c = 0
            if op == '*':
                c = a * b
            else:
                c = a + b
            nums.append(c)

        return nums.pop()
        # end part02
    else:
        ans = int(pieces[0])
        i = 1
        while i < len(pieces):
            op = pieces[i]
            num = int(pieces[i + 1])
            i += 2
            if op == '+':
                ans += num
            else:
                ans *= num
        return ans


@time_func
def part01(lines):
    ans = 0
    for line in lines:
        ans += solve_eq(line, False)

    print(f'part01 answer: {ans}')
# end part01


@time_func
def part02(lines):
    ans = 0
    for line in lines:
        ans += solve_eq(line, True)

    print(f'part02 answer: {ans}')
# end part02


# run the parts
part01(input_sets)
part02(input_sets)

