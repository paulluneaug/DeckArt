import matplotlib.pyplot as plt
import numpy as np


metricsPath = "DeckArt_Unity\Assets\Resources\Metrics.txt"

costHeader = "Average Cost"
attackHeader = "Average Attack"
defenseHeader = "Average Defense"
gameDurationHeader = "Average Game Duration"
winRateHeader = "WinRate"
bestWinRateHeader = "Best WinRate"

iterationIndex = [i for i in range(1, 501)]

dataSets = {
    costHeader : [],
    attackHeader : [],
    defenseHeader : [],
    gameDurationHeader : [],
    winRateHeader : [],
    bestWinRateHeader : [],
}

currentDataSet = ""

def IsNumber(s):
    try:
        result = float(s)
        return True, result
    except ValueError:
        return False, 0


def ProcessLine(line : str):
    global currentDataSet
    if not line:
        return
    
    isNumber, parsedValue = IsNumber(line)
    if isNumber:
        dataSets[currentDataSet].append(parsedValue)
        return
        
    if line in dataSets.keys():
        currentDataSet = line
        return
        
    print(f"Invalid Line ({line})")
    

f = open(metricsPath, "r")
lines = f.readlines()

for line in lines:
    ProcessLine(line[:-1])
    


    
fig, cardsDataPlot = plt.subplots()
cardsDataPlot.plot(iterationIndex, dataSets[costHeader], color='yellow')  
cardsDataPlot.plot(iterationIndex, dataSets[attackHeader], color='red')  
cardsDataPlot.plot(iterationIndex, dataSets[defenseHeader], color='blue')  


fig, gameDuration = plt.subplots()
# gameDuration.plot(iterationIndex, dataSets[gameDurationHeader], color='black') 
plt.show() 