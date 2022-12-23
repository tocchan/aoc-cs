import io
import re
import sys
import time
from collections import deque
from copy import deepcopy

import aoc
from utils import time_func

# config
INPUT_FILE = 'day22.input.txt'


# read input
file_text = str()
with open(INPUT_FILE, 'r') as input_file:
    file_text = input_file.read()

players = file_text.split('\n\n')
player_decks = list()
for player in players:
    turn_strings = player.split(':')[1].splitlines(False)
    player_decks.append(deque([int(line) for line in turn_strings if line.isdigit()]))


# implementation
def play_turn(deck0, deck1):
    card0 = deck0.popleft()
    card1 = deck1.popleft()

    if card0 > card1:
        deck0.append(card0)
        deck0.append(card1)
    else:
        deck1.append(card1)
        deck1.append(card0)

    if len(deck0) == 0:
        return 1
    elif len(deck1) == 0:
        return 0
    else:
        return -1


def calculate_score(deck):
    total = len(deck)
    score = 0
    for card in deck:
        score += total * card
        total -= 1
    return score


@time_func
def part01():
    deck0 = deepcopy(player_decks[0])
    deck1 = deepcopy(player_decks[1])

    result = play_turn(deck0, deck1)
    while result < 0:
        result = play_turn(deck0, deck1)

    ans = int()
    if result == 0:
        ans = calculate_score(deck0)
    else:
        ans = calculate_score(deck1)

    print(f'part01 answer: {ans}')
# end part01


def get_key(deck0, deck1):
    check = ''.join([chr(32 + c) for c in deck0]) + ' ' + ''.join([chr(32 + c) for c in deck1])
    return check


all_games = dict()


def play_round(deck0, deck1):
    game_key = get_key(deck0, deck1)
    if game_key in all_games:
        return all_games[game_key]
    opposite_key = get_key(deck1, deck0)

    seen_configs = set()
    while (len(deck0) > 0) and (len(deck1) > 0):
        # has this configuration been seen
        check = get_key(deck0, deck1)
        if check in seen_configs:
            # player 1 wins
            return 0
            # deck0.append(deck0.popleft())
            # deck0.append(deck1.popleft())
        else:
            seen_configs.add(check)

            # play round as normal
            card0 = deck0.popleft()
            card1 = deck1.popleft()
            if card0 <= len(deck0) and card1 <= len(deck1):
                # recurse round
                next_deck0 = deque(list(deck0)[:card0])
                next_deck1 = deque(list(deck1)[:card1])
                winner = play_round(next_deck0, next_deck1)
                if winner == 0:
                    deck0.append(card0)
                    deck0.append(card1)
                else:
                    deck1.append(card1)
                    deck1.append(card0)
            else:
                # normal round
                if card0 > card1:
                    deck0.append(card0)
                    deck0.append(card1)
                else:
                    deck1.append(card1)
                    deck1.append(card0)
        # end normal round
    # end while

    winner = 1 if len(deck0) == 0 else 0
    all_games[game_key] = winner
    all_games[opposite_key] = 1 - winner

    return winner


@time_func
def part02():
    deck0 = deepcopy(player_decks[0])
    deck1 = deepcopy(player_decks[1])

    result = play_round(deck0, deck1)

    ans = int()
    if result == 0:
        ans = calculate_score(deck0)
    else:
        ans = calculate_score(deck1)

    print(f'part02 answer: {ans}')
# end part02


# run the parts
part01()
part02()
