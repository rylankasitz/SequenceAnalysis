import plotly.graph_objs as go
import HelperMethods
from collections import OrderedDict
from ete3 import Tree, TreeStyle
import base64

def StackedSequenceGraph(inputData, sequencename, nameOrder, Sequences, labelsize=11, title="", visible=True):
    data = []
    annotations = []  
    sequences = []
    annotationSeqs = {}
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
                                    name=name, orientation = 'h', hoverinfo='name+text', visible=visible))
                for s in sequences[:]:
                    annotationSeqs[s] = s

    # Annotations  
    for sequence in annotationSeqs.keys():
        total = 0
        dist = 0
        for orfname in orfs:
            if sequence in inputData[orfname]["DicInverse"][sequencename]:
                total += inputData[orfname]["Dic"][sequencename][sequence]
                dist += inputData[orfname]["DicInverse"][sequencename][sequence]
        average = round(total/len(orfs), 3)
        annotations.append(dict(x=dist + 35, y=sequence, text=average, font=dict(size=labelsize), showarrow=False, visible=visible))

    height = len(Sequences)*30
    if height < 300:
        height = 300
    return data, go.Layout(title=title, showlegend=False, height=height, barmode='stack', annotations=annotations, 
                           xaxis=dict(showgrid=False, showticklabels=False), margin=go.Margin(l=200,r=10,b=100,t=125,pad=4),
                           yaxis=dict(tickangle=0)), annotations

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
        for i,x in enumerate(annotations[d]):
            annotations[d][i]['visible'] = True;
        buttons.append(dict(args=[{'visible': visible}, {'annotations': annotations[d]}], label=d, method='update'))
        visible = [False]*datalen
    return [dict(active=0, showactive=False, buttons=buttons, direction = 'down', pad = {'r': 10, 't': 0}, x = 0, 
                 xanchor = 'left', y = 1.3, yanchor = 'middle')]

def CreateRecombinationGraph(inputData, colors, sequencesdata, title=""):
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
            if not ("bp" + str(i)) in bpsdata:
                bpsdata["bp" + str(i)] = {}
            if not ("nonbp" + str(i)) in bpsPoints:
                bpsPoints["nonbp" + str(i)] = {}
            if not ("bp" + str(i)) in bpsPoints:
                bpsPoints["bp" + str(i)] = {}
            bpsdata["nonbp" + str(i)][seq] = bp["StartSite"] - lastSite
            bpsdata["bp" + str(i)][seq] = bp["EndSite"] - bp["StartSite"]
            bpsPoints["nonbp" + str(i)][seq] = str(bp["StartSite"]) + " " + HelperMethods.findOrfFromSeq(sequencesdata, seq, int(bp["StartSite"]))
            bpsPoints["bp" + str(i)][seq] = str(bp["EndSite"]) + " " + HelperMethods.findOrfFromSeq(sequencesdata, seq, int(bp["EndSite"]))
            lastSite = bp["EndSite"] 
            lastIndex = i + 1
        if not "nonbp" + str(lastIndex) in bpsdata:
            bpsdata["nonbp" + str(lastIndex)] = {}
            bpsPoints["nonbp" + str(lastIndex)] = {}
        bpsdata["nonbp" + str(lastIndex)][seq] = bp["SequenceLength"] - lastSite
        bpsPoints["nonbp" + str(lastIndex)][seq] = bp["SequenceLength"]

    # Plot data
    height = 125*len(inputData)
    if height < 300:
        height = 300
    for i, key in enumerate(bpsdata.keys()):
        data.append(go.Bar(y=inputData.keys(), x=HelperMethods.match(inputData.keys(), bpsdata[key], full=True), name=labels[(i)%2], orientation = 'h',
                    text=HelperMethods.match(inputData.keys(), bpsPoints[key], full=True), marker=dict(color=colors[(i)%2]), 
                    hoverinfo='text')) 
    return go.Figure(data=data, layout=go.Layout(title=title, barmode='stack', height=height, showlegend=False, margin=go.Margin(l=200,r=100,b=100,t=100,pad=4)))

def CreatePhyloGeneticTree(inputfile, outputfile, size):  
    f = open(inputfile, "r")
    data = f.readlines()[0]
    f.close()
    tree = Tree(data)
    tree.set_outgroup(tree.get_midpoint_outgroup())
    ts = TreeStyle()
    ts.show_leaf_name = True
    ts.show_branch_length = False
    ts.show_branch_support = False
    ts.optimal_scale_level = "mid"
    t = tree.render(str(outputfile), w=size, units="px", tree_style=None)


# Html Output
def addRow(height):
    return '''<div class="row" stylesheet="height: ''' + height + ''';">'''

def endRow():
    return '''</div>'''

def addCol(width):
    return '''<div class="col" stylesheet="width: ''' + width + ''';">'''

def endCol():
    return '''</div>'''

def CreateHtmlPlotString(src, title="", width=0, height=0, padding_top=0, min_width=380):    
    src = src.replace(r"file://", "")
    content = open(src, 'r').read().replace("'", '"')
    content = "'" + content[:12] + '<script src="https://cdn.plot.ly/plotly-latest.min.js"></script>' + content[12:] + "'"
    string = '''<iframe width="''' + str(width) + '''" height="''' + str(height) + '''" padding-top="''' + str(padding_top) + '''" 
                          frameborder="0" seamless="seamless" scrolling="no" srcdoc=''' + content + ''' 
                          style="min-width:''' + str(min_width) +''';"></iframe>'''
    return string

def CreateImageHtmlString(location, width='auto', height='auto', title="", min_width=500):
    encoded = base64.b64encode(open(location, "rb").read())
    return '''<div width="''' + str(width) + '''" height="''' + str(height) + '''"
                style="min-width:''' + str(min_width) +''';">
                <center style="float:left;"><h4>''' + title + '''</h4></center>
                <img src="data:image/png;base64, ''' + encoded + '''"/></div>'''

def InitalizeHtmlString():
    return '''<html><head><link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.1/css/bootstrap.min.css">
                          <style>body{ margin:0 50; } iframe{ float: left;  overflow:hidden; } img{ float: left; padding: 75 }</style></head><body>'''

def EndHtmlString():
    return  '''</body></html>'''