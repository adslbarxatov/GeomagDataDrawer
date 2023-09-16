# Geomag data drawer: complete user guide
> **ƒ** &nbsp;Nicolay B. aka RD_AAOW, Соля́ников Я.; 16.09.2023; 14:41

# Contents
- [General information](#general-information)
- [Video guide](https://youtube.com/watch?v=uYtQJX5BgvU)
- [Main app interface](#main-app-interface)
    - [Adding a curve or object to a diagram](#adding-a-curve-or-object-to-a-diagram)
    - [Removing curves and objects from diagram](#removing-curves-and-objects-from-diagram)
    - [Hide / show curves and objects](#hide--show-curves-and-objects)
    - [Selection of curves and objects](#selection-of-curves-and-objects)
    - [Selection of a range for plotting a curve or line](#selection-of-a-range-for-plotting-a-curve-or-line)
    - [Setting the location and size of curve or object](#setting-the-location-and-size-of-curve-or-object)
    - [Setting the caption of curve image](#setting-the-caption-of-curve-image)
    - [Setting coordinate axes of curve image](#setting-coordinate-axes-of-curve-image)
    - [Setting the grid of curve image](#setting-the-grid-of-curve-image)
    - [Customizing fonts and colors for captions](#customizing-fonts-and-colors-for-captions)
    - [Set curve line style](#set-curve-line-style)
- [App menu](#app-menu)
    - File
    - [“Open data file”](#open-data-file)
    - [“Load from clipboard”](#load-from-clipboard)
    - [“Save data file”](#save-data-file)
    - [“Save diagram”](#save-diagram)
    - [“Close diagram”](#close-diagram)
    - [“Generate curves”](#generate-curves)
    - [“Settings”](#settings)
    - [“Exit”](#exit)
- Operations
    - [“Add curve or object”](#add-curve-or-object)
    - [“Parametric curves addition”](#parametric-curves-addition)
    - [“Select curve data columns”](#section-27)
    - [“Merge curves or objects”](#section-28)
    - [“Delete curves or objects”](#section-29)
    - [“Edit diagram data”](#section-30)
    - [“Load style”](#section-31)
    - [“Save style”](#section-32)
    - [“Reset style”](#section-33)
    - [“Save curves template”](#section-34)
- Help
    - [“About / Help”](#section-36)
    - [Menu item with interface language selection](#section-37)
    - [“Associate files”](#section-38)
    - Optional
    - [“Join data tables”](#section-40)
    - [“Create vector image”](#section-41)
- [Hardware and software requirements and app contents](#section-42)
    - App contents
    - [Files and directories](#section-44)
    - [Supported data formats](#section-45)
    - [Support for Microsoft Office Excel formats](#microsoft-office-excel)
    - [Requirements for installing the app](#section-46)
    - [Working with the app from the command line](#section-47)
- [Limits and default parameter values](#limits-and-default-parameter-values)
- [File format specifications](#file-format-specifications)
    - Geomag data drawer files
    - [Microsoft Office Excel data files](#microsoft-office-excel-1)
    - [Data files in Windows CSV format](#windows-csv)
    - [Files to extract data from](#section-50)
- [Download links](https://adslbarxatov.github.io/DPArray#geomag-data-drawer)
- [Русская версия](https://adslbarxatov.github.io/GeomagDataDrawer/ru)

---

# 1. General information

***Geomag data drawer*** is a software tool designed to build diagrams based on tabular data. It was originally
created as a means of visualizing the results of experimental measurements; can still be used for this purpose.

Geomag data drawer supports the following data formats:
- ***Microsoft Office Excel ’97 and ’07 Spreadsheets*** (`.xls`, `.xlsx`). Support is provided through additional software
(see section [“Hardware and software requirements and app contents”](#section-42) of this guide). The presence of the
installed Microsoft Office software package is not required.
- ***Table data in Windows CSV format*** (`.csv`; separators are semicolons).
- In order to expand the capabilities of the app, the ability to ***extract data from files of text formats*** has also been added.
- In addition, the app has its ***own data storage format***, which includes both the diagram data itself,
as well as its style and display settings (Geomag data drawer files, `.gdd`).
- The app can ***generate tabular data*** by running a given numerical range through analytically
given function. The function and plotting range can be manually specified by user.
- Finally, it is possible to use ***text data from the clipboard*** for plotting.
- Data file conversion from the command line is also available, which allows automatic (batch) processing.

In terms of data visualization and processing, Geomag data drawer provides the following main features:
- Construction of diagrams according to the presented data.
- Generation of data for plotting curves from an analytically given function.
- Editing diagram data in the built-in editor.
- Saving data files in Windows CSV format and as raw tabular data.
- Customization of most of the graphic characteristics of diagrams: sizes, placement, colors, fonts, lines, markers, etc.
- Setting your own coordinate plane and independent use of data columns as abscissas and ordinates for each dependence under study.
- Adding additional graphic objects: text labels, rectangles, ellipses and lines.
- Saving diagrams in graphic format `.png` with the ability to set a sufficiently large output size.
This allows you to overcome the limitations of raster graphics when further using the image.
- Saving diagrams in vector formats `.svg` version 1.1 and `.emf`, if it is necessary to avoid losses
when inserting an image into a printed document, or additional editing of the generated diagram in a vector graphics editor is required.

Implementation as a standalone application, support for all basic features for editing, building and designing diagrams, flexible
settings system, bilingual interface, as well as the ability to store data in its own format eliminates the need to install additional
software systems. Therefore, ***Geomag data drawer*** can be considered a very useful tool for processing and visualizing tabular data.

[:arrow_double_up:](#contents)

---

# 2. Main app interface

The main window of the app is shown in the figure below.

<img src="/GeomagDataDrawer/img/2_01.png" />

The main elements of the window:

&#x25B6; The diagram display field.

&#x25B6; List field of added curves and objects (selection of an arbitrary set of points for simultaneous adjustment is available; see below).

&#x25B6; Buttons for adding (<img src="/GeomagDataDrawer/img/2_70.png" />) and deleting (<img src="/GeomagDataDrawer/img/2_73.png" />) curves and additional objects.

&#x25B6; Button for hide / show (<img src="/GeomagDataDrawer/img/2_60.png" />) curves and objects.

&#x25B6; Diagram options panel, which allows you to customize your own display settings for selected curves and objects. Namely:

&#x3000;&#x25B7; in what range of abscissas and ordinates should the curve be drawn, and whether transposition is required (tab <img src="/GeomagDataDrawer/img/2_33.png" />);

&#x3000;&#x25B7; how to position the image of curve and/or object on the sheet, and what size it should be (tab <img src="/GeomagDataDrawer/img/2_34.png" />);

&#x3000;&#x25B7; whether it is necessary to manually set the location of the diagram captions and the title of the figure, or the app should set them automatically (tab <img src="/GeomagDataDrawer/img/2_36.png" />);

&#x3000;&#x25B7; how to display the coordinate axes of the diagram (tab <img src="/GeomagDataDrawer/img/2_46.png" />);

&#x3000;&#x25B7; how to display the coordinate grid of the diagram (tab <img src="/GeomagDataDrawer/img/2_47.png" />);

&#x3000;&#x25B7; how to display diagram labels (tab <img src="/GeomagDataDrawer/img/2_48.png" />);

&#x3000;&#x25B7; how to display the lines of curves on the diagram (tab <img src="/GeomagDataDrawer/img/2_49.png" />);

&#x25B6; App menu (see section “[App menu](#app-menu)”).

&nbsp;

The main window of the app during operation is shown in the figure below:

<img src="/GeomagDataDrawer/img/2_02.png" />

All changes made by user in the diagram parameters field are immediately reflected in the diagram in relation
to those curves and graphical objects that are marked in the list of curves and objects and highlighted
in gray in the diagram field. The set of editable curves and objects can be arbitrary. Adding to the editable
group is performed in the same way as selecting a group of files in Windows Explorer, i.e. using left mouse button
and `[Ctrl]` and `[Shift]` keys. Thus, the user can fine-tune parameters quickly and efficiently.

Each control in the parameter field, tabs, as well as app menu items are provided with hints. To call them,
it’s enough to hold the cursor over any element for two seconds. If there is no movement, the tooltip is displayed for 30 seconds.

The app supports the function of an operating system *“Open with...”*. If loading is successful, the app immediately
starts working with the specified file.

It’s also possible to *automatically build curves when loading a file* (if its content allows). To do this, the app
creates a standard template file `GeomagDataDrawer.txt` (if it doesn’t already exist), similar to the one used
in the [parametric curves addition](#parametric-curves-addition) function. The user can change it at will by creating different
preview styles. This allows you to speed up the search for the desired data file, focusing on the image of the diagram,
and not on its tabular representation (similar to viewing photos).

The app has an ability to *move around the plotting area*. To do this, you can use scroll bars, as well as mouse pointer.
In the latter case, you need to left-click in any part of the area and move pointer in the direction in which you want
to move the diagram, then release the button (similar to movement of a hand when moving a large sheet of paper).

The size of plotting area, and, accordingly, the size of saved image of the diagram is determined by the location
of images of curves and objects. Its lower right corner coincides with the lower right corner of an edge curve image.
Accordingly, to increase the plotting area, it’s necessary to move this image further to the right and/or down.

Finally, if work on the diagram hasn’t been completed, all data and settings can be saved to the app's own file format,
and then restored at the next start. This function is duplicated by the *function of autosave (when closed)
and auto-recovery (when opened)* of the app state.

[:arrow_double_up:](#contents)

---

## 2.1. Adding a curve or object to a diagram

To add a new curve or graphical object to the diagram, click the <img src="/GeomagDataDrawer/img/2_70.png" />
button next to the list of curves and objects. A hotkey is available for this function, see section “[App menu](#app-menu)”
of this guide. This will open the addition options window:

<img src="/GeomagDataDrawer/img/2_03_en.png" />

When adding a curve, you’ll need to specify which column from the downloaded data file will be *the abscissa column*
and which will be *the ordinate column*. When adding an object, you’ll need to select its type (in the current version
– line (4 types), rectangle, ellipse or text label). The number of curves and objects in the diagram should not
exceed the maximum allowed (see section [“Limits and default parameter values”](#limits-and-default-parameter-values) of this guide).

[:arrow_double_up:](#contents)

---

## 2.2. Removing curves and objects from diagram

Using the <img src="/GeomagDataDrawer/img/2_73.png" /> button, you can remove curves and/or objects from the diagram. This will
require user confirmation. User can select multiple curves and objects in the list to delete at the same time. A hotkey is available
for this function; see section “[App menu](#app-menu)” of this guide.

[:arrow_double_up:](#contents)

---

## 2.3. Hide / show curves and objects

Using the <img src="/GeomagDataDrawer/img/2_60.png" /> button, user can temporarily hide the selected curves and objects
(the button will then be “released”). If necessary, they can be displayed again at any time (the button will be “pressed”).

[:arrow_double_up:](#contents)

---

## 2.4. Selection of curves and objects

The middle mouse button allows you to select curves and objects for customization with
the mouse. To do this, press the button and (holding it) select the area in which the required curves and objects are located
in the diagram plotting field. All diagram elements affected by the rectangular selection area
will be highlighted in the list of added curves and objects.

[:arrow_double_up:](#contents)

---

## 2.5. Selection of a range for plotting a curve or line
> *Settings on this and all subsequent tabs apply to all curves selected (highlighted) in the curve list.
> Some functions will be unavailable when working with graphic objects*

The content of the tab is shown in the figure below:

<img src="/GeomagDataDrawer/img/2_50_en.png" />

Plotting ranges are calculated automatically when loading each curve in accordance with its real boundaries.
Moreover, the boundaries are rounded up to the first significant figure on the left. Despite this, they can be changed
manually by user at his discretion. From this tab, the transposition of the curve is also available, i.e. exchange
of abscissas with ordinates in places.

Note that range boundaries aren’t required to respect the `max ≥ min` rule. In particular, the coordinate plane
can be “reflected” horizontally, vertically, or in both directions at once. This can be done both manually
and using <img src="/GeomagDataDrawer/img/2_37.png" /> and <img src="/GeomagDataDrawer/img/2_38.png" /> buttons,
which will correspond to the rotation of the axes abscissa and ordinate, respectively. However, the transposition
option remains available. All this allows you to increase the flexibility and informativeness of the diagram,
depending on the needs of a particular study.

[:arrow_double_up:](#contents)

---

## 2.6. Setting the location and size of curve or object

The content of the tab is shown in the figure below:

<img src="/GeomagDataDrawer/img/2_51_en.png" />

The image size of an individual curve or object and its offset are limited by the maximum sheet size
(see section [“Limits and default parameter values”](#limits-and-default-parameter-values) of this guide).
But due to the ability to move around the plotting area and its rather large size, the app allows you to build
and place a large number of curves on the diagram in the required way.

Using the appropriate buttons, you can arrange the selected curves and objects in a row or column. The app will
automatically calculate the corresponding coordinates for the selected images.

App also has the ability to set the location of the image of each element of the diagram using mouse:
- By *double-clicking the left mouse button* on the diagram field, the currently selected images are shifted
so that their upper left corners are set to the clicked position.
- By *double-clicking the right mouse button* the images are resized so that their lower right corners
are at the clicked position.
- By *holding down the* `[Ctrl]` *key and double-clicking the left mouse button* the figure’s caption is shifted.
- By *double-clicking the right mouse button while holding down* `[Ctrl]` *key* the labels of the diagram axes are shifted
so that they visually intersect at the point where the click was made.

The new parameter values are transferred to the corresponding fields of the diagram settings. Therefore,
the restrictions on this function are the same as on the corresponding fields.

In relation to graphical objects, the offset fields determine their location on the diagram, and the size fields
set the size of rectangles, ellipses, and line display fields. Field sizes for text labels are calculated automatically.

[:arrow_double_up:](#contents)

---

## 2.7. Setting the caption of curve image

The content of the tab is shown in the figure below:

<img src="/GeomagDataDrawer/img/2_53_en.png" />

When you select *automatically set captions* and their location, all other fields become inaccessible, and the corresponding
parameters are calculated automatically by the app. Axis labels are placed next to the axes inside the coordinate plane.
Image captions are arranged so that when several curves are superimposed, captions are in one line with spaces between them.
The caption itself, as well as the name of the curve in the list, is taken either from the data file, or is formed by the app
using serial numbering.

When you select *manually set captions*, the values calculated by the app are substituted into the parameter fields.
In the following, they’re limited only by size of the curve image. Note that the diagram label field in this tab only applies
to the first of selected curves if there are several of them selected.

As applied to the “Text label” graphical object, the label field sets the text of the label itself. The behavior of the object
doesn’t depend on the state of the “Autodetect” checkbox.

[:arrow_double_up:](#contents)

---

## 2.8. Setting coordinate axes of curve image

The content of the tab is shown in the figure below:

<img src="/GeomagDataDrawer/img/2_54_en.png" />

Using the appropriate fields, you can set the number of main and additional divisions on the diagram axes, the thickness
of axes in pixels, as well as their color and relative position, specify the format for displaying numbers (normal or exponential).

You can also select *auto-detection* for the number of divisions. In this case, the number of serifs is chosen as the most appropriate
for the given plotting range.

All parameters (here and below) have valid ranges; you can find them in the
section [“Limits and default parameter values”](#limits-and-default-parameter-values) of this guide.
The color of axes is adjusted by pressing the colored button next to the corresponding inscription.
The button has a color corresponding to the last selected axis color.

Axes can be forced to the left (top), center (middle) or bottom (right), or their location can be determined automatically.
In the latter case, app will set the axis to the intersection with the zero of the other axis or align it to the side to which
the zero is closer.

The “×” button hereinafter is intended to disable the display of a particular element. This, in fact, means setting
a white color for it, which is interpreted by the app as transparent and is not displayed when redrawing. Specifying
white for the axes, grid, and labels will have the same effect.

[:arrow_double_up:](#contents)

---

## 2.9. Setting the grid of curve image

The content of the tab is shown in the figure below:

<img src="/GeomagDataDrawer/img/2_55_en.png" />

Using the appropriate fields, you can set thickness of the grid lines, as well as colors of the main and additional lines. The grid
line colors default to match the background (white), so the grid is not visible. When “turning on” the grid, its lines are rebuilt
in accordance with location of the notches on the axes. The “×” button or setting the line color to white hides the grid.

[:arrow_double_up:](#contents)

---

## 2.10. Customizing fonts and colors for captions

The content of the tab is shown in the figure below:

<img src="/GeomagDataDrawer/img/2_56_en.png" />

The tab allows you to configure the display style of the image caption, labels of the curve axes and the “Text line” graphic object.
The font selection buttons call up a standard window for selecting a font, its size and style. Colored buttons allow you to set
the colors of the labels.

The app doesn’t allow the use of some fonts. If the user selects an inappropriate font, the app notifies him about this
and offers to choose another one.

Setting the captions color to white disables them.

[:arrow_double_up:](#contents)

---

## 2.11. Set curve line style

The content of the tab is shown in the figure below:

<img src="/GeomagDataDrawer/img/2_57_en.png" />

The tab allows you to configure the thickness of curve line or the “Line” graphic object, its color and display style,
as well as the marker used. By style we mean one of three options for representing a curve:
- “Line only” – indicates display of a line without markers;
- “Line and markers” – indicates display of a line with markers;
- “Markers only” – indicates the display of only markers at the reference points of a curve.

If the style expects to display markers, the marker selection field becomes available; otherwise it is blocked. The list
of available markers consists of two parts:
- Standard markers generated by the app. Available as 1 (square), 2 (circle), 3 (triangle), 4 (empty rectangle), 5 (ring) and 6 (cross).
- Additional markers.

Additional markers are obtained by the app from images in `.png` format, stored in the `Markers` directory, located
in the same directory as the app. If this directory doesn’t exist, it will be created automatically. If the directory contains
images in `.png` format, app loads as markers only those that are available, intact and of the appropriate size
(see section [“Limits and default parameter values”](#limits-and-default-parameter-values) of this guide). Successfully
loaded markers are available under numbers 7, 8 and beyond.

It should be noted that app converts loaded images in a special way. More precisely:
- White (`#FFFFFF`) color or white transparent color (`#00FFFFFF`) is replaced with white transparent color;
- All other colors are replaced with the selected curve color.

This feature allows you to use almost any image as markers without much modification. Additional markers aren’t included
in the standard package of the app.

[:arrow_double_up:](#contents)

---

# 3. App menu

The main menu is represented by items described below. In parentheses are “hot keys” that can be used to call up
the corresponding menu.
- File; includes submenu:
    - Open data file (`[Ctrl]` + `[O]`);
    - Load from clipboard (`[Ctrl]` + `[V]`);
    - Save data file (`[Ctrl]` + `[S]`);
    - Save diagram (`[Ctrl]` + `[I]`);
    - Close diagram (`[Ctrl]` + `[W]`);
    - Generate curves (`[Ctrl]` + `[G]`);
    - Settings (`[Ctrl]` + `[P]`);
    - Exit (`[Ctrl]` + `[Q]`).
- Operations; includes submenu:
    - Add curve or object (`[F5]`);
    - Parametric curves addition (`[Ctrl]` + `[F5]`);
    - Change curve data columns (`[F6]`);
    - Merge curves or objects (`[F7]`);
    - Remove curves or objects (`[F8]`);
    - Edit diagram data (`[Ctrl]` + `[E]`);
    - Load style (`[F11]`);
    - Save style (`[F12]`);
    - Reset style (`[F9]`);
    - Save the template for adding curves (`[Ctrl]` + `[T]`);
    - Replace preview template;
    - Restore preview template.
- Additional; includes submenu:
    - Merge tables;
    - Generate vector image.
- Help; includes submenu:
    - About the app / help (`[F1]`);
    - Select the interface language (list).
    - Associate files.

[:arrow_double_up:](#contents)

---

## 3.1. “File”

## 3.1.1. “Open data file”

Calls up a standard file open window. The ability to add curves and objects appears immediately after confirming the selection
of a data file, if it’s available and has the correct format. If an error occurs during loading, the user receives
a corresponding message, and the app again displays the last successfully loaded diagram.

If the user selected a Geomag data drawer (`.gdd`) file that already contained settings for displaying curves and graphical
objects, their last state will be displayed in the diagram field. If you use a template to automatically add curves to your
diagram, they will also be displayed when you load the data file. In all other cases, work begins with an empty field
in the diagram. But in any case, the file must contain at least two lines and at least two columns of data.

The app explicitly sets the maximum allowed number of columns and rows of data
(see section [“Limits and default parameter values”](#limits-and-default-parameter-values) of this guide).
Columns and rows of data that are out of range are ignored during loading.

When choosing the option to extract data from a text file, the user will first need to specify the expected number
of columns of data.

<img src="/GeomagDataDrawer/img/3_22_en.png" />

This value may be specified with a margin if the exact number of columns is unknown. All missing values will be replaced
with zeros. For more information about data extraction, see section [“File format specifications”](#file-format-specifications)
of this guide.

When opening any file (except `.gdd`), the user will also be required to specify the number of lines in the file that
will be used to look up the data column names.

<img src="/GeomagDataDrawer/img/3_80_en.png" />

The specified number of lines will be considered by the app exactly as text: the data from these lines will be ignored.
This value must be specified accurately, because a smaller value will cause an error (the text will appear where the data
should be), and a larger value will lead to the loss of part of the data and incorrect display of column names.

The data column labels extracted from the data file are used to mark and label curves in the app. If column names couldn’t
be read (in particular, if the number of expected columns during data extraction turned out to be more or less than
the actual number of columns), the app names them in the order of reading: c.1, c.2, etc.

[:arrow_double_up:](#contents)

---

## 3.1.2. “Load from clipboard”

The menu item allows you to use text data on the clipboard copied from another app to create a diagram.
This will prompt the user to specify the expected number of columns and number of rows to search for column names.

> ***All unsaved data is lost if the user agrees to the corresponding warning***

[:arrow_double_up:](#contents)

---

## 3.1.3. “Save data file”

The app can save loaded data to files of all types that it supports (except `.xls` and `.xlsx`). If saving occurs with
an error, user is notified about this. Data column names can also be saved if the appropriate option is selected
in the app settings. It should be noted that saving graphic objects is only possible in the app’s own format (`.gdd`).

[:arrow_double_up:](#contents)

---

## 3.1.4. “Save diagram”

Calls up a window of options for forming the final diagram image.

<img src="/GeomagDataDrawer/img/3_15_en.png" />

The user is offered the following saving options:
- *Save visible layout*. In this case, the image is saved in the form and size in which it is displayed in the main app window.
In this case, you can zoom in on the image if necessary, for example, to improve its quality. Limitations on the scale
can be found in the section [“Limits and default parameter values”](#limits-and-default-parameter-values) of this guide.
File format is `.png`.
- *Adjust the layout*. This group of options allows you to save images in A3 and A4 paper size proportions. In this case,
the image displayed in the main window is located in the upper left corner of the future sheet of paper, and its size
is recalculated so that it fills the sheet as much as possible, taking into account its location and size. The unfilled
part of the sheet remains empty. Note that image conversion is performed in 300 dpi quality, which ensures acceptable
print quality on black and white and color printers. File format is `.png`.
- *Save as vector image*. This option allows you to save images in `.svg` and `.emf` vector formats. Suitable for making
more subtle modifications to a diagram image, as well as in other cases where a bitmap image is not appropriate
(for example, when positioning an image within the text of a printed document). The elements of the figure are grouped
in such a way as to minimize the time required for manual correction of the diagram.
 
The “Save” button opens a standard save window; the “Cancel” button aborts the operation.

[:arrow_double_up:](#contents)

---

## 3.1.5. “Close diagram”

The menu closes the current diagram and returns the app to its initial state. Settings fields and menu items related
to saving are blocked.

> ***All unsaved data is lost if the user agrees to the corresponding warning***

[:arrow_double_up:](#contents)

---

## 3.1.6. “Generate curves”

Calls up a window that allows you to set analytical functions and data generation range.

<img src="/GeomagDataDrawer/img/3_81_en.png" />

User can specify the functions whose tabular representation needs to be calculated and specify the plotting range.
If errors are detected when composing a function, explanatory messages are displayed below the function input field.
The correctness of the function setting is checked by pressing the “+” button or the [Enter] key; the function is added
to the list of tested functions. The number of curves you can add is limited (see section
[“Limits and default parameter values”](#limits-and-default-parameter-values) of this guide). Range correctness
is checked when you click “OK” button.

The “OK” button starts generation and transfers data for drawing to the main app window. After this, all main menu functions
become available. The “Cancel” button returns the app to its previous state.

> ***All unsaved data is lost if the user agrees to the corresponding warning***

[:arrow_double_up:](#contents)

---

## 3.1.7. “Settings”

Calls up the app settings window.

<img src="/GeomagDataDrawer/img/3_17_en.png" />

The user can:
- Set a mandatory request to exit the app. If this option is disabled, the app can be exited, including by accidentally
closing the main window, which can lead to the loss of important data (unless the following option is set).
- Enable the function of automatic restoration of the app state. If enabled, the last loaded data and displayed curves
will be restored by the app in the same form the next time it is launched.
- Enable the function of automatically adding curves to the diagram when loading a data file (see section
[“Main app interface”](#main-app-interface) of this guide).
- Enable the function of saving data column names in text data files.

The “Save” button causes the app configuration to be updated; the “Cancel” button allows you to leave the settings
window without changing them.

[:arrow_double_up:](#contents)

---

## 3.1.8. “Exit”

Allows you to exit the app. Depending on the settings, exit may be accompanied by saving the diagram state and
a request to exit. In this case, the app saves the size and position of the main window; they are restored at the next startup.

> ***All unsaved data is lost if the user agrees to the corresponding warning***

[:arrow_double_up:](#contents)

---

## 3.2. “Operations”

## 3.2.1. “Add curve or object”

The menu item performs functions similar to those described in
[clause 2.1](#adding-a-curve-or-object-to-a-diagram) of this guide.

[:arrow_double_up:](#contents)

---

## 3.2.2. “Parametric curves addition”

If you need to quickly add several diagrams of the same type and also quickly arrange them on a sheet, you can use
the parametric addition of curves. Using the corresponding dialog box, you can specify the numbers of data columns,
the dimensions of each image and its position, as well as its caption.

<img src="/GeomagDataDrawer/img/3_21_en.png" />

If you need to add several diagrams at once, you can use the option to load parameters from a file. For this purpose,
the user can create a text document, the lines of which are formed according to the rules specified in the dialog box,
and load it by clicking the “File” button. In addition, a file suitable for this function can be created using
the corresponding menu item.

[:arrow_double_up:](#contents)

---
:warning: ***Translation is in progress*** :warning:
---

## 3.2.3. «Выбор столбцов данных кривой»

Пункт меню позволяет изменить столбцы данных, использованные при построении выбранной кривой. При этом будет вызвано окно, аналогичное тому, которое
вызывается при простом добавлении кривой. Здесь потребуется указать, какой столбец из файла данных будет новым столбцом абсцисс, а какой – столбцом
ординат в выбранной зависимости. Данная опция может быть полезна в случае, если требуется быстро построить новую кривую на месте имеющейся,
используя настройки стиля и расположение последней.

[:arrow_double_up:](#contents)

---

## 3.2.4. «Совместить кривые или объекты»

Пункт меню позволяет совместить две кривые, т.е. расположить их друг над другом и опционально задать общие оси. Функция требует, чтобы перед
её вызовом были выделены ровно две кривые; в противном случае совмещение выполнено не будет.

Окно параметров совмещения кривых представлено на рисунке ниже.

![image063](https://user-images.githubusercontent.com/20893717/147869889-1bf8e42c-f477-4abd-8fe7-b186be280f91.png)

Пользователь может указать, на месте какой из кривых следует расположить результат совмещения. Также можно выбрать, какая ось будет общей
для двух кривых, и следует ли установить цвет подписей осей и подписи кривой в цвет самой кривой для повышения наглядности диаграммы.

При совмещении общим осям кривых присваиваются одинаковые параметры (максимальное и минимальное значение, число засечек и расположение),
а разные оси разводятся по краям изображения кривой. Общими также становятся размеры изображений кривых, значение параметра транспонирования кривой
и параметры осей; размещение подписей кривых выполняется в автоматическом режиме.

Данная функция может быть применена и к графическим объектам, если это необходимо. Обратите внимание, что процедура не является необратимой,
т.к. всего лишь меняет расположение и стили отображения кривых на диаграмме.

[:arrow_double_up:](#contents)

---

## 3.2.5. «Удалить кривые или объекты»

Пункт меню выполняет функции, аналогичные описанным в [пункте 2.2](#section-4) данного руководства.

[:arrow_double_up:](#contents)

---

## 3.2.6. «Редактировать данные диаграммы»

Пункт меню вызывает окно редактирования данных диаграммы:

![image040](https://user-images.githubusercontent.com/20893717/147869954-ae07465f-d803-4a64-ac7d-239b27fc1f3b.png)

> ***Данная опция может работать медленно и при вызове на некоторое время замораживать интерфейс программы.
> Это связано с особенностями механизмов работы редактора и не является сбоем или ошибкой.***

Данные в таблице представлены в том виде, в котором они были получены из файла данных. Для редактирования каждого отдельного значения достаточно выделить
соответствующую ячейку и начать клавишу `[F2]` либо дважды щёлкнуть её. Дробные значения должны вводиться с десятичным разделителем, указанным
в настройках ОС. Обо всех ошибках при заполнении ячеек таблицы пользователь извещается или сразу при попытке покинуть ячейку, или при попытке
сохранить внесённые изменения.

Доступно также редактирование имён столбцов данных. Для этого достаточно дважды щёлкнуть по заголовку столбца, название которого требуется изменить.

Для самой таблицы доступны следующие операции:
- Перемещение текущей выбранной строки на одну позицию вверх (кнопка
![image041](https://user-images.githubusercontent.com/20893717/147870027-ca60fb71-6be0-42c2-b173-64889eda63fd.png)).
В качестве текущей здесь и далее выступает последняя выделенная строка;
- Перемещение текущей выбранной строки на одну позицию вниз (кнопка
![image042](https://user-images.githubusercontent.com/20893717/147870034-8aaaf99a-2bd1-4065-a073-1658267f35f0.png));
- Добавление строки перед текущей выбранной (кнопка
![image043](https://user-images.githubusercontent.com/20893717/147870039-c363440c-acad-4f33-9157-867ca20e7d92.png));
- Добавление строки после текущей выбранной (кнопка
![image044](https://user-images.githubusercontent.com/20893717/147870051-fb300b87-8d6d-4661-b8df-dda21a72f0e1.png));
- Удаление выбранных строк (кнопка
![image045](https://user-images.githubusercontent.com/20893717/147870055-a2c9c3ff-c394-4ff8-ac07-b00867e873e3.png)).
Удаляются все затронутые выделением строки. То, как при этом выделены столбцы, не имеет значения.

Кнопка «Сохранить изменения» в случае их корректности применяет внесённые изменения к данным диаграммы и инициирует её перерисовку.
Кнопка «Отменить изменения» возвращает диаграмму в исходное состояние.

[:arrow_double_up:](#contents)

---

## 3.2.7. «Загрузить стиль»

Вызывает стандартное окно открытия файла, позволяющее выбрать необходимый стиль отображения выбранных кривых и объектов. Если файл стиля `.gds`
не доступен, его загрузка не выполняется; если файл повреждён, его загрузка выполняется настолько, насколько это возможно.

Далее пользователю предлагается выбрать, каким образом следует интерпретировать загруженный стиль:
- *Применить к выделенным кривым и объектам*. В этом случае номера столбцов данных, сохранённые в файле стиля, игнорируются, а стиль
применяется ко всем выделенным кривым и объектам в порядке прямой нумерации.
- *Добавить на диаграмму кривые, заявленные в файле стиля*. В этом случае, если указанные в файле стиля столбцы данных доступны, будут добавлены
соответствующие кривые, и уже к ним будут применены содержащиеся в файле стили.

В стандартную комплектацию программы файлы стилей не входят.

[:arrow_double_up:](#contents)

---

## 3.2.8. «Сохранить стиль»

Вызывает стандартное окно сохранения файла, позволяющее выбрать необходимое местоположение и имя для нового файла стиля. В файл сохраняются
все параметры в том состоянии, в котором они находились на момент выбора данного пункта меню? применительно ко всем выбранным в данный момент
кривым и объектам. Это позволяет создавать как стили отображения отдельных зависимостей, так и стили для представления групп файлов данных
одинакового формата.

[:arrow_double_up:](#contents)

---

## 3.2.9. «Сбросить стиль»

Пункт меню возвращает все параметры стиля всех выбранных кривых и объектов, кроме границ построения, на стандартные значения
(see section [“Limits and default parameter values”](#limits-and-default-parameter-values) of this guide).

[:arrow_double_up:](#contents)

---

## 3.2.10. «Сохранить шаблон добавления кривых»

Позволяет сохранить шаблон добавления кривых на диаграмму, который затем может быть использован в функции
[параметрического добавления кривых](#parametric-curves-addition).
В файл записываются базовые параметры для всех кривых, добавленных на диаграмму к моменту вызова функции.

[:arrow_double_up:](#contents)

---

## 3.2.11. «Заменить шаблон добавления кривых»

Позволяет сохранить шаблон добавления кривых на диаграмму в качестве такового по умолчанию. После выполнения
этой операции все новые файлы данных, не содержащие собственных стилей отображения, будут отображаться согласно
вновь заданному шаблону.

[:arrow_double_up:](#section)

---

## 3.2.12. «Сбросить шаблон добавления кривых»

Позволяет восстановить стандартный шаблон добавления кривых (восемь первых кривых по четыре в линию).

[:arrow_double_up:](#section)

---

## 3.3. «Помощь»

## 3.3.1. «О программе / справка»

Пункт меню позволяет вызвать данное руководство пользователя, а также получить доступ к другим проектам
и ресурсам Лаборатории.

[:arrow_double_up:](#contents)

---

## 3.3.2. Пункт меню с выбором языка интерфейса

Пункт меню позволяет выбрать язык подписей и подсказок интерфейса программы. Язык меняется сразу, без перезапуска программы, после чего сохраняется
в конфигурации приложения.

В текущей версии доступны следующие языки:
- Русский (Россия);
- Английский (США).

[:arrow_double_up:](#contents)

---

## 3.3.3. «Ассоциировать файлы»

Пункт меню позволяет закрепить приложение в качестве стандартного для открытия файлов `.gdd` и заблокировать от ручного редактирования
файлы стилей `.gds`. Благодаря этому файлы диаграмм можно открывать двойным щелчком, без отдельного входа в приложение.

[:arrow_double_up:](#contents)

---

## 3.4. «Дополнительно»

## 3.4.1. «Соединить таблицы»

Эта функция позволяет объединять таблицы данных различных типов в единый файл. Например, если имеются следующие файлы:

```
  A B
1 1 1
2 4 5
3 6 7

  C D E
1 8 9 4
3 2 3 2
```

на их основе можно получить следующие итоговые таблицы: без восстановления пропусков

```
  A B C D E
1 1 1 8 9 4
3 6 7 2 3 2
```
и с восстановлением пропусков

```
  A B C D E
1 1 1 8 9 4
2 4 5 0 0 0
3 6 7 2 3 2
```
Результат будет зависет от выбранных настроек.

[:arrow_double_up:](#contents)

---

## 3.4.2. «Создать векторный рисунок»

Этот инструмент позволяет генерировать векторное изображение (`SVG` или `EMF`), используя файл скрипта
с настраиваемыми параметрами кривых, осей и текстовых меток. С описанием формата скрипта можно ознакомиться
[здесь](https://github.com/adslbarxatov/GeomagDataDrawer/tree/master/VIGScripts) или путём сохранения
образца из окна данной функции.

[:arrow_double_up:](#contents)

---

# 4. Аппаратно-программные требования и комплектация программы

## 4.1. Комплектация программы

В стандартную комплектацию программы входят следующие файлы:
- `GeomagDataDrawer.exe` – исполняемый модуль программы;
- `ExcelDataReader.dll`, `ExcelDataReader.DataSet.dll`, `ICSharpCode.SharpZipLib.dll` – файлы библиотек, обеспечивающих поддержку формата
данных Microsoft Office Excel. Приложение может функционировать без поддержки этого формата.

[:arrow_double_up:](#contents)

---

## 4.2. Создаваемые файлы и директории

Программа может создавать следующие файлы и директории:
- `.gds` – файлы стилей отдельных кривых диаграммы;
- `.png`, `.svg`, `.emf` – файлы изображений диаграмм;
- `.gdd`, `*.*`, `.csv` – файлы данных, сохранённые программой;
- `Backup.gdd` – файл данных для автоматического восстановления состояния программы;
- `GeomagDataDrawer.txt` – файл шаблона отображения открываемых файлов данных, содержащий описание автоматически добавляемых кривых;
- `Markers` – директория для хранения дополнительных маркеров.

[:arrow_double_up:](#contents)

---

## 4.3. Поддерживаемые форматы данных

Программа поддерживает следующие форматы:
- `.xls`, `.xlsx` – файлы таблиц Microsoft Office Excel ’97 и ’07;
- `.csv` – файлы табличных данных;
- `*.*` – файлы произвольных табличных данных или текстовые файлы с доступными для извлечения данными;
- `.gdd` – файлы данных Geomag data drawer;
- `.png` – файлы изображений маркеров.

[:arrow_double_up:](#contents)

---

## 4.4. Поддержка форматов Microsoft Office Excel

Поддержка файлов таблиц Microsoft Office Excel ’97 и ’07 выполняется с помощью библиотеки ExcelDataReader версии 3.4 
(библиотеки `ExcelDataReader.dll`, `ExcelDataReader.DataSet.dll`) и вспомогательной библиотеки, предоставляющей функционал архивирования файлов
для поддержки формата MS Excel ’07 версии 0.86.0.518 (библиотека `ICSharpCode.SharpZipLib.dll`).

Обе библиотеки распространяются на основе лицензии МТИ (MIT license). Ниже приведён её текст ([оригинал](https://opensource.org/licenses/mit-license.php)).

> Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the «Software»), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
> 1. The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
> The software is provided «as is», without warranty of any kind, express or implied, including but not limited to the warranties of merchantability, fitness for a particular purpose and noninfringement. In no event shall the authors or copyright holders be liable for any claim, damages or other liability, whether in an action of contract, tort or otherwise, arising from, out of or in connection with the software or the use or other dealings in the software.

Перевод:

> Данная лицензия разрешает лицам, получившим копию данного программного обеспечения и сопутствующей документации (в дальнейшем именуемыми «Программное Обеспечение»), безвозмездно использовать Программное Обеспечение без ограничений, включая неограниченное право на использование, копирование, изменение, добавление, публикацию, распространение, сублицензирование и/или продажу копий Программного Обеспечения, а также лицам, которым предоставляется данное Программное Обеспечение, при соблюдении следующих условий:
> 1. Указанное выше уведомление об авторском праве и данные условия должны быть включены во все копии или значимые части данного Программного Обеспечения.
> Данное программное обеспечение предоставляется «как есть», без каких-либо гарантий, явно выраженных или подразумеваемых, включая гарантии товарной пригодности, соответствия по его конкретному назначению и отсутствия нарушений, но не ограничиваясь ими. Ни в каком случае авторы или правообладатели не несут ответственности по каким-либо искам, за ущерб или по иным требованиям, в том числе, при действии контракта, деликте или иной ситуации, возникшим из-за использования программного обеспечения или иных действий с программным обеспечением.

Комплект библиотек доступен [здесь](https://github.com/ExcelDataReader/ExcelDataReader). Код библиотек не включён в исполняемый файл (находится в библиотеках).
Библиотеки, включённые в комплектацию, не модифицировались при разработке приложения.

Включив текст лицензии MIT в файл справки, а также не использовав методы защиты исходного кода программы от реинжиниринга (код не обфусцирован и не упакован;
доступен для реинжиниринга с помощью бесплатных программных средств, что проверено на примере ILSpy; извлекаемый код информативен), считаем требования лицензий
библиотек, включённых в сборку, ***выполненными***.

[:arrow_double_up:](#contents)

---

## 4.5. Требования к установке программы

Обязательные требования к установке программы:
- Доступная оперативная память – не менее 256 Мб;
- Доступное дисковое пространство – не менее 20 Мб;
- Установленная среда .NET Framework версии 4.8.

Дополнительные требования к установке программы:
- Наличие свободного места для хранения изображений диаграмм и сохраняемых файлов данных;
- Наличие видеокарты, способной к достаточно быстрой обработке быстро сменяющихся изображений;
- Наличие процессора, способного к достаточно быстрой обработке объёмов данных, предоставляемых для построения диаграмм, и соответствующего объёма оперативной памяти;
- Наличие монитора, поддерживающего достаточно большое разрешение.

[:arrow_double_up:](#contents)

---

## 4.6. Работа с программой из командной строки

Варианты использования программы из командной строки:
- `GeomagDataDrawer` – запуск программы;
- `GeomagDataDrawer /?` – отображение сообщения с вариантами использования программы из командной строки;
- `GeomagDataDrawer <имя_файла>` – загрузка файла «имя_файла»;
- `GeomagDataDrawer <имя_файла> <имя_файла_2> [SLC] [ECC]` – преобразование файлов из первого формата во второй. В качестве конечных файлов
допускается указание поддерживаемых типов изображений; приложение будет опираться на указанные расширения файлов при преобразовании.
    - `[SLC]` – количество строк, используемых для поиска имён столбцов; если не указан, считывается из конфигурации приложения;
    - `[ECC]` – ожидаемое количество столбцов данных (для извлечения); если не указан, считывается из конфигурации приложения.

[:arrow_double_up:](#contents)

---

# 5. Limits and default parameter values

> Все значения указаны только в отношении данной версии программы. Звёздочкой помечены параметры, ограничения и значения которых
> пересчитываются с учётом масштаба при сохранении конечного изображения

| Параметр | Минимум | Максимум | Значение по умолчанию |
| -------- | ------- | -------- | --------------------- |
| Количество строк данных | 2 | 10001 | — |
| Количество столбцов данных | 2 | 100 | — |
| Количество кривых на диаграмме | 0 | 20 | — |
| Количество графических объектов на диаграмме | 0 | 50 | — |
| Число основных делений на оси Ox | 1 | 100 | Автоопределение |
| Число дополнительных делений на оси Ox | 1 | 10 | 2 |
| Число основных делений на оси Oy | 1 | 100 | Автоопределение |
| Число дополнительных делений на оси Oy | 1 | 10 | 2 |
| Толщина осей/засечек, px* | 1 | 10 | 1 |
| Цвет осей/засечек | — | — | Чёрный |
| Расположение оси Ox | — | — | Автоматически |
| Расположение оси Oy | — | — | Автоматически |
| Формат подписей оси Ox | — | — | Нормальный |
| Формат подписей оси Oy | — | — | Нормальный |
| Толщина линий сетки, px* | 1 | 10 | 1 |
| Цвет основных линий сетки | — | — | Белый (фон) |
| Цвет дополнительных линий сетки | — | — | Белый (фон) |
| Длина подписи кривой | 0 | 200 | — |
| Шрифт подписи кривой | — | — | Arial |
| Кегль подписи кривой* | 4 | 36 | 9 |
| Стиль подписи кривой | — | — | Обычный |
| Цвет подписи кривой | — | — | Чёрный |
| Шрифт подписей осей | — | — | Arial |
| Кегль подписей осей* | 4 | 36 | 8 |
| Стиль подписей осей | — | — | Обычный |
| Цвет подписей осей | — | — | Чёрный |
| Толщина кривых, px* | 1 | 10 | 1 |
| Стиль отображения кривой | — | — | Линия без маркеров |
| Маркер кривой | 1 | 6 + число успешно загруженных изображений маркеров | 1 (квадратик) |
| Размеры изображения дополнительного маркера, px | 3 × 3 | 17 × 17 | — |
| Размеры рисунка одной кривой, размер области диаграммы, размер сохраняемого изображения, px* | 100 × 100 | 10000 × 10000 | 500 × 500 |
| Смещение рисунка кривой от верхнего левого края изображения, px* | (0, 0) | (9900, 9900) | (0, 0) |
| Масштаб сохраняемого изображения | 1,0 | 10,0 | 1,0 |

[:arrow_double_up:](#contents)

---

# 6. File format specifications

## 6.1. Файлы данных Geomag data drawer

Спецификация формата `.gdd` на данный момент недоступна к публикации. Не рекомендуется самостоятельное внесение изменений в файлы
данных Geomag data drawer во избежание потерь данных.

[:arrow_double_up:](#contents)

---

## 6.2. Файлы данных в формате Microsoft Office Excel

В файле `.xls` или `.xlsx` должна храниться одна таблица. Пустые поля, пропуски и иные элементы будут замещены нулями.
Таблица должна располагаться на первом листе файла. Допускаются текстовые пояснения в первых строках файла. Допускается использование
формул без зависимостей, для разрешения которых необходим запуск Microsoft Office Excel. Допускается использование форматирования листа.

[:arrow_double_up:](#contents)

---

## 6.3. Файлы данных в формате Windows CSV

Требуется одинаковое количество числовых значений в строках файла `.csv`. Количество подписей должно совпадать с числом столбцов данных,
иначе они будут проигнорированы. Значения должны быть разделены точкой с запятой (`;`) в любых количествах, но не менее одного. Десятичным
признаком может быть точка (`.`) или запятая (`,`). При сохранении файла программа использует запятую. Допускается обычное
и экспоненциальное представление чисел с дробной частью и без неё (с символами `E` и `e`). Абзацы и посторонние символы будут замещены
нулевыми значениями. Используется кодовая страница Windows1251.

Пример содержимого файла в формате Windows CSV:

```
Ст.1;Ст.2;Ст.3
1;10;40,9
2;20,1;4.39e-01
3;0,9;4,33E-1
```

[:arrow_double_up:](#contents)

---

## 6.4. Файлы, из которых необходимо извлечь данные

Используется кодовая страница Windows1251. Символы `0` – `9`, `-`, латинские `e` и `E`, `.` и `,` считаются элементами
числовых значений. Все остальные символы используются в качестве разделителей. Наборы допустимых символов, которые не являются числами, интерпретируются
как нулевые значения. Нулевыми значениями замещаются также недостающие значения в строках, если при загрузке файла было задано число столбцов,
превышающее имеющееся в файле.

Если в строке нет числовых значений, она игнорируется. Если же в строке есть хоть один допустимый символ, она загружается программой. В связи с этим
перед загрузкой файла настоятельно рекомендуется удалить из него все строки, которые могут быть интерпретированы подобным образом, или воспользоваться
опцией поиска названий столбцов. В противном случае на диаграмме могут быть отображены некорректные (с точки зрения её смысла) показания, а диапазон
её построения, рассчитанный с учётом таких данных, может сильно отличаться от требуемого.

Пример содержимого файла и его интерпретации:

| Исходные данные | Данные после удаления разделителей | Результат |
| - | - | - |
| 1 10 40.9 | 1 10 40.9 | 1,0; 10,0; 40,9 |
| abcde1 1*0 40,9 | e1 1 0 40,9 | 0,0; 1,0; 0,0; 40,9 |
| -10 +40.9 -4,9e-1 | -10 40.9 -4,9e-1 | -10,0; 40,9; -0,49 |

[:arrow_double_up:](#contents)
