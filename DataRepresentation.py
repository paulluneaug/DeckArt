import matplotlib.pyplot as plt
import numpy as np


metricsPath = "DeckArt_Unity\Assets\Resources\Metrics.txt"

costHeader = "Average Cost"
attackHeader = "Average Attack"
defenseHeader = "Average Defense"
gameDurationHeader = "Average Game Duration"
winRateHeader = "WinRate"
bestWinRateHeader = "Best WinRate"

gamesPerIterationHeader = "Games per iteration"
iterationCountHeader = "Iteration Count"

dataSets = {
    costHeader : [],
    attackHeader : [],
    defenseHeader : [],
    gameDurationHeader : [],
    winRateHeader : [],
    bestWinRateHeader : [],
}

variables = {
    gamesPerIterationHeader : 0,
    iterationCountHeader : 0,
}

writeInVariables = False
currentKey = ""

def IsNumber(s):
    try:
        result = float(s)
        return True, result
    except ValueError:
        return False, 0


def ProcessLine(line : str):
    global currentKey, writeInVariables
    if not line:
        return
    
    isNumber, parsedValue = IsNumber(line)
    if isNumber:
        if writeInVariables:
            variables[currentKey] = parsedValue
        else :
            dataSets[currentKey].append(parsedValue)
        return
        
    if line in dataSets.keys():
        writeInVariables = False
        currentKey = line
        return
    
    if line in variables.keys():
        writeInVariables = True
        currentKey = line
        return
        
    print(f"Invalid Line ({line})")
    

f = open(metricsPath, "r")
lines = f.readlines()

for line in lines:
    ProcessLine(line[:-1])
    


iterationIndex = [i for i in range(1, int(variables[iterationCountHeader]) + 1)]
    
fig, cardsDataPlot = plt.subplots()
cardsDataPlot.plot(iterationIndex, dataSets[costHeader], color='yellow')  
cardsDataPlot.plot(iterationIndex, dataSets[attackHeader], color='red')  
cardsDataPlot.plot(iterationIndex, dataSets[defenseHeader], color='blue')  


fig, gameDuration = plt.subplots()
gameDuration.plot(iterationIndex, dataSets[gameDurationHeader], color='black') 

fig, winRate = plt.subplots()
winRate.plot(iterationIndex, dataSets[winRateHeader], color='yellow')  
winRate.plot(iterationIndex, dataSets[bestWinRateHeader], color='red')  
plt.show() 