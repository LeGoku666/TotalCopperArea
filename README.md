# TotalCopperArea
Calculate total exposed copper area on a PCB from bitmap exported in cadsoft eagle.

A simple little project. The program counts the white pixels and calculates the area based on the calculated DPI formula.
In Cadsoft EAGLE you leave only the layers whose area you want to calculate turned on, they must be white, the background black. You export the monochrome bitmap in eagle. 
Open the finished bitmap in the program and press START.

The program can calculate a pair of images in parallel asynchronously without blocking the GUI.
