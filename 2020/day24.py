import io
import re
import sys
import time
from collections import deque
from copy import deepcopy


import aoc
from utils import time_func

# config
INPUT_FILE = 'day24.input.txt'


# read input
file_text = str()
with open(INPUT_FILE, 'r') as input_file:
    file_text = input_file.read()


# implementation

east = [1, 0, -1]
west = [-1, 0, 1]
northwest = [-1, 1, 0]
southeast = [1, -1, 0]
northeast = [0, 1, -1]
southwest = [0, -1, 1]


def add_vec3(a, b):
    for i in range(3):
        a[i] += b[i]


def neg_vec3(v):
    return [-v[0], -v[1], -v[2]]


def parse_line(dirs):
    dir_len = len(dirs)
    idx = 0
    pos = [0, 0, 0]
    token = ''
    while idx < dir_len:
        token += dirs[idx]
        if token == 'e':
            add_vec3(pos, east)
            token = ''
        elif token == 'w':
            add_vec3(pos, west)
            token = ''
        elif token == 'se':
            add_vec3(pos, southeast)
            token = ''
        elif token == 'sw':
            add_vec3(pos, southwest)
            token = ''
        elif token == 'nw':
            add_vec3(pos, northwest)
            token = ''
        elif token == 'ne':
            add_vec3(pos, northeast)
            token = ''

        idx += 1
    # end while
    return pos


def get_key(vec):
    return ','.join(str(v) for v in vec)


@time_func
def part01():
    lines = file_text.splitlines(False)
    black_tiles = {}
    for line in lines:
        pos = parse_line(line)
        key = get_key(pos)
        if key in black_tiles:
            black_tiles[key] = not black_tiles[key]
        else:
            black_tiles[key] = True

    ans = 0
    for tile in black_tiles:
        is_black_tile = black_tiles[tile]
        ans += 1 if is_black_tile else 0

    print(f'part01 answer: {ans}')
    return black_tiles
# end part01


def get_neighbor_keys(tile):
    vec = [int(v) for v in tile.split(',')]
    neighbors = []
    for i in range(6):
        neighbors.append(deepcopy(vec))

    add_vec3(neighbors[0], east)
    add_vec3(neighbors[1], west)
    add_vec3(neighbors[2], northeast)
    add_vec3(neighbors[3], northwest)
    add_vec3(neighbors[4], southeast)
    add_vec3(neighbors[5], southwest)

    ret_keys = [get_key(v) for v in neighbors]

    return ret_keys


def count_neighbors(tile_map, tile):
    neighbors = get_neighbor_keys(tile)
    count = 0
    for neighbor in neighbors:
        if neighbor in tile_map:
            is_black = tile_map[neighbor]
            count += 1 if is_black else 0

    return count


# make sure tiles near a black tile exist in the map before we run
def spread_day(prev_day):
    day = deepcopy(prev_day)
    for key in prev_day:
        is_black = prev_day[key]
        if is_black:
            neighbors = get_neighbor_keys(key)
            for neighbor in neighbors:
                if neighbor not in day:
                    day[neighbor] = False
    return day


def run_day(prev_day):
    day = spread_day(prev_day)

    for tile in day:
        is_black = day[tile]
        nearby_count = count_neighbors(prev_day, tile)
        if is_black:
            if nearby_count == 0 or nearby_count > 2:
                day[tile] = False
        else:
            if nearby_count == 2:
                day[tile] = True

    return day


@time_func
def part02(art):
    for turn in range(100):
        # print(f'day {turn + 1}...')
        art = run_day(art)

    ans = 0
    for tile in art:
        is_black_tile = art[tile]
        ans += 1 if is_black_tile else 0

    print(f'total tiles: {len(art)}')
    print(f'part02 answer: {ans}')
# end part02


# run the parts
initial_map = part01()
part02(initial_map)
