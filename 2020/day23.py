import io
import re
import sys
import time
from collections import deque
from copy import deepcopy

import aoc
from utils import time_func

# config
INPUT_FILE = 'day23.sample.txt'


# read input
# file_text = str()
# with open(INPUT_FILE, 'r') as input_file:
#     file_text = input_file.read()

# cups = [3, 8, 9, 1, 2, 5, 4, 6, 7]
cups = [9,6,2,7,1,3,8,5,4]

min_cup = min(s for s in cups)
max_cup = max(s for s in cups)


def prev_card(idx, count):
    val = idx - 1
    if val < 0:
        val = count - 1
    return val


def get_next(start_idx, next_ptrs):
    next_cards = list()
    idx = start_idx
    for i in range(3):
        next_cards.append(next_ptrs[idx])
        idx = next_ptrs[idx]
    return next_cards


def from_linked_list(next_ptr, start_idx, ret_length):
    final_list = []
    cur_idx = start_idx
    for i in range(ret_length):
        final_list.append(cur_idx + 1)
        cur_idx = next_ptr[cur_idx]
    return final_list


def run_game(cup_list, turns, ret_length):
    # making a circular doubly linked list
    # but with parallel arrays as I don't want to make the object
    count = len(cup_list)
    next_ptr = [i + 1 for i in range(count)]
    next_ptr[len(next_ptr) - 1] = cup_list[0] - 1

    # going to make the value the index + 1, so cleanup the start of this array
    for idx in range(9):
        next_idx = idx + 1 if idx < count - 1 else 0
        val = cup_list[idx]
        next_val = cup_list[next_idx]
        next_ptr[val - 1] = next_val - 1

    # so to see what card is next, just ask what is next
    # my cur index is at the first cup_list
    cur_idx = cup_list[0] - 1

    for turn in range(turns):
        next_values = get_next(cur_idx, next_ptr)
        next_idx = prev_card(cur_idx, count)
        while next_idx in next_values:
            next_idx = prev_card(next_idx, count)

        # okay, we know our next idx, point this to our next list
        # first, remove it from after where we were
        next_val = next_ptr[next_values[2]]
        next_ptr[cur_idx] = next_val

        # patch it up to where we want it to be
        next_next = next_ptr[next_idx]
        next_ptr[next_idx] = next_values[0]
        next_ptr[next_values[2]] = next_next

        cur_idx = next_ptr[cur_idx]

        # test = from_linked_list(next_ptr, cur_idx)
        # print(test)
    # end for

    # reconstruct it into a list
    final_list = from_linked_list(next_ptr, 0, ret_length)

    return final_list
# end run_game


@time_func
def part01():
    cup_list = deepcopy(cups)
    final_list = run_game(cup_list, 100, len(cup_list))

    ans = ''.join([str(cup) for cup in final_list[1:]])
    print(f'part01 answer: {ans}')
# end part01


@time_func
def part02():
    cup_list = deepcopy(cups)
    for i in range(max_cup + 1, 1000000 + 1):
        cup_list.append(i)

    final_list = run_game(cup_list, 10000000, 3)
    ans = final_list[1] * final_list[2]

    print(f'part02 answer: {ans}')
# end part02


# run the parts
part01()
part02()
