import io
import re
import sys
import time
from collections import deque
from copy import deepcopy


import aoc
from utils import time_func

# config
INPUT_FILE = 'day25.input.txt'


# read input
file_text = str()
with open(INPUT_FILE, 'r') as input_file:
    file_text = input_file.read()

public_card, public_door = [int(line) for line in file_text.splitlines(False)]
subject_number = 7
divisor = 20201227

def calc_loop_size(public_key):
    num = 1
    loop_size = 0
    while num != public_key:
        num = num * subject_number
        num = num % divisor
        loop_size += 1

    return loop_size


def calc_private_key(subject, loop_size):
    num = 1
    for i in range(loop_size):
        num = num * subject
        num = num % divisor
    return num


# implementation
@time_func
def part01():
    card_loop = calc_loop_size(public_card)
    door_loop = calc_loop_size(public_door)

    print(f'card loop size = {card_loop}')
    print(f'door loop size = {door_loop}')

    priv_key0 = calc_private_key(public_card, door_loop)
    # priv_key1 = calc_private_key(public_door, card_loop)

    ans = priv_key0
    print(f'part01 answer: {ans}')
# end part01


@time_func
def part02():
    ans = 0
    print(f'part02 answer: {ans}')
# end part02


# run the parts
part01()
part02()
