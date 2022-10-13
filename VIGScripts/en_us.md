# Vector image generator: sample script

This file is an example script for creating a vector image file. As you can see,
comments can be located at the beginning of file and are not highlighted by special
characters. In addition, comments can be located between description blocks
and at the end of file.

Description blocks must be separated from other description blocks and from comment
blocks by at least one paragraph; a paragraph is the end of a block. Blocks
of descriptions begin with keywords that should be written strictly in such way
(preserving the case of letters) as shown in this example. Any block may be absent.

#

The ```[Line]``` block describes curves displayed on the picture. Each individual
block describes a separate curve. Each line contains coordinates of single point
on curve. Points are connected in order of appearance. There may be lines
of special types:

- If some line of the block begins with keyword ```c=``` (with small letter,
without spaces), the line should contain a description of curve’s color
in format ```c=R;G;B```, where ```R```, ```G``` and ```B``` are color decimal components.
- If the line starts with keyword ```w=```, then the thickness of curve line
should be specified after the equal sign.

If any keyword occurs repeatedly, then the value of corresponding parameter
will be replaced by newly specified. Moreover, all lines of special kind
may be absent.

```
[Line]
c=0;0;255
w=5
200;100
125;100
100;125
100;150

[Line]
100;150
c=255;0;0
100;175
w=4
125;200
200;200

[Line]
w=10
200;200
200;160
180;160
c=0;128;0
```
The ```[Ox]``` block describes Ox axis. If ```[Ox]``` keyword is repeated in the script file,
it will be ignored. Each line contains description of the notch in format ```O;S```,
where ```O``` is an offset of the notch from the left edge of axis; ```S``` is length
of the notch (in half on both sides of the axis). There may be lines of special types:

- Starts with ```c=``` – color of axis and notches.
- Starts with ```w=``` – thickness of the axis line and notches.
- Starts with ```o=``` – similar to ```w=``` – offset relative to the top of image.

If any keyword occurs repeatedly, then the value of corresponding parameter
will be replaced by the newly specified. Moreover, all lines of special kind
may be absent.

```
[Ox]
o=200
w=2
c=255;128;0
100;10
150;10
200;10
```
The ```[Oy]``` block describes Oy axis (similar to Ox axis).

```
[Oy]
o=100
w=3
100;15
170;20
230;15
```
The ```[Text]``` block describes text labels on the image. If ```[Text]``` keyword is repeated
in the script file, contents of corresponding block will be added to existing
descriptions. Each first line contains line parameters in format ```X;Y;R;G;B;S```,
where ```X``` and ```Y``` are coordinates of the label; ```R```, ```G``` and ```B``` – color;
```S``` is the font size. Each second line contains the text itself.

```
[Text]
150;250;0;255;0;10
Fig. 1. Diagram 1
100;195;255;128;0;8
100
150;195;255;128;0;8
150
200;195;255;128;0;8
200
110;100;0;0;0;8
100
110;170;64;64;64;10
170
110;230;0;0;0;8
230
```
The ```[Include]``` block allows you to connect other scripts with descriptions
of image elements. Block may be absent. Location point of the block
will be replaced by contents of specified file. Coordinate plane of the content
will be shifted according to specified values of ```X``` and ```Y```. If included files
also contain ```[Include]``` blocks, they will also be replaced by files specified
in them. Total number of blocks in all files should not exceed 100. Assembled
final script will be processed according to rules described above.

```
[Include]
C:\Axes.sc
0;100

[Include]
C:\Texts.sc
100;150

```
In the final script, curly braces and offset coordinates will also be added
to contents of the included file:

```
{
0;100
<Contents of Axes.sc>
}
```

This expression can also be used in the source script to combine and offset
group of objects.

#

Starting with version 1.3, the application can be used from the command line.
To view call description, run ```SVGGenerator /?``` command. If generation from
the command line passes without errors, application does not return any messages
so as not to delay the batch processing of many files. If there are errors,
they are displayed as regular messages
