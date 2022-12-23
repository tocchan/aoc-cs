import io
import re
import sys
import time
from copy import deepcopy

import aoc
from utils import time_func

# config
INPUT_FILE = 'day19.input.txt';
REMOVE_LINE_BREAKS = True


# read input
file_text = ""
with open(INPUT_FILE, 'r') as input_file:
    file_text = input_file.read()

lines = file_text.splitlines(False)
rule_texts = []
inputs = []

line_count = len(lines)
line_idx = 0
longest_line = 0
while line_idx < line_count:
    line = lines[line_idx]
    if not line:
        break;
    rule_texts.append(line)
    line_idx += 1

line_idx += 1
while line_idx < line_count:
    line = lines[line_idx]
    inputs.append(line)
    longest_line = max(len(line), longest_line)
    line_idx += 1

def to_sequence(text_val):
    if text_val.isdigit():
        return int(text_val)
    else:
        return text_val.replace('"', '')

rules = {}
a_rule = -1
b_rule = -1

rule_texts.append('8: 42 | 42 8')
rule_texts.append('11: 42 31 | 42 11 31')
for rule_text in rule_texts:
    key, branches = rule_text.split(':', 1)
    ikey = int(key)
    branches = branches.split('|')

    rule_seq = []
    for branch in branches:
        sequence = [to_sequence(s) for s in branch.strip().split(' ')]
        rule_seq.append(sequence)

    rules[ikey] = rule_seq


# implementation

def permute(set0, set1, max_len):
    if len(set0) == 0:
        return set1
    elif len(set1) == 0:
        return set0

    values = []
    for i in set0:
        for j in set1:
            new_item = i + j
            if len(new_item) <= max_len and new_item not in values:
                values.append(new_item)
    return values


# rules is a map to potential sequences
# inputs are the inputs at the bottom of the file
def generate_sequences(key, max_len):
    sequences = rules[key]
    values = []
    if max_len <= 0:
        return values

    for sequence in sequences:
        options = []
        cnt = 0
        seq_len = len(sequence)
        for item in sequence:
            if isinstance(item, int):
                next_sequences = generate_sequences(item, max_len - (seq_len - 1))
                options = permute(options, next_sequences, max_len)
            else:
                options = permute(options, [item], max_len)
            cnt += 1

        values += options

    return values


def rule_is_valid(seq, rule_seq):
    if len(seq) < len(rule_seq):
        return False

    # try all combinations of the following rules
    # if there is only one rule, fall down to it
    seq_len = len(seq)
    rule_count = len(rule_seq)
    if rule_count == 1:
        return sequence_is_valid(rule_seq[0], seq)

    # if there are multiple rules, try every possible split of our input
    max_len = seq_len - (rule_count - 1)
    for i in range(1, max_len + 1):
        if sequence_is_valid(rule_seq[0], seq[:i]) and rule_is_valid(seq[i:], rule_seq[1:]):
            return True
    return False


def sequence_is_valid(key, seq):
    if not isinstance(key, int):
        return key == seq

    # empty string means done
    if not seq:
        return False

    # okay, split my sequence evenly over the search space
    rule = rules[key]
    for rule_seq in rule:
        if rule_is_valid(seq, rule_seq):
            return True
    return False


# valid_sequences = generate_sequences(0, longest_line)
# print(f'Sequences Generated: {len(valid_sequences)}')

@time_func
def part01():
    print('part1: nope')
# end part01


# rule_key is the thing that we're searching for
# pre is what this item has to start with
# post is what this item must end with
# offset is index this was first predicted/scanned, or original completion step
def add_to_set(set_ref, rule_key, pre, post, offset):
    item = [rule_key, pre, post, offset]
    if item not in set_ref:
        set_ref.append(item)


# rule is finished if it has no next steps
def rule_finished(rule):
    return len(rule[2]) == 0


def is_terminal(item):
    return item == 'a' or item == 'b'


def complete_rule(sets, rule_set, offset):
    # so for this, any set where my pre item is the same as their first post item
    # I make a new set, with that rules  key, with this as the pre, and their post being one less
    set_key = rule_set[0]
    source_idx = rule_set[3]
    source_set = sets[source_idx]
    if source_idx == offset:
        print('should not happen - if so, I messed up an offset')
        return

    for rule in source_set:
        post = rule[2]
        if (len(post) > 0) and (set_key == post[0]):
            add_to_set(sets[offset], rule[0], [post[0]], post[1:], rule[3])


def scan_rule(sets, rule_set, offset, word):
    if offset >= len(word):
        return

    # for every rule I can potentially 'finish',
    # add it to the next set
    ch = word[offset]
    next_rule_set = sets[offset + 1]
    source_set_idx = rule_set[3]

    if (len(rule_set[2]) > 0) and (rule_set[2][0] == ch):
        # this is a scan, flip that rule about and add it to the next
        # group, having the offset of the originating rule
        add_to_set(next_rule_set, rule_set[0], [ch], [], source_set_idx)
# end scan_rule


# based on our current rule set
def predict_rule(sets, rule_set, offset, grammar):
    # what can this rule go to
    post = rule_set[2][0]

    # add predictions for those rules
    sequences = grammar[post]
    for seq in sequences:
        add_to_set(sets[offset], post, [], seq, offset)


# The 'complete' is -1, any set
# that contains it
def contains_pass(rule):
    for item in rule:
        if (item[0] == -1) and (item[1] == [0]) and rule_finished(item):
            return True
    return False


def earley_parse(word, grammar):
    sets = []

    for i in range(len(word) + 1):
        sets.append([])

    # -1 is my 'complete' state, 0 goes to it
    add_to_set(sets[0], -1, [], [0], 0)

    set_count = len(sets)
    for set_idx in range(set_count):
        rule_sets = sets[set_idx]
        rule_set_idx = 0
        while rule_set_idx < len(rule_sets):
            rule_set = rule_sets[rule_set_idx]
            if rule_finished(rule_set):
                complete_rule(sets, rule_set, set_idx)
            else:
                next_char = rule_set[2][0]
                if is_terminal(next_char):
                    scan_rule(sets, rule_set, set_idx, word)
                else:
                    predict_rule(sets, rule_set, set_idx, grammar)
            rule_set_idx += 1
    # end for

    # passes if the final set contains a rule pointing back to -1
    return contains_pass(sets[len(word)])
# end earley parse

@time_func
def part02():
    count = 0
    for word in inputs:
        if earley_parse(word, rules):
            count += 1

    print(f'part02 answer: {count}')
# end part02


# run the parts
# part01()
part02()

