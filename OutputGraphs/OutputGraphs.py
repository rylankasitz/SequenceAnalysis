import json
import sys, argparse
import os
import webbrowser

from plotly.offline import plot
import plotly.graph_objs as go
import plotly.figure_factory as ff

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
OutputFolder = r"C:\Users\rylan kasitz\Documents\SequenceAnalysisProgram\Output\Test"   # temp

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
size = len(Sequences)*50
if size > 800:
    size = 800
elif size < 600:
    size = 600
for analysisName, data in PercentIdentityData.items():
    #s = HelperMethods.removeVaccines(data["Sequences"])
    p = ff.create_annotated_heatmap(z=data["Data"], y=data["Sequences"], x=data["Sequences"], colorscale=Heatmap_Color, zmin=Heatmap_MinVal, 
                                 zmax=100, hoverinfo = "none", xaxis=dict(side='bottom'))
    p.layout.update(autosize=True, width=size, height=size, margin=go.Margin(l=250,r=100,b=200,t=50,pad=4))
    plot(p, filename=OutputFolder + "/PercentIdentity_Heatmaps/" + analysisName + ".html", auto_open=False, config={'showLink': False})

# Phylogetetic Trees
for name, value in Trees.items():
    Graphs.CreatePhyloGeneticTree(value["NewickFile"], OutputFolder + "/PhyloGeneticTrees/" + name + "_tree.png", Tree_Image_Size)

## Report Items ##

# Orf Bar Plots
''' orfData = []
    layout = None
    #orfAnnotations = {}
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
            fig_vaccines.append(go.Figure(data=data, layout=layout))'''

# Orf Vaccine Graphs
fig_vaccines_n = []
fig_vaccines_a = []
vac_height = 0
for sequence in Sequences.keys():
    if Sequences[sequence]["Vaccine"]:
        fig_a, fig_n, vac_height = Graphs.CreateOrfPlot(PercentIdentityData, sequence, Sequences, Heatmap_Color, Heatmap_MinVal, AnalysisNames,
                                                 title = sequence)
        fig_vaccines_a.append(fig_a)
        fig_vaccines_n.append(fig_n)

# Orf Plot
orfData_n = []
orfData_a = []
layouts_n = []
layouts_a = []
layout_n = None
layout_a = None
rangeD_n = {}
prevousRange = 0
seqs = []
vis = True
orf_height = 0
for sequence in Sequences.keys():
    if not Sequences[sequence]["Vaccine"]:
        fig_a, fig_n, orf_height = Graphs.CreateOrfPlot(PercentIdentityData, sequence, Sequences, Heatmap_Color, Heatmap_MinVal, 
                                                        AnalysisNames, title = "")
        orfData_n += (fig_n['data'])
        orfData_a += (fig_a['data'])
        layouts_n.append(fig_n['layout'])
        layouts_a.append(fig_a['layout'])
        seqs.append(sequence)
        layout_n = fig_n['layout']
        layout_a = fig_a['layout']
dropdown_n = Graphs.CreateNewDropDown(seqs, layouts_n)
layout_n['updatemenus'] = dropdown_n
layout_n['height'] = orf_height = len(seqs)*75 + 75
layout_n['title'] = "Nucleotide Comparison"
layout_n['margin']['t'] = 200
fig_orfDropdown_n = go.Figure(data=orfData_n, layout=layout_n)
dropdown_a = Graphs.CreateNewDropDown(seqs, layouts_a)
layout_a['updatemenus'] = dropdown_a
layout_a['height'] = orf_height = len(seqs)*75 + 75
layout_a['title'] = "Amino Acid Comparison"
layout_a['margin']['t'] = 200
fig_orfDropdown_a = go.Figure(data=orfData_a, layout=layout_a)


# Recombination Graph
fig_recombination = None
if(len(RecombinationData) > 0):
    fig_recombination = Graphs.CreateRecombinationGraph(RecombinationData, Recombination_Colors, Sequences, title="Recombination Sites")

# Heatmaps
s = HelperMethods.removeVaccines(PercentIdentityData["Wholegenome"]["Sequences"])
fig_Heatmap_Wholegenome = ff.create_annotated_heatmap(z=PercentIdentityData["Wholegenome"]["Data"], y=s, 
                          x=s, colorscale=Heatmap_Color, zmin=Heatmap_MinVal, zmax=100, hoverinfo = "none")
fig_Heatmap_Wholegenome.layout.update(title="Whole Genome Nucleotide Heatmap", width=size, height=size, 
                                    margin=go.Margin(l=200,r=75,b=200,t=50,pad=4), xaxis=dict(side='bottom'))
try:
    s = HelperMethods.removeVaccines(PercentIdentityData["Orf2b-Orf5a_aa"]["Sequences"])
    fig_Heatmap_orf2b5a = ff.create_annotated_heatmap(z=PercentIdentityData["Orf2b-Orf5a_aa"]["Data"], y=s, 
                          x=s, colorscale=Heatmap_Color, zmin=Heatmap_MinVal, zmax=100, hoverinfo = "none") 
    fig_Heatmap_orf2b5a.layout.update(title="Orf2b through Orf5a Amino Acid Heatmap", width=size, height=size, 
                                        margin=go.Margin(l=200,r=75,b=200,t=50,pad=4), xaxis=dict(side='bottom'))
except:
    fig_Heatmap_orf2b5a = None
    

# Create Plots
html_orfDropdown_n = plot(fig_orfDropdown_n, filename=OutputFolder + "/ReportParts/orfgraph.html", auto_open=False, config={'showLink': False})
html_orfDropdown_a = plot(fig_orfDropdown_a, filename=OutputFolder + "/ReportParts/orfgraph.html", auto_open=False, config={'showLink': False})
html_vaccinePlots_n = []
html_vaccinePlots_a = []
for i,f in enumerate(fig_vaccines_n):
    html_vaccinePlots_n.append(plot(f, filename=OutputFolder + "/ReportParts/vaccine_n" + str(i+1) + ".html", auto_open=False, config={'showLink': False}))
for i,f in enumerate(fig_vaccines_a):
    html_vaccinePlots_a.append(plot(f, filename=OutputFolder + "/ReportParts/vaccine_a" + str(i+1) + ".html", auto_open=False, config={'showLink': False}))
html_heatmap_wholegenome = plot(fig_Heatmap_Wholegenome, filename=OutputFolder + "/ReportParts/heatmap_wholegenome.html", auto_open=False, config={'showLink': False})
html_heatmap_orf2b5a = plot(fig_Heatmap_orf2b5a, filename=OutputFolder + "/ReportParts/heatmap_orf2b5a.html" , auto_open=False, config={'showLink': False})
html_recombination = plot(fig_recombination, filename=OutputFolder + "/ReportParts/recombination.html" , auto_open=False, config={'showLink': False})


# Add to html
recomb_height = len(RecombinationData)*125
if recomb_height < 300:
    recomb_height = 300

html_string = Graphs.InitalizeHtmlString()
for str in html_vaccinePlots_n:
    html_string += Graphs.CreateHtmlPlotString(str, width='33%', height=vac_height, min_width=500)
for str in html_vaccinePlots_a:
    html_string += Graphs.CreateHtmlPlotString(str, width='33%', height=vac_height, min_width=500)
html_string += Graphs.CreateHtmlPlotString(html_orfDropdown_n, width='50%', height=orf_height)
html_string += Graphs.CreateHtmlPlotString(html_orfDropdown_a, width='50%', height=orf_height)
html_string += Graphs.CreateHtmlPlotString(html_heatmap_wholegenome, width='50%', padding_top='50%', min_width=size, height=size)
html_string += Graphs.CreateHtmlPlotString(html_heatmap_orf2b5a, width='50%', padding_top='50%', min_width=size, height=size)
html_string += Graphs.CreateHtmlPlotString(html_recombination, width=800, height=recomb_height, min_width=800)
html_string += Graphs.CreateImageHtmlString(OutputFolder + "\PhyloGeneticTrees\Wholegenome_tree.png", width=600, height='auto', 
                                            title='Whole Genome Phylogenetic Tree', min_width=600)
html_string += Graphs.EndHtmlString()

f = open(OutputFolder + "/Report.html",'w')
f.write(html_string)
f.close()

webbrowser.open('file://' + os.path.realpath(OutputFolder + "/Report.html"))

# Mobile Report
'''html_string = Graphs.InitalizeHtmlString()
#html_string += Graphs.CreateHtmlPlotString(html_orfDropdown, width='100%', height=bar_height)
    for str in html_vaccinePlots:
        html_string += Graphs.CreateHtmlPlotString(str, width='100%', height=bar_height)
    html_string += Graphs.CreateHtmlPlotString(html_heatmap_wholegenome, width='100%', height=size, min_width=size)
    html_string += Graphs.CreateHtmlPlotString(html_heatmap_orf2b5a, width='100%', height=size, min_width=size)
    html_string += Graphs.CreateHtmlPlotString(html_recombination, width='100%', height=recomb_height, min_width=200)
    html_string += Graphs.CreateImageHtmlString(OutputFolder + "\PhyloGeneticTrees\Wholegenome_tree.png", width='100%', height='auto', 
                                                title='Whole Genome Phylogenetic Tree', min_width=200)
    html_string += Graphs.EndHtmlString()

    f = open(OutputFolder + "/Report_Mobile.html",'w')
    f.write(html_string)
    f.close()'''
        
  