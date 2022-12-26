# Notusu

_Your mapping software is so weird!_

Notusu is a Command Line Application to convert between .nbs (note block music project) and .osu (osu!mania 4K beatmap) files. What.

## Usage

### `.nbs` to `.osu`

#### Generating Metadata

Program reads all data from your nbs file, so you need to provide metadata (song title, artist, difficulty, etc.) in your .nbs file by filling out the song properties form. Write the mapper's name in "Song Author" and the artist's name in "Original Author". Song description is used to store difficulty name, key count of the map, and the timing offset. Write the description in a form like this:

```
diff=The Difficulty Name
keys=4
offset=0

```

And it must be ended with an empty line.

#### Mapping in ONBS

You will need to create N tracks (N=key count), each named with "TrackX" (X is a number ranged between 1~N), and an extra track named "Timing". Timing track must be placed most down below to be recognized properly because of a hidden feature in the program ~~(or bug)~~. Also add a custom instrument named "Tempo Changer" and keep its sound file "None".

##### Timing

Change the instrument to "Tempo Changer" and place notes on Timing track to create timing points. The BPM of the timing point is the pitch of the note(e.g. place a tempo changer note pitched +150 on timing track to create a timing point with 150 BPM). If there is no note on the first slot of timing track, in other words, no timing point on the first beat, the program will generate one according to the BPM stored in your song.

##### Placing Notes

Use any instrument you like and place notes on tracks above to place notes. Currently LNs (long notes) are not supported.

The program will ignore all properties of notes on your tracks, but please keep all notes in Minecraft's 2 octave range because out ranged notes have other features that are currently not implemented yet.

#### Use the Program

Compile the source in .NET 6.0 SDK and you will get the executable binary file (`Notusu.exe` on Windows or `Notusu` on Mac/Linux). Execute it in the command line like this:

```
Notusu -i your/nbs/file.nbs -t osu
```

Then a .osu file will be generated in the current working directory. Or you can specific the output directory by giving an extra argument in command line:

```
Notusu -i your/nbs/file.nbs -t osu output/to/here
```

Now import the song in osu!, copy the .osu file into the song folder, and enjoy!

### `.osu` to `.nbs`

Converting .osu to .nbs is not implemented yet!

## Special Thanks

[Open Note Block Studio](https://opennbs.org)

## Declaration

This program is not officially supported by osu! dev team. Use it in your own risk. Neither the author of Notusu nor osu! dev team is responsible to any crash and error of osu! or your system caused by Notusu and its generation.
