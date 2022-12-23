import io
import math
import re
import sys
import time
from copy import deepcopy

import aoc
from utils import time_func

# config
INPUT_FILE = 'day17.input.txt';
REMOVE_LINE_BREAKS = True

# read input
input_sets = aoc.ImportInput(INPUT_FILE, REMOVE_LINE_BREAKS)

# there is a single `1` slide in this one, so lets make our map
slices = []
space_slice = []
for input_set in input_sets:
    space_slice.append(list(input_set))
slices.append(space_slice)


# implementation
def print_slices(layers):
    z = -math.floor(len(layers) / 2)
    for layer in layers:
        print(f'z = {z}')
        for line in layer:
            print(''.join(line))
        z += 1
        print('')


def create_layer(xsize, ysize):
    layer = []
    y = 0
    while y < ysize:
        line = ['.'] * xsize
        layer.append(line)
        y += 1
    return layer


def copy_to_slice(dst, src, dx, dy):
    xsize = len(src[0])
    ysize = len(src)

    y = 0
    while y < ysize:
        x = 0
        while x < xsize:
            dst[y + dy][x + dx] = src[y][x]
            x += 1
        y += 1


def grow(layers):
    new_layers = []

    zsize = len(layers)
    ysize = len(layers[0]) + 2
    xsize = len(layers[0][0]) + 2

    new_layers.append( create_layer(xsize, ysize) )
    z = 0
    while z < zsize:
        layer = create_layer(xsize, ysize)
        old_layer = layers[z]
        copy_to_slice(layer, old_layer, 1, 1)
        new_layers.append(layer)
        z += 1
    new_layers.append( create_layer(xsize, ysize) )

    return new_layers


def count_neighbors(layers, x, y, z):
    zsize = len(layers)
    ysize = len(layers[0])
    xsize = len(layers[0][0])

    count = 0
    for dz in range(-1, 2):
        nz = z + dz
        if nz < 0 or nz >= zsize:
            continue

        for dy in range(-1, 2):
            ny = y + dy
            if ny < 0 or ny >= ysize:
                continue
            for dx in range(-1, 2):
                nx = x + dx
                if nx < 0 or nx >= xsize:
                    continue

                if layers[nz][ny][nx] == '#':
                    count += 1
            # end for dx
        # end for dy
    # end  for dz
    return count
# end count_neighbors

def trim(layers):
    # todo - cut off unneeded layers
    return layers


def iterate(layers):
    layers = grow(layers)
    next_layers = deepcopy(layers)

    zsize = len(layers)
    ysize = len(layers[0])
    xsize = len(layers[0][0])

    z = 0
    while z < zsize:
        y = 0
        while y < ysize:
            x = 0
            while x < xsize:
                is_on = layers[z][y][x] == '#'
                on_count = count_neighbors(layers, x, y, z)
                if is_on:
                    on_count -= 1

                if is_on and ((on_count < 2) or (on_count > 3)):
                    next_layers[z][y][x] = '.'
                elif not is_on and (on_count == 3):
                    next_layers[z][y][x] = '#'
                x += 1
            y += 1
        z += 1
    # end while

    return trim(next_layers)


def count_active_in_slice(layer):
    count = 0
    for line in layer:
        for c in line:
            if c == '#':
                count += 1
    return count


def count_active(layers):
    count = 0
    for layer in layers:
        count += count_active_in_slice(layer)
    return count

@time_func
def part01(layers):
    print_slices(layers)
    for i in range(6):
        layers = iterate(layers)
        # print_slices(layers)

    count = count_active(layers)
    print(f'Active Cells: {count}')

# end part01

def create_ddd(sx, sy, sz):
    ddd = []
    for iz in range(sz):
        ddd.append(create_layer(sx, sy))
    return ddd


def copy_to_cube(dst, src, dx, dy, dz):
    sz = len(src)
    for iz in range(sz):
        nz = dz + iz
        copy_to_slice(dst[nz],src[iz], dx, dy)


def grow_4d(dddd):
    the_copy = []
    wsize = len(dddd)
    zsize = len(dddd[0]) + 2
    ysize = len(dddd[0][0]) + 2
    xsize = len(dddd[0][0][0]) + 2

    the_copy.append( create_ddd(xsize, ysize, zsize) )
    for w in range(wsize):
        ddd = create_ddd(xsize, ysize, zsize)
        copy_to_cube(ddd, dddd[w], 1, 1, 1)
        the_copy.append(ddd)
    the_copy.append( create_ddd(xsize, ysize, zsize) )

    return the_copy
#end grow_4d

def count_neighbors_4d(dddd, x, y, z, w):
    count = 0
    wsize = len(dddd)
    for dw in range(-1, 2):
        nw = w + dw
        if nw < 0 or nw >= wsize:
            continue

        count += count_neighbors(dddd[nw], x, y, z)
    return count


def iterate_4d(dddd):
    layers = grow_4d(dddd)
    next_layers = deepcopy(layers)

    wsize = len(layers)
    zsize = len(layers[0])
    ysize = len(layers[0][0])
    xsize = len(layers[0][0][0])

    w = 0
    while w < wsize:
        z = 0
        while z < zsize:
            y = 0
            while y < ysize:
                x = 0
                while x < xsize:
                    is_on = layers[w][z][y][x] == '#'
                    on_count = count_neighbors_4d(layers, x, y, z, w)
                    if is_on:
                        on_count -= 1
                    if is_on and ((on_count < 2) or (on_count > 3)):
                        next_layers[w][z][y][x] = '.'
                    elif not is_on and (on_count == 3):
                        next_layers[w][z][y][x] = '#'
                    x += 1
                y += 1
            z += 1
        w += 1
    # end while

    return next_layers


def count_4d(dddd):
    count = 0
    for ddd in dddd:
        count += count_active(ddd)
    return count


@time_func
def part02(layers):
    dddd = [layers]
    for i in range(6):
        dddd = iterate_4d(dddd)

    count = count_4d(dddd)
    print(f'4d active cells = {count}')
    return count
# end part02


# run the parts
part01(slices)
part02(slices)

