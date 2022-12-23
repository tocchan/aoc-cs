from time import perf_counter
from functools import wraps

"""
Courtesy of Chris Cunningham on the Tech-Art slack
"""


def time_func(func):
    @wraps(func)
    def wrapper(*args, **kwargs):
        # run the function and time it
        start = perf_counter()
        ret = func(*args, **kwargs)
        dur = perf_counter() - start

        # display the data
        func_name = func.__name__
        display = f"{func_name} took: {dur:.4f}s"
        if dur < .001:
            display = f"{func_name} took: {dur * 1000000.0:.4f}us"
        elif dur < .5:
            display = f"{func_name} took: {dur * 1000.0:.4f}ms"
        print(display)

        # return the result of the function call
        return ret
    return wrapper
