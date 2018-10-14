import os
import shutil

os.system("pyinstaller --onefile --clean --hidden-import sip build.spec")
shutil.move(r"./dist\BuildGraphs.exe", r"C:\Users\rylan kasitz\source\repos\SequenceAnalysis\PRRSAnalysis\bin\Debug\_ApplicationFiles\BuildGraphs.exe")
