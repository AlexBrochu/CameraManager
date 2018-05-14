# Camera Manager
This project is a proof of concept that documentation can be used dynamically. If I had more time I could have added the posibility to configure the PTZ and the metadata of a camera and also the possibility to modify an existing profile. 

## Features:
1. Connect to a camera remotely
2. Save login info
3. Delete login info
4. Create new profile
5. Delete profiles

# Setup Project
Before running the project:
You have to change the value of two properties ONVIFEXPath and VLCPath with your path to these resources. 

![alt text](https://github.com/AlexBrochu/CameraManager/blob/master/screenshots/setup_project.png)

# How to Connect to a Camera 

The user has to provide an address, a username and the password to the camera he wants to connect to. 
These informations can be saved by providing a name and can be loaded for the next time. 

![alt text](https://github.com/AlexBrochu/CameraManager/blob/master/screenshots/Connect.png)

# How to configure a Profile

Currently the project allows the user to configure the video source, the video encoder, the audio source and the audio encoder of a camera. 

![alt text](https://github.com/AlexBrochu/CameraManager/blob/master/screenshots/dashboard.png)

To create a valid profile the user has to configure at least the video source and encoder.
A configuration has to be selected and can be modify with the configuration options available.
To show the available options the user has to click on the Show Options. 

The configuration is in a JSON format and can be modify by editing it. 

![alt text](https://github.com/AlexBrochu/CameraManager/blob/master/screenshots/change_video_source.png)

When an element of the media Profile is configure the button turns green and the next button corresponding to the next element to configure turns blue. 

![alt text](https://github.com/AlexBrochu/CameraManager/blob/master/screenshots/configureProfile.png)

# TODO
* Add modify profile options
* Add more validations
* Make edit config more user friendly
