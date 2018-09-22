import json
import sys, argparse
import os
import webbrowser

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
parser=argparse.ArgumentParser()
parser.add_argument('--i')
parser.add_argument('--out')
args=parser.parse_args()
DataFolder = args.i
OutputFolder = args.out

DataFolder = r"C:\Users\rylan kasitz\source\repos\SequenceAnalysis\PRRSAnalysis\bin\Debug\_TempData" # temp
OutputFolder = r"C:\Users\rylan kasitz\Documents\SequenceAnalysisProgram\Output\ Test"   # temp

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
HelperMethods.createDir(OutputFolder + "/ReportParts/")

## Non Report Items ##

# Heatmaps
for analysisName, data in PercentIdentityData.items():
    data = dict(data=[go.Heatmap(z=data["Data"], y=data["Sequences"], x=data["Sequences"], colorscale=Heatmap_Color, zmin=Heatmap_MinVal, 
                                 zmax=100)], layout=dict(autosize=True, width=len(Sequences)*75, height=len(Sequences)*75, margin=go.Margin(l=200,r=10,b=200,t=50,pad=4)))
    plot(data, filename=OutputFolder + "/PercentIdentity_Heatmaps/" + analysisName + ".html", auto_open=False, config={'showLink': False})

# Phylogetetic Trees
for name, value in Trees.items():
    Graphs.CreatePhyloGeneticTree(value["NewickFile"], OutputFolder + "/PhyloGeneticTrees/" + name + "_tree.png", Tree_Image_Size)

## Report Items ##

# Orf Bar Plots
orfData = []
layout = None
orfAnnotations = {}
rangeD = {}
prevousRange = 0
seqs = []
vis = True
for sequence in Sequences.keys():
    if not Sequences[sequence]["Vaccine"]:
        data, layout, annotations = Graphs.StackedSequenceGraph(PercentIdentityData, sequence, AnalysisNames, Sequences, 
                                                                title="Amino Acid Percent Identity Comparisons", visible=vis)
        if vis:
            vis = False
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
        data, layout, annotations = Graphs.StackedSequenceGraph(PercentIdentityData, sequence, AnalysisNames, Sequences,
                                                                title=(sequence + " Comparison"))
        layout['annotations'] = annotations
        fig_vaccines.append(go.Figure(data=data, layout=layout))


# Recombination Graph
fig_recombination = None
if(len(RecombinationData) > 0):
    fig_recombination = Graphs.CreateRecombinationGraph(RecombinationData, Recombination_Colors, Sequences, title="Recombination Sites")

# Heatmaps
size = len(Sequences)*75
if size > 800:
    size = 800
elif size < 600:
    size = 600
fig_Heatmap_Wholegenome = dict(data=[go.Heatmap(z=PercentIdentityData["Wholegenome"]["Data"], y=PercentIdentityData["Wholegenome"]["Sequences"], 
                          x=PercentIdentityData["Wholegenome"]["Sequences"], colorscale=Heatmap_Color, zmin=Heatmap_MinVal, zmax=100)],
                          layout=dict(title="Whole Genome Nucleotide Heatmap", width=size, height=size, 
                                      margin=go.Margin(l=200,r=10,b=200,t=50,pad=4)))
try:
    fig_Heatmap_orf2b5a = dict(data=[go.Heatmap(z=PercentIdentityData["Orf2b-Orf5a_aa"]["Data"], y=PercentIdentityData["Orf2b-Orf5a_aa"]["Sequences"], 
                          x=PercentIdentityData["Orf2b-Orf5a_aa"]["Sequences"], colorscale=Heatmap_Color, zmin=Heatmap_MinVal, zmax=100)], 
                          layout = dict(title="Orf2b through Orf5a Amino Acid Heatmap", width=size, height=size, 
                                        margin=go.Margin(l=200,r=10,b=200,t=50,pad=4)))
except:
    fig_Heatmap_orf2b5a = None
    

# Create Plots
html_orfDropdown = plot(fig_orfDropdown, filename=OutputFolder + "/ReportParts/orfgraph.html", auto_open=False, config={'showLink': False})
html_vaccinePlots = []
for i,f in enumerate(fig_vaccines):
    html_vaccinePlots.append(plot(f, filename=OutputFolder + "/ReportParts/vaccine" + str(i+1) + ".html", auto_open=False, config={'showLink': False}))
html_heatmap_wholegenome = plot(fig_Heatmap_Wholegenome, filename=OutputFolder + "/ReportParts/heatmap_wholegenome.html", auto_open=False, config={'showLink': False})
html_heatmap_orf2b5a = plot(fig_Heatmap_orf2b5a, filename=OutputFolder + "/ReportParts/heatmap_orf2b5a.html" , auto_open=False, config={'showLink': False})
html_recombination = plot(fig_recombination, filename=OutputFolder + "/ReportParts/recombination.html" , auto_open=False, config={'showLink': False})


# Add to html
bar_height = len(Sequences)*30
if bar_height < 250:
    bar_height = 250
recomb_height = len(RecombinationData)*125
if recomb_height < 300:
    recomb_height = 300

html_string = Graphs.InitalizeHtmlString()
html_string += Graphs.CreateHtmlPlotString(html_orfDropdown, width='25%', height=bar_height)
for str in html_vaccinePlots:
    html_string += Graphs.CreateHtmlPlotString(str, width='25%', height=bar_height)
html_string += Graphs.CreateHtmlPlotString(html_heatmap_wholegenome, width='50%')
html_string += Graphs.CreateHtmlPlotString(html_heatmap_orf2b5a, width='50%')
html_string += Graphs.CreateHtmlPlotString(html_recombination, width='50%', height=recomb_height)
html_string += Graphs.CreateImageHtmlString(OutputFolder + "\PhyloGeneticTrees\Wholegenome_tree.png", width='50%', height='auto', 
                                            title='Whole Genome Phylogenetic Tree')
html_string += Graphs.EndHtmlString()

f = open(OutputFolder + "/Report.html",'w')
f.write(html_string)
f.close()

webbrowser.open('file://' + os.path.realpath(OutputFolder + "/Report.html"))
        
  