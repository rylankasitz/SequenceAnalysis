import os
import shutil

os.system("pyinstaller --onefile --clean build.spec")
shutil.move(r"./dist\BuildGraphs.exe", r"C:\Users\rylan kasitz\source\repos\PRRSAnalysis\PRRSAnalysis\bin\Debug\_ApplicationFiles\BuildGraphs.exe")
