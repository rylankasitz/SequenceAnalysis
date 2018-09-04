import json
import sys
import os

from  plotly import  tools
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
Sequences = HelperMethods.readJson(DataFolder + "/Sequences.json")
PercentIdentityData = HelperMethods.readJson(DataFolder + "/PercentIdentities.json")
RecombinationData = HelperMethods.readJson(DataFolder + "/Recombination.json")
AnalysisNames = HelperMethods.readJson(DataFolder + "/AnalysisNames.json")
Trees = HelperMethods.readJson(DataFolder + "/Trees.json")

# Create output folders
HelperMethods.createDir(OutputFolder)
HelperMethods.createDir(OutputFolder + "/PercentIdentity_Heatmaps/")
HelperMethods.createDir(OutputFolder + "/PhyloGeneticTrees/")

## Non Report Items ##

# Heatmaps
for analysisName, data in PercentIdentityData.items():
    data = [go.Heatmap(z=data["Data"], y=data["Sequences"], x=data["Sequences"], colorscale=Heatmap_Color, zmin=Heatmap_MinVal, zmax=100)]
    plot(data, filename=OutputFolder + "/PercentIdentity_Heatmaps/" + analysisName + ".html", auto_open=False)

# Phylogetetic Trees
for name, value in Trees.items():
    Graphs.CreatePhyloGeneticTree(value["NewickFile"], OutputFolder + "/PhyloGeneticTrees/" + name + "_tree.png", Tree_Image_Size)

## Report Items ##
fig_report = tools.make_subplots(rows=4, cols=4, shared_xaxes=False, 
                                 specs=[[{}, {}, {}, {}],
                                        [{'rowspan': 2, 'colspan': 2}, None, {'rowspan': 2, 'colspan': 2}, None],
                                        [None, None, None, None],
                                        [{'colspan': 2}, None, {'colspan': 2}, None]])

# Orf Bar Plots
orfData = []
layout = None
orfAnnotations = {}
rangeD = {}
prevousRange = 0
seqs = []
for sequence in Sequences.keys():
    if not Sequences[sequence]["Vaccine"]:
        data, layout, annotations = Graphs.StackedSequenceGraph(PercentIdentityData, sequence, AnalysisNames, Sequences)
        orfData += data
        orfAnnotations[sequence] = annotations
        rangeD[sequence] = range(prevousRange, prevousRange + len(data))
        prevousRange += len(data)
        seqs.append(sequence)
dropdown = Graphs.CreateDropDown(seqs, len(orfData), rangeD, orfAnnotations)
layout['updatemenus'] = dropdown
fig_orfDropdown = go.Figure(data=orfData, layout=layout)

# Vaccine Orf Bar Plots
fig_vaccines = []
for sequence in Sequences.keys():
    if Sequences[sequence]["Vaccine"]:
        data, layout, annotations = Graphs.StackedSequenceGraph(PercentIdentityData, sequence, AnalysisNames, Sequences)
        layout['annotations'] = annotations
        fig_vaccines.append(go.Figure(data=data, layout=layout))


# Recombination Graph
fig_recombination = None
if(len(RecombinationData) > 0):
    fig_recombination = Graphs.CreateRecombinationGraph(RecombinationData, Recombination_Colors)

# Heatmaps
fig_Heatmap_Wholegenome = go.Heatmap(z=PercentIdentityData["Wholegenome"]["Data"], y=PercentIdentityData["Wholegenome"]["Sequences"], 
                          x=PercentIdentityData["Wholegenome"]["Sequences"], colorscale=Heatmap_Color, zmin=Heatmap_MinVal, zmax=100)
try:
    fig_Heatmap_orf2b5a = go.Heatmap(z=PercentIdentityData["Orf2b-Orf5a_aa"]["Data"], y=PercentIdentityData["Orf2b-Orf5a_aa"]["Sequences"], 
                          x=PercentIdentityData["Orf2b-Orf5a_aa"]["Sequences"], colorscale=Heatmap_Color, zmin=Heatmap_MinVal, zmax=100)
except:
    fig_Heatmap_orf2b5a = None
    

# Add Figures
HelperMethods.addFig(fig_report, fig_orfDropdown, 1, 1)
for i,vfig in enumerate(fig_vaccines):
    HelperMethods.addFig(fig_report, vfig, 1, i + 2)
if fig_recombination != None:
    HelperMethods.addFig(fig_report, fig_recombination, 4, 1)
if fig_Heatmap_Wholegenome != None:
    fig_report.append_trace(fig_Heatmap_Wholegenome, 2, 1)
if fig_Heatmap_orf2b5a != None:
    fig_report.append_trace(fig_Heatmap_orf2b5a, 2, 3)

# Plot
fig_report['layout']['showlegend'] = False;
fig_report['layout']['margin'] = {'r': 200, 'l': 20, 'b': 20, 't': 20}
fig_report['layout']['images'] = [dict(source=OutputFolder + "/PhyloGeneticTrees/Wholgenome_tree.png",
                                        xref= "paper",
                                        yref= "paper",
                                        x= 0,
                                        y= 3,
                                        sizex= 2,
                                        sizey= 2,
                                        xanchor="right", yanchor="bottom")]
plot(fig_report, filename=OutputFolder + "/Report.html", auto_open=True)