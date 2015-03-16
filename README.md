# Eilium - CasparCG Control Panel

Eilium is a CasparCG Control Panel software created and customized for eSports (electronic sports) events in Finland.

**The program has been used on national television and other events**

* Winter 2014, Lantrek '14. The first eSports tournament broadcast ever that was sent to national TV station web broadcasting service (YLE Areena)
* Summer 2014, Assembly Summer 2014. The first ever eSports tournament that was broadcasted on national television (YLE TV2) 
* Winter 2015, Assembly Winter 2015. The ASUS ROG CS:GO Tournament was broadcasted on national television (YLE TV2) 
* Winter 2015, Lantrek '15. The CMStorm CS:GO Tournament was broadcasted on national television (YLE TV2)

##Code & Guide

Please, do not shoot me for I have not commented much in the code and for some parts, it sucks.
I had a very tight schedule in which I had to upgrade the program and I did not have the time to think about 
the public release. I was hoping I could comment the code later on but again I cannot find the time to do so.

I also do not have the time right now to actually write up a guide for the program and the idea behind the design. 

**To put things short:**

* I've created the program so the user can select the template folder inside CasparCG server /templates/ folder. This way I just had to create the templates per game
and each broadcast post chose the folder they had to use.  
* For some templates I've created a background video, <template folder name>_background.mp4 which starts and stops each time the template is activated.
* We have successfully used in a broadcast two GoPro cameras hooked up into the same Decklink that CasparCG was running and used them as player cameras. After that they we turned on in the program and crop\position values were set to each layer. I had to test out the position and cropping values in CasparCG Client  and copy them to the program so that they looked right.
* There are hotkeys. Some might not work. You can use the XML to alter them.
* There is a folder path which should be set to where the Server is. This path is nescessary for something, I cannot remember what, in the program. 
* The client can connect to an external server in a properly set local area network (another windows machine). I cannot remember if Windows Firewall had something to do with this.

## Templates

Eilium uses a very specific set of Flash templates, listed below. Unfortunately I cannot release these
template sets as some are paid for and some have design which are not created by me but have been given to me by game 
design companies such as Blizzard. I will, hopefully, later on release the templates on my blog 
(www.ossipesonen.fi) when I've created a generic type to be shared.

* Casters
* Clock
* Countdown
* EndCredits
* Groups
* Info
* Interview
* Playoffs
* Presentation
* Schedule
* Scoreboard
* Sponsors
* Twitter
* vsTeams
* vsPlayers

For some of the templates Eilium uses background 

## Designs

I have included a _designs_and_previews folder on the project structure which includes some layer designs and previews of the designs I've done and actually completed. 
Again, I apologize for not going into this more deeply. 

**Basic tips**

* I've used a safe-zone of 5% in 1080p resolution for web broadcasting, 10% for TV. 
* A dude in YLE was really cautious about text showing so some stuff I had to use 60px fonts on 72dpi but it can be less, like 42px.
* Make sure no templates overlap. Create a framework and make sure you follow it, meaning you STOP overlapping layer.
* Embed fonts on Flash
* Use Tweener in Flash. Takes less time.
* Create a Class file for each template. 
