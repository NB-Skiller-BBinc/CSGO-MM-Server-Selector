CS:GO Server Selector
=======================

Here's a temporary *working* version of the CS:GO Server Selector.

Things are going to be broken, but here's a quick guide:

* Start the Server Selector **FIRST**, and block the regions you don't want to queue in one-at-a-time.
* Only then can you start CS:GO. Starting it sooner will bork MM, as pings are calculated from the moment the menu starts.
* The program may (read: WILL) crash, sometimes taking your network access with it. If that happens, run `unblock.bat` and restart the program. After doing so, unblock all servers and continue using it from there.


The current version is still missing some data, so here's how you can help:

* Download the most recent build of the tool.
* In CS:GO, put `steamdatagram_client_spew_level 4`. (This makes Steam give you more information about your connection.)
* When the GC puts you in the wrong server, look for `XXX#YY (AAA.BBB.CCC.DDD)` in the console.
* Copy this information down - this is the Datagram Relay country code and IP.
* Search to see if yours has been reported under *Issues*. If not, add it!
