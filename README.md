<img src="https://raw.githubusercontent.com/JensKrumsieck/PowerPoint-Remote/master/PowerPoint%20Remote/Resources/PPTRemote.png" alt="PPTRemote" width="250px"/>

# PowerPoint-Remote

A simple WPF/Web based Remote for Powerpoint.

<img src="https://raw.githubusercontent.com/JensKrumsieck/PowerPoint-Remote/master/.github/screenshot.png" alt="PPTRemote" width="250px"/>

Open a PPT File to start the Server and use the QR Code to navigate your webbrowser to the internal server. You will see a preview of the current slide and next/previous buttons to navigate through the presentation.

The Slideshow is controlled by sending webrequest from the web interface to the internal server. The server forwards the commands via COM to the powerpoint instance. Screenshots of the Slideshow-Monitor are send as MemoryStream-based response to the /preview request to show the current slide as image.

<sub>some inspiration came from: https://github.com/PuZhiweizuishuai/PPT-Remote-control</sub>

