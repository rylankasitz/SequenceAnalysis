import json
import os

def readJson(file):
    with open(file, 'r') as content_file:
        return json.loads(content_file.read())
    return null

def createDir(directory):
    if not os.path.exists(directory):
        os.makedirs(directory)

def match(list, dictionary):
    values = []
    for item in list:
        if item in dictionary:
            values.append(dictionary[item])
    return values
