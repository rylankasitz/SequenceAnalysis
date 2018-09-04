import plotly.graph_objs as go
from plotly.tools import FigureFactory as FF
import HelperMethods
import numpy as np
from collections import OrderedDict
from PyQt5 import QtGui
from ete3 import Tree, TreeStyle

def StackedSequenceGraph(inputData, sequencename, nameOrder, Sequences, labelsize=11):
    data = []
    annotations = []  
    sequences = []
    orfs = []
    for orfname in nameOrder:
        if sequencename in inputData[orfname]["Dic"] and orfname != "Orf2b-Orf5a_aa": 
            sequences = inputData[orfname]["Sequences"][:]
            dataNormal = inputData[orfname]["Dic"][sequencename].copy()
            dataInverse = inputData[orfname]["DicInverse"][sequencename].copy()
                 
            sequences.remove(sequencename)
            del dataNormal[sequencename]
            del dataInverse[sequencename]
            s = sequences[:]
            for seq in s:      
                if Sequences[seq]["Vaccine"]:
                    sequences.remove(seq)
                    

            orfparts = orfname.split('_')
            type = orfparts[len(orfparts)-1]
            name = orfname.rsplit('_', 1)[0]
            if orfname != "Wholegenome" and type == "aa":     
                orfs.append(orfname)
                data.append(go.Bar(y=sequences, x=HelperMethods.match(sequences, dataInverse), 
                                    text=HelperMethods.match(sequences, dataNormal),
                                    name=name, orientation = 'h', hoverinfo='name+text'))  

    # Annotations  
    for sequence in sequences:
        total = 0
        dist = 0
        for orfname in orfs:
            if sequence in inputData[orfname]["DicInverse"][sequencename]:
                total += inputData[orfname]["Dic"][sequencename][sequence]
                dist += inputData[orfname]["DicInverse"][sequencename][sequence]
        average = round(total/len(orfs), 3)
        annotations.append(dict(x=dist + 70, y=sequence, text=average, font=dict(size=labelsize), showarrow=False))

    return data, go.Layout(barmode='stack', annotations=annotations), annotations

def CreateDropDown(datalist, datalen, rangeD, annotations):
    buttons = []
    dataArray = []
    visible = [False]*datalen
    inverval = datalen/len(datalist)
    for i,d in enumerate(datalist):
        dataArray += d
        for j in rangeD[d]:
            visible[j] = True
        visible += [True]*100000
        buttons.append(dict(args=[{'visible': visible}, {'annotations': annotations[d]}], label=d, method='update'))
        visible = [False]*datalen
    return [dict(buttons=buttons, direction = 'down', pad = {'r': 10, 't': 10}, showactive = True, x = 0.1, xanchor = 'left', y = 1.1, 
                 yanchor = 'top')]

def CreateRecombinationGraph(inputData, colors):
    bpsdata = OrderedDict()
    bpsPoints = OrderedDict()
    data = []
    labels = ["Non Recombinations Site", "RecombinationSite"]
    
    # Get data
    for seq, recombData in inputData.items():     
        lastSite = 0
        lastIndex = 0
        for i, bp in enumerate(recombData):
            if not ("nonbp" + str(i)) in bpsdata:
                bpsdata["nonbp" + str(i)] = {}
                bpsdata["bp" + str(i)] = {}
                bpsPoints["nonbp" + str(i)] = {}
                bpsPoints["bp" + str(i)] = {}
            bpsdata["nonbp" + str(i)][seq] = bp["StartSite"] - lastSite
            bpsdata["bp" + str(i)][seq] = bp["EndSite"] - bp["StartSite"]
            bpsPoints["nonbp" + str(i)][seq] = bp["StartSite"]
            bpsPoints["bp" + str(i)][seq] = bp["EndSite"]
            lastSite = bp["EndSite"] 
            lastIndex = i + 1
        if not "nonbp" + str(lastIndex) in bpsdata:
            bpsdata["nonbp" + str(lastIndex)] = {}
            bpsPoints["nonbp" + str(lastIndex)] = {}
        bpsdata["nonbp" + str(lastIndex)][seq] = bp["SequenceLength"] - lastSite
        bpsPoints["nonbp" + str(lastIndex)][seq] = bp["SequenceLength"]

    # Plot data
    for i, key in enumerate(bpsdata.keys()):
        data.append(go.Bar(y=inputData.keys(), x=HelperMethods.match(inputData.keys(), bpsdata[key]), name=labels[(i)%2], orientation = 'h',
                    text=HelperMethods.match(inputData.keys(), bpsPoints[key]), marker=dict(color=colors[(i)%2]), hoverinfo='name+text')) 
    return go.Figure(data=data, layout=go.Layout(barmode='stack', showlegend=False))

def CreatePhyloGeneticTree(inputfile, outputfile, size):
    f = open(inputfile, "r")
    try:
        data = f.readlines()[0]
        f.close()

        tree = Tree(data)
        tree.set_outgroup(tree.get_midpoint_outgroup())
        ts = TreeStyle()
        ts.show_leaf_name = True
        ts.show_branch_length = False
        ts.show_branch_support = False
        ts.optimal_scale_level = "mid"
        t = tree.render(str(outputfile), w=size, units="px", tree_style=ts)
    except:
        print "Can't read " + inputfile