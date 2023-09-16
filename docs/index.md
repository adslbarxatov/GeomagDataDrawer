# Geomag data drawer: complete user guide
> **ƒ** &nbsp;Nicolay B. aka RD_AAOW, Соля́ников Я.; 17.09.2023; 0:10

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
    - [“Select curve’s data columns”](#select-curves-data-columns)
    - [“Merge curves or objects”](#merge-curves-or-objects)
    - [“Remove curves or objects”](#remove-curves-or-objects)
    - [“Edit diagram data”](#edit-diagram-data)
    - [“Load style”](#load-style)
    - [“Save style”](#save-style)
    - [“Reset style”](#reset-style)
    - [“Save curves addition template”](#save-curves-addition-template)
    - [“Replace preview template”](#replace-preview-template)
    - [“Restore preview template”](#restore-preview-template)
    - Additional
    - [“Merge data tables”](#merge-data-tables)
    - [“Create vector image”](#create-vector-image)
    - Help
    - [“About / Help”](#about--help)
    - [Menu item with interface language selection](#menu-item-with-interface-language-selection)
    - [“Associate files”](#associate-files)
- [Hardware and software requirements and app contents](#hardware-and-software-requirements-and-app-contents)
    - App contents
    - [Files and directories](#files-and-directories)
    - [Supported file formats](#supported-file-formats)
    - [Support for Microsoft Office Excel formats](#support-for-microsoft-office-excel-formats)
    - [Working with the app from the command line](#working-with-the-app-from-the-command-line)
- [Limits and default parameter values](#limits-and-default-parameter-values)
- [File format specifications](#file-format-specifications)
    - Geomag data drawer files
    - [Microsoft Office Excel data files](#microsoft-office-excel-data-files)
    - [Data files in Windows CSV format](#data-files-in-windows-csv-format)
    - [Files to extract data from](#files-to-extract-data-from)
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

Finally, if work on the diagram hasn’t been completed, all data and settings can be saved to the app’s own file format,
and then restored at the next start. This function is duplicated by the *function of autosave (when closed)
and auto-recovery (when opened)* of the app state.

[:arrow_double_up:](#contents)

---

## 2.1. Adding a curve or object to a diagram

To add a new curve or graphical object to the diagram, click the <img src="/GeomagDataDrawer/img/2_70.png" />
button next to the list of curves and objects. A hotkey is available for this function, see section “[App menu](#app-menu)”
of this guide. This will open the addition options window:

<img src="/GeomagDataDrawer/img/2_03_en.png" />

When adding a curve, you’ll need to specify which column from the loaded data file will be *the abscissa column*
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
    - Merge data tables;
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

## 3.2.3. “Select curve’s data columns”

The menu item allows you to change the data columns used to plot the selected curve. This will open a window similar
to the one that is called up when simply adding a curve. Here you will need to indicate which column from the data file
will be the new abscissa column, and which will be the ordinate column in the selected relation. This option can be useful
if you need to quickly create a new curve in place of an existing one, using the style settings and the location of the latter.

[:arrow_double_up:](#contents)

---

## 3.2.4. “Merge curves or objects”

The menu item allows you to combine two curves, i.e. place them on top of each other and optionally set common axes.
The function requires that exactly two curves be selected before calling it; otherwise the alignment will not be performed.

The curve alignment parameters window is shown in the figure below.

<img src="/GeomagDataDrawer/img/3_63_en.png" />

User can specify where the alignment result should be placed at which of the curves. You can also choose which axis
is shared between two curves, and whether to set the color of the axis labels and curve labels to the color of
the curve itself to enhance the visualization of the diagram.

When combining, the common axes of the curves are assigned the same parameters (maximum and minimum values,
number of ticks, and location), and different axes are placed along the edges of the curve image. The dimensions of the curve
images, the value of the curve transposition parameter, and the axes parameters also become common; placement of curve labels
is performed automatically.

This function can also be applied to graphic objects, if necessary. Please note that the procedure is not irreversible,
because it just changes the location and display styles of curves on the diagram.

[:arrow_double_up:](#contents)

---

## 3.2.5. “Remove curves or objects”

The menu item performs functions similar to those described in
[clause 2.2](#removing-curves-and-objects-from-diagram) of this guide.

[:arrow_double_up:](#contents)

---

## 3.2.6. “Edit diagram data”

The menu item opens a window for editing diagram data:

<img src="/GeomagDataDrawer/img/3_40_en.png" />

> ***This option may work slowly and freeze the app interface for a while when called.
> This is due to the peculiarities of the editor’s operating mechanisms and is not a failure or error***

The data in the table is presented as it was obtained from the data file. To edit each individual value, simply select
the corresponding cell and press the `[F2]` key or double-click it. Fractional values must be entered with the decimal
separator specified in the OS settings. The user is notified of all errors when filling out table cells either immediately
when trying to leave the cell, or when trying to save the changes made.

You can also edit data column names. To do this, just double-click on the header of the column whose name you want to change.

The following operations are available for the table itself:
- Move the currently selected line one position up (button <img src="/GeomagDataDrawer/img/3_41.png" />).
Hereinafter, the last selected line is used as the current line.
- Move the currently selected line one position down (button <img src="/GeomagDataDrawer/img/3_42.png" />).
- Add a line before the currently selected one (button <img src="/GeomagDataDrawer/img/3_43.png" />).
- Add a line after the currently selected one (button <img src="/GeomagDataDrawer/img/3_44.png" />).
- Delete selected rows (button <img src="/GeomagDataDrawer/img/3_45.png" />).
All affected rows are deleted. It doesn’t matter in what order columns are selected.

The “Save changes” button, if correct, applies the changes made to the diagram data and initiates its redrawing.
The “Cancel changes” button returns the diagram to its original state.

[:arrow_double_up:](#contents)

---

## 3.2.7. “Load style”

Calls up a standard file opening window, allowing you to select the desired display style for selected curves and objects.
If a `.gds` style file is not available, it’s not loaded; if the file is damaged, it’s loaded as best as possible.

Next, user is asked to choose how to interpret the loaded style:
- *Apply to selected curves and objects*. In this case, the data column numbers stored in the style file are ignored,
and the style is applied to all selected curves and objects in direct numbering order.
- *Add curves specified in the style file to the diagram*. In this case, if the data columns specified in the style
file are available, the corresponding curves will be added, and the styles contained in the file will be applied to them.

Style files aren’t included in the standard package of the application.

[:arrow_double_up:](#contents)

---

## 3.2.8. “Save style”

Calls up the standard file save window, allowing you to select the desired location and name for the new style file.
All parameters are saved to the file in the state in which they were at the time of selecting this menu item in relation
to all currently selected curves and objects. This allows you to create both styles for displaying individual dependencies,
as well as styles for representing groups of data files of the same format.

[:arrow_double_up:](#contents)

---

## 3.2.9. “Reset style”

The menu item returns all style parameters of all selected curves and objects, except for plotting range, to standard values
(see section [“Limits and default parameter values”](#limits-and-default-parameter-values) of this guide).

[:arrow_double_up:](#contents)

---

## 3.2.10. “Save curves addition template”

Allows you to save a template for adding curves to a diagram, which can then be used in a function of
[parametric curves addition](#parametric-curves-addition).
The file records the basic parameters for all curves added to the diagram at the time the option is called.

[:arrow_double_up:](#contents)

---

## 3.2.11. “Replace preview template”

Allows you to save the template for adding curves to a diagram as the default one. After this operation is completed,
all new data files that don’t contain their own display styles will be displayed according to the newly specified template.

[:arrow_double_up:](#section)

---

## 3.2.12. “Restore preview template”

Allows you to restore the standard template for adding curves (the first eight curves, four in a line).

[:arrow_double_up:](#section)

---

## 3.3. “Additional”

## 3.3.1. “Merge data tables”

This feature allows you to combine data tables of different types into a single file. For example, if you have the following files:

```
  A B
1 1 1
2 4 5
3 6 7

  C D E
1 8 9 4
3 2 3 2
```

based on them, you can obtain the following summary tables: *without restoring gaps*

```
  A B C D E
1 1 1 8 9 4
3 6 7 2 3 2
```
and *with the restoration of gaps*

```
  A B C D E
1 1 1 8 9 4
2 4 5 0 0 0
3 6 7 2 3 2
```
The result will depend on the selected settings.

[:arrow_double_up:](#contents)

---

## 3.3.2. “Create vector image”

This tool allows you to generate a vector image (`SVG` or `EMF`) using a script file with customizable parameters for curves,
axes and text labels. A description of the script format can be found
[here](https://github.com/adslbarxatov/GeomagDataDrawer/tree/master/VIGScripts) or by saving a sample from the window
of this function.

[:arrow_double_up:](#contents)

---

## 3.4. “Help”

## 3.4.1. “About / Help”

The menu item allows you to call up this user manual, as well as access other projects and resources of the Laboratory.

[:arrow_double_up:](#contents)

---

## 3.4.2. Menu item with interface language selection

The menu item allows you to select the language of captions and tooltips of the app interface. The language changes
immediately, without restarting the app, and is then saved in the app configuration.

The following languages are available in the current version:
- Russian (Russia);
- English (USA).

[:arrow_double_up:](#contents)

---

## 3.4.3. “Associate files”

The menu item allows you to set the app as the default one for opening `.gdd` files and block `.gds` style files from manual
editing. Thanks to this, diagram files can be opened by double-clicking, without having to entering the app separately.

[:arrow_double_up:](#contents)

---

# 4. Hardware and software requirements and app contents

## 4.1. App contents

The standard package of the app includes the following files:
- `GeomagDataDrawer.exe` – executable module;
- `ExcelDataReader.dll`, `ExcelDataReader.DataSet.dll`, `ICSharpCode.SharpZipLib.dll` – library files that provide support
for the Microsoft Office Excel data format. The app can function without support for this format (without these libraries).

[:arrow_double_up:](#contents)

---

## 4.2. Files and directories

The app can create the following files and directories:
- `.gds` – files of styles of individual diagram curves;
- `.png`, `.svg`, `.emf` – diagram image files;
- `.gdd`, `*.*`, `.csv` – data files saved by the app;
- `Backup.gdd` – data file for automatically restoring the app state;
- `GeomagDataDrawer.txt` – a template file for displaying opened data files, containing a description of automatically added curves;
- `Markers` – directory for storing additional markers.

[:arrow_double_up:](#contents)

---

## 4.3. Supported file formats

The app supports following formats:
- `.xls`, `.xlsx` – Microsoft Office Excel ’97 and ’07 table files;
- `.csv` – tabular data files;
- `*.*` – files of arbitrary tabular data or text files with data available for extraction;
- `.gdd` – Geomag data drawer data files;
- `.png` – marker image files.

[:arrow_double_up:](#contents)

---

## 4.4. Support for Microsoft Office Excel formats

Support for Microsoft Office Excel ’97 and ’07 table files is performed using the ExcelDataReader library
(`ExcelDataReader.dll` and `ExcelDataReader.DataSet.dll`, version 3.6) and an auxiliary library that provides file archiving
functionality to support the MS Excel ’07 format (library `ICSharpCode.SharpZipLib.dll`, version 0.86.0.518).

Both libraries are distributed under the MIT license through the **nuget** system. Below is its text ([original](https://opensource.org/license/mit)).

> Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the «Software»), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
> 1. The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
> The software is provided «as is», without warranty of any kind, express or implied, including but not limited to the warranties of merchantability, fitness for a particular purpose and noninfringement. In no event shall the authors or copyright holders be liable for any claim, damages or other liability, whether in an action of contract, tort or otherwise, arising from, out of or in connection with the software or the use or other dealings in the software.

App can work without these libraries, but loading of files `.xls` and `.xlsx` will be unavailable.

[:arrow_double_up:](#contents)

---

## 4.5. Working with the app from the command line

Options for using the app from the command line:
- `GeomagDataDrawer` – run the app;
- `GeomagDataDrawer /?` – displays a message with options for using the app from the command line;
- `GeomagDataDrawer <file_name>` – loading the file “file_name”;
- `GeomagDataDrawer <file_name> <file_name_2> [SLC] [ECC]` – converting files from the first format to the second. As final files
it’s possible to indicate supported image types; the app will rely on the specified file extensions when converting.
     - `[SLC]` – number of lines used to search for column names; if not specified, read from the app configuration;
     - `[ECC]` – expected number of data columns (to be extracted); if not specified, read from the app configuration.

[:arrow_double_up:](#contents)

---

# 5. Limits and default parameter values

> All values are specified only for this version of the app. An asterisk marks parameters whose restrictions
> and values are recalculated taking into account the scale when saving the final image

| Parameter | Minimum | Maximum | Default value |
| -------- | ------- | -------- | --------------------- |
| Data rows quantity | 2 | 10001 | — |
| Data columns quantity | 2 | 100 | — |
| Quantity of curves on the diagram | 0 | 20 | — |
| Quantity of graphic object on the diagram | 0 | 50 | — |
| Quantity of primary notches on Ox axe | 1 | 100 | Autodetection |
| Quantity of secondary notches on Ox axe | 1 | 10 | 2 |
| Quantity of primary notches on Oy axe | 1 | 100 | Autodetection |
| Quantity of secondary notches on Oy axe | 1 | 10 | 2 |
| Width of axes / notches, px* | 1 | 10 | 1 |
| Color of axes / notches | — | — | Black |
| Placement of Ox axe | — | — | Auto |
| Placement of Oy axe | — | — | Auto |
| Format of numbers on Ox axe | — | — | Generic |
| Format of numbers on Oy axe | — | — | Generic |
| Width of grid lines, px* | 1 | 10 | 1 |
| Color of primary grid lines | — | — | White (background) |
| Color of secondary grid lines | — | — | White (background) |
| Width of curve’s caption | 0 | 200 | — |
| Font of curve’s caption | — | — | Arial |
| Font size of curve’s caption* | 4 | 36 | 9 |
| Style of curve’s caption | — | — | Default |
| Color of curve’s caption | — | — | Black |
| Font of axe’s caption | — | — | Arial |
| Font size of axe’s caption* | 4 | 36 | 8 |
| Style of axe’s caption | — | — | Default |
| Color of axe’s caption | — | — | Color |
| Width of curve’s line, px* | 1 | 10 | 1 |
| Style of curve | — | — | Line without markers |
| Curve’s marker | 1 | 6 + number of successfully loaded marker images | 1 (square) |
| Size of image of additional marker, px | 3 × 3 | 17 × 17 | — |
| Dimensions of image of single curve, size of the diagram area, size of the saved image, px* | 100 × 100 | 10000 × 10000 | 500 × 500 |
| Offset the curve image from the top left edge of the diagram, px* | (0, 0) | (9900, 9900) | (0, 0) |
| Scale of the saved image | 1,0 | 10,0 | 1,0 |

[:arrow_double_up:](#contents)

---

# 6. File format specifications

## 6.1. Geomag data drawer files

The `.gdd` format specification is not currently available for publication. It’s not recommended to make changes
to Geomag data drawer data files yourself to avoid data loss.

[:arrow_double_up:](#contents)

---

## 6.2. Microsoft Office Excel data files

A `.xls` or `.xlsx` file must contain one table. Empty fields, gaps and other elements will be replaced with zeros.
The table must be located on the first sheet of the file. Text explanations are allowed in the first lines of the file.
You can use formulas without dependencies that require running Microsoft Office Excel to resolve. You can use sheet formatting.

[:arrow_double_up:](#contents)

---

## 6.3. Data files in Windows CSV format

The same number of numeric values are required across the lines of the `.csv` file. The number of captions must match
the number of data columns, otherwise they will be ignored. Values must be separated by semicolons (`;`) in any number,
but not less than one. The decimal character can be a period (`.`) or a comma (`,`). When saving a file, the app uses
a comma. Conventional and exponential representation of numbers with and without fractional parts (with symbols `E`
and `e`) is allowed. Paragraphs and extraneous characters will be replaced with null values.

Example file contents in Windows CSV format:

```
Col.1;Col.2;Col.3
1;10;40,9
2;20,1;4.39e-01
3;0,9;4,33E-1
```

[:arrow_double_up:](#contents)

---

## 6.4. Files to extract data from

The characters `0` – `9`, `-`, Latin `e` and `E`, `.` and `,` are considered elements of numeric values. All other characters
are used as delimiters. Sets of valid characters that aren’t numbers are interpreted as null values. Zero values also replace
missing values in rows if, when loading the file, the number of columns exceeding those available in the file was specified.

If a line has no numeric values, it is ignored. If the line contains at least one valid character, it is loaded
by the app. Therefore, before loading the file, it’s strongly recommended to remove any lines from it that could be
interpreted in this way, or use the option to search for column names. Otherwise, the diagram may display incorrect
(from the point of view of its meaning) readings, and the range of its plotting, calculated taking into account
such data, may differ greatly from the expected one.

Example of file contents and its interpretation:

| Initial data | Data after removing delimiters | Result |
| - | - | - |
| 1 10 40.9 | 1 10 40.9 | 1,0; 10,0; 40,9 |
| abcde1 1*0 40,9 | e1 1 0 40,9 | 0,0; 1,0; 0,0; 40,9 |
| -10 +40.9 -4,9e-1 | -10 40.9 -4,9e-1 | -10,0; 40,9; -0,49 |

[:arrow_double_up:](#contents)
