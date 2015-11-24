set terminal png transparent nocrop enhanced size 1050,920 font "arial,10" 
set output 'CrossMutationMap.png'

set border linewidth 0
#unset key
#unset colorbox
#unset tics
set cbtics scale 0

set lmargin screen 0.1
set rmargin screen 0.8
set tmargin screen 0.9
set bmargin screen 0.1

set title "Crossover / Mutation Heatmap" 
#set xrange [ 0.00000 : 1.00000 ] noreverse nowriteback
#set yrange [ 0.00000 : 1.00000 ] noreverse nowriteback
set xrange [1 : 100]
set yrange [1 : 100]
set xlabel "Mutation"
set ylabel "Crossover"

set cblabel "Fitness" 
#set cbrange [ 0.00000 : 1.00000 ] noreverse nowriteback
#set palette rgbformulae -7, 2, -7

set palette maxcolors 8
set palette defined ( 0 '#FFFFCC',\
    	    	      1 '#FFEDA0',\
		      2 '#FED976',\
		      3 '#FEB24C',\
		      4 '#FD8D3C',\
		      5 '#FC4E2A',\
		      6 '#E31A1C',\
		      7 '#00FF00' )
#B10026

# x = 0.0
# set datafile separator comma
set pm3d map
set pm3d interpolate 0,0
#set pm3d interpolate 1,1
splot 'output.txt' matrix with image
##plot 'heatmap.csv' matrix columnheaders using 1:2:3 with image