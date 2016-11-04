CS:GO Server Selector
=======================

Hooray! Finally an SDR (Steam Datagram Relay)-ready server picker!
Better yet, there's no ads! But wait, there's more!

Yes, the server picker is now hosted right here on github.

No more waiting for a one-man-band to fix things, no: The CS:GO community can now work together on a better tool.

****

The current version is still missing some data, so here's how you can help:

* Download the most recent build of the tool.
* In CS:GO, put `steamdatagram_client_spew_level 5`. (This makes Steam give you more information about your connection.)
* When the GC puts you in the wrong server, look for `XXX#YY (AAA.BBB.CCC.DDD)` in the console.
* Copy this information down - this is the Datagram Relay country code and IP.
* Search to see if yours has been reported under *Issues*. If not, add it!
