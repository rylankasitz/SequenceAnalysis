import json
import os

from  plotly import  tools
from plotly.offline import plot
import plotly.graph_objs as go

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

def addFig(report, fig, r, c):
    for plot in fig['data']:
        report.append_trace(plot, r, c)
    report['layout'].update(fig['layout'])
