# MonteHalo

MonteHalo is a halo simulation application built on the Universal Windows Platform (UWP). Halo is a kind of (or should I say, various kinds of) atmospheric phenomenon caused by ice crystals in the sky. The name MonteHalo combines the topic of interest **Halo**, with the basic algorithm used behind the codes, the **Monte Carlo** method. Considering "Monte" and "Halo" both come from a Latin root (while "Halo" is actually borrowed from ancient Greek ἅλως), it literally means, the mountain of halo.

Halo phenomenon can take a variety of forms, some very common that one can observe in almost any sunny days with cirrus cloud. Others may take a bit of luck to be witnessed. If you are interested in the phenomenon, please visit [atoptics](http://www.atoptics.co.uk/halosim.htm) and [AKM e.v.](https://www.meteoros.de/themen/halos/) for more information.

The app is meant to provide a chance for everyone to know this fantastic miracle of nature, and at the same time establish a platform for halo observers and researchers.

##Install on your machine
Since this is a UWP app, currently it only support windows 10. I may consider to publish a WPF or XAMARIN version in the future (But I have to learn those things first). 

The DEMO is not yet uploaded to windows store. Please clone or download the project from this website and open the MonteHalo.sln file with Visual Studio 2015 or higher version. Simply click run and it shall do the rest. Enjoy!

##Run
To be completed.

##Features of Versions

###In Demo version
1. Single hexagonal prism monochromatic simulation.
2. Dome scattering plot, free to look in different directions.
3. Free to choose the geometric and orientation parameters of the crystal.
4. Free to choose the sun altitude.

### More to come **SOON!**
1. Chromatic plotting, plane plotting in various kinds of coordinates.
2. Mixing the crystals.
3. More crystal shapes, like pyramidals.
4. More organized, user friendly parameter input page.
5. Mouse operation on 3D views.
6. Ray tracking demonstration.
7. Classify the halos by type.
8. Storing data using database.
9. A detailed introduction for halo.

##Author and Acknowledgement

This application is written by 王凯越, currently a senior student from USTC. I welcome other programmers to report issues and fire a pull request to contribute to the program. 

I used DirectX 11 for the plotting part. Special thanks to SharpDX team for bringing the DX11 to C#, which makes things much easier. I also referenced HelixToolkit for managing my interop between UWP app code with DX11.

Soon, I will be using SQLite for database features.
