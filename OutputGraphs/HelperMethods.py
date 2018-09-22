import json
import os

def readJson(file):
    with open(file, 'r') as content_file:
        return json.loads(content_file.read())
    return null

def createDir(directory):
    if not os.path.exists(directory):
        os.makedirs(directory)

def match(list, dictionary, full=False):
    values = []
    for item in list:
        if item in dictionary:
            values.append(dictionary[item])
        elif full:
            values.append(0)
    return values

def addFig(report, fig, r, c):
    for plot in fig['data']:
        report.append_trace(plot, r, c)
    report['layout'].update(fig['layout'])

def findOrfFromSeq(sequencedata, seqname, pos):
    for orf, value in sequencedata[seqname]['KnownOrfData'].items():
        if value['StartLocationN'] <= pos and value['EndLocationN'] >= pos:
            return orf
    return ""