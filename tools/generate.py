
info = "Generate variants of one text."

args = [
    ("path", "Path to file with generate format.", path),
    ("variants", "Variants count to generate.", integer)
]


def run(path, variants):
    with open(path) as f:
        text = f.read()

    for i in range(0, variants+1):
        print(text.format(i))
