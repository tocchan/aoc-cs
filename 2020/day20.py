import io
import math
import re
import sys
import time
from copy import deepcopy

import aoc
from utils import time_func

# config
INPUT_FILE = 'day20.input.txt';
REMOVE_LINE_BREAKS = True


# read input
file_text = ""
with open(INPUT_FILE, 'r') as input_file:
    file_text = input_file.read()

tile_nums = []
tile_maps = []

tile_texts = file_text.split('\n\n')
for tile_text in tile_texts:
    lines = tile_text.splitlines(False)
    tile_num = lines[0][5:].split(':')[0]
    tile_nums.append(int(tile_num))
    tile_maps.append([list(line) for line in lines[1:]])

map_count = len(tile_maps)
tile_width = len(tile_maps[0][0])

grid_width = int(math.sqrt(map_count))


# implementation
def grid_index(x, y):
    return y * grid_width + x


def get_rotated(tile):
    copy = deepcopy(tile)
    width = len(tile[0])
    for y in range(width):
        for x in range(width):
            ch = tile[y][x]
            copy[x][width - y - 1] = ch
    return copy


def get_flipped(tile):
    copy = deepcopy(tile)
    width = len(tile[0])
    for y in range(width):
        for x in range(width):
            copy[width - y - 1][x] = tile[y][x]
    return copy


def rotate_tile(tile_idx):
    tile = tile_maps[tile_idx]
    copy = get_rotated(tile)
    tile_maps[tile_idx] = copy


copy_nums = []
copy_tiles = []
for idx, tile in enumerate(tile_maps):
    num = tile_nums[idx]
    copy_nums += [num] * 8

    for i in range(4):
        copy_tiles.append(tile)
        copy_tiles.append(get_flipped(tile))
        tile = get_rotated(tile)

tile_nums = copy_nums
tile_maps = copy_tiles


# side is 0 1 2 3 compares tile0's right, bottom, left, or top side against tile1's opposite side
def tiles_match_side(tile0_idx, tile1_idx, side):
    # nothing there yet - so valid
    if (tile0_idx == -1) or (tile1_idx == -1):
        return True

    ix0 = 0
    iy0 = 0
    ix1 = 0
    iy1 = 0
    dx = 0
    dy = 1

    if side == 0:
        # my right side
        ix0 = tile_width - 1
    elif side == 1:
        iy0 = tile_width - 1
        dx = 1
        dy = 0
    elif side == 2:
        ix1 = tile_width - 1
    elif side == 3:
        iy1 = tile_width - 1
        dx = 1
        dy = 0

    tile0 = tile_maps[tile0_idx]
    tile1 = tile_maps[tile1_idx]
    for i in range(tile_width):
        if tile0[iy0][ix0] != tile1[iy1][ix1]:
            return False
        ix0 += dx
        ix1 += dx
        iy0 += dy
        iy1 += dy

    return True
# end tiles_match_side


def can_fit(grid_map, grid_idx, tile_idx):
    grid_y = int(grid_idx / grid_width)
    grid_x = grid_idx % grid_width

    lx = grid_x - 1
    rx = grid_x + 1
    ty = grid_y - 1
    by = grid_y + 1

    if rx < grid_width:
        right_tile = grid_map[grid_index(rx, grid_y)]
        if not tiles_match_side(tile_idx, right_tile, 0):
            return False

    if lx >= 0:
        left_tile = grid_map[grid_index(lx, grid_y)]
        if not tiles_match_side(tile_idx, left_tile, 2):
            return False

    if by < grid_width:
        bot_tile = grid_map[grid_index(grid_x, by)]
        if not tiles_match_side(tile_idx, bot_tile, 1):
            return False

    if ty >= 0:
        bot_tile = grid_map[grid_index(grid_x, ty)]
        if not tiles_match_side(tile_idx, bot_tile, 3):
            return False

    return True
# end can_fit


def place_in_grid(grid_map, grid_idx, tile_indices):
    if len(tile_indices) == 0:
        return True

    for tile_idx in tile_indices:
        if can_fit(grid_map, grid_idx, tile_idx):
            grid_map[grid_idx] = tile_idx
            group_idx = int(tile_idx / 8)
            new_tiles = [i for i in tile_indices if int(i / 8) != group_idx]

            if place_in_grid(grid_map, grid_idx + 1, new_tiles):
                return True

    if grid_idx == 0:
        grid_idx = grid_idx

    grid_map[grid_idx] = -1
    return False
# end place_in_grid


def find_tile_placements(tile_indices):
    grid_map = [-1] * map_count
    place_in_grid(grid_map, 0, tile_indices)
    return grid_map


@time_func
def part01():
    tile_indices = [i for i in range(len(tile_maps))]

    grid = find_tile_placements(tile_indices)
    top_left = grid_index(0, 0)
    top_right = grid_index(grid_width - 1, 0)
    bottom_left = grid_index(0, grid_width - 1)
    bottom_right = grid_index(grid_width - 1, grid_width - 1)

    corner_indices = [top_left, top_right, bottom_left, bottom_right]
    tile_indices = [grid[corner_idx] for corner_idx in corner_indices]
    tile_ids = [tile_nums[tile_idx] for tile_idx in tile_indices]

    ans = tile_ids[0] * tile_ids[1] * tile_ids[2] * tile_ids[3]

    print(f'part01 answer: {ans}')
    return grid
# end part01


def paste_into_map(final_map, gx, gy, tile):
    mx = gx * (tile_width - 2)
    my = gy * (tile_width - 2)
    for ty in range(0, tile_width - 2):
        for tx in range(0, tile_width - 2):
            final_map[my + ty][mx + tx] = tile[ty + 1][tx + 1]


def mark_sea_monster(seamap, x, y, seamonster):
    sw = len(seamonster[0])
    sh = len(seamonster)
    mh = len(seamap)
    mw = len(seamap[0])

    for dy in range(sh):
        for dx in range(sw):
            if seamonster[dy][dx] == '#' and seamap[y + dy][x + dx] != '#':
                return False

    # mark him
    for dy in range(sh):
        for dx in range(sw):
            if seamonster[dy][dx] == '#':
                seamap[y + dy][x + dx] = 'O'

    return True

def find_sea_monsters(seamap):
    seamonster = [list('                  # '),
                  list('#    ##    ##    ###'),
                  list(' #  #  #  #  #  #   ')]

    sw = len(seamonster[0])
    sh = len(seamonster)
    mh = len(seamap)
    mw = len(seamap[0])

    found = False
    for y in range(mh - sh + 1):
        for x in range(mw - sw + 1):
            found = mark_sea_monster(seamap, x, y, seamonster) or found

    return found



@time_func
def part02(grid_ids):
    map_width = grid_width * (tile_width - 2)
    final_map = []
    for line_idx in range(map_width):
        final_map.append(['.'] * map_width)

    for grid_idx, tile_idx in enumerate(grid_ids):
        gx = grid_idx % grid_width
        gy = int(grid_idx / grid_width)

        tile = tile_maps[tile_idx]
        paste_into_map(final_map, gx, gy, tile)

    # okay, I have a map, rotate and flip until I can find a sea monster
    found = find_sea_monsters(final_map)
    rot_idx = 0
    while (not found) and rot_idx < 4:
        final_map = get_rotated(final_map)
        found = find_sea_monsters(final_map)
        rot_idx += 1

    if not found:
        final_map = get_flipped(final_map)
        found = find_sea_monsters(final_map)
        rot_idx = 0
        while (not found) and rot_idx < 4:
            final_map = get_rotated(final_map)
            found = find_sea_monsters(final_map)
            rot_idx += 1

    # well, hope I found one, count '#'
    count = 0
    for row in final_map:
        for item in row:
            if item == '#':
                count += 1

    print(f'part02 answer: {count}')
# end part02


# print(str(tiles_match_side(1, 0, 2)))

# run the parts
grid = part01()
part02(grid)

