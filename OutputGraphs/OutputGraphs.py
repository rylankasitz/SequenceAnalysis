import json
import sys
import os

from plotly.offline import plot
import plotly.graph_objs as go

import HelperMethods
import Graphs

# OutputData Settings
Heatmap_Color = [[0.0, 'rgb(255,23,68)'], [0.5, 'rgb(255,234,0)'], [1.0, 'rgb(0,230,118)']]
Heatmap_MinVal = 80
Recombination_Colors = ['rgb(0,176,255)', 'rgb(255,23,68)']
Tree_Image_Size = 720

# Intialize variables
DataFolder = ""
OutputFolder = ""

# Read command line args
if len(sys.argv) >= 3:
    DataFolder = sys.argv[1]
    OutputFolder = sys.argv[2]
else:
    DataFolder = r"C:\Users\rylan kasitz\source\repos\PRRSAnalysis\PRRSAnalysis\bin\Debug\_TempData"
    OutputFolder = r"C:\Users\rylan kasitz\Documents\TestOutput"

# Get data variables
PercentIdentityData = HelperMethods.readJson(DataFolder + "/PercentIdentities.json")
RecombinationData = HelperMethods.readJson(DataFolder + "/Recombination.json")
AnalysisNames = HelperMethods.readJson(DataFolder + "/AnalysisNames.json")
Trees = HelperMethods.readJson(DataFolder + "/Trees.json")
Sequences = RecombinationData.keys()

# Create output folders
HelperMethods.createDir(OutputFolder)
HelperMethods.createDir(OutputFolder + "/PercentIdentity_Heatmaps/")
HelperMethods.createDir(OutputFolder + "/PhyloGeneticTrees/")

## Report Items ##

# Orf Bar Plots
orfData = []
layout = None
orfAnnotations = {}
rangeD = {}
prevousRange = 0
for sequence in Sequences:
    data, layout, annotations = Graphs.StackedSequenceGraph(PercentIdentityData, sequence, AnalysisNames)
    orfData += data
    orfAnnotations[sequence] = annotations
    rangeD[sequence] = range(prevousRange, prevousRange + len(data))
    prevousRange += len(data)
dropdown = Graphs.CreateDropDown(Sequences, len(orfData), rangeD, orfAnnotations)
layout['updatemenus'] = dropdown
fig = dict(data=orfData, layout=layout)
plot(fig, filename=OutputFolder + "/Report.html", auto_open=False)

# Vaccine Orf Bar Plots

# Recombination Graph
fig = Graphs.CreateRecombinationGraph(RecombinationData, Recombination_Colors)
plot(fig, filename=OutputFolder + "/Report.html", auto_open=False)

## Non Report Items ##

# Heatmaps
for analysisName, data in PercentIdentityData.items():
    data = [go.Heatmap(z=data["Data"], y=data["Sequences"], x=data["Sequences"], colorscale=Heatmap_Color, zmin=Heatmap_MinVal, zmax=100)]
    plot(data, filename=OutputFolder + "/PercentIdentity_Heatmaps/" + analysisName + ".html", auto_open=False)

# Phylogetetic Trees
for name, value in Trees.items():
    Graphs.CreatePhyloGeneticTree(value["NewickFile"], OutputFolder + "/PhyloGeneticTrees/" + name + "_tree.png", Tree_Image_Size)