# Mediapipe-Unity3D-Package
##Integrated Mediapipe with Unity 3D

This project integrated Mediapipe v0.6.9 with Unity 3D

## Installation
This project contain Unity Package and a sample project.
To use Unity Package, create unity 3d project at v2019.3.12f1 or higher, import package and install AR foundation to your new project.
Enable unsafe code, uncheck Mutil Thread Rendering and remove Vulkan Graphic setting in your Player setting. 
Now, the package is ready to use

## Features

- Hand Detection and Tracking 3D

- Hand Gesture Recognition

## Usage
- To get 21 hand landmarks: Manager.instance.GetHandLandmarks() 
- To use hand gesture recognition: access Gesture singleton to use 4 gesture recognition 