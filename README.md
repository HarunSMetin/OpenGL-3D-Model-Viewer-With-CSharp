
# OpenGL 3D Model Viewer with C#

This is a simple 3D model viewer built with OpenGL and C#. It allows you to load 3D models in OBJ format and view them in a 3D environment. You can rotate, zoom in/out, and pan around the model to inspect it from different angles.

## Requirements
To run this program, you need to have the following:

- Windows operating system
- .NET Framework 4.0 or later
- OpenGL 3.3 or later
- Visual Studio (recommended version: Visual Studio 2019)

## Installation
To install this program, you can simply clone this repository or download the source code as a ZIP file. Once you have the source code, open the solution file (.sln) in Visual Studio and build the project. This should create an executable file (OBJ_Import.exe) in the bin/debug folder.

## Usage
#### Your "{model}.obj" and "{model}_diffuse.png" file must be stated in same directory. 

To use this program, simply double-click on the "OBJ_Import.exe" file. This should open up the 3D model viewer window. To load a 3D model, click on the File menu and select Open. Navigate to the folder where your OBJ file is located and select it. The program should load the model and display it in the 3D environment. <br/> You can also use the arrow keys on your keyboard to rotate the model.

## Credits
This program was created by Harun Metin as a personal project. It uses the following libraries:

- OpenTK (for OpenGL bindings)
- AssimpNet (for loading 3D models)
- ImGui.NET (for UI elements)

## License
This program is licensed under the MIT License. See the LICENSE file for more information.
