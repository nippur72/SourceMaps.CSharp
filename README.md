# CSharp.SourceMappings

C# classes for handling javascript Source Maps (.map files for in-browser javascript debugging)

Copied and converted from "dart2js.dart" source code (Dart language SDK).

## How to use the example

This example demostrates the use of map files over a fictional language "UPPECARSE-JAVASCRIPT"
where the source files are made of Javascript written all upercase. 

The test program "compiles" (merges) `source1.txt` + `source2.txt` and produces `myapp.js` and `myapp.js.map` as result.

To run the example:

- clone this repo
- open in visual studio
- run the `TestExample` project. This will create `myapp.js` and `myapp.js.map` files
- place some breakpoints in `source1.txt` or `source2.txt`
- run the `Website` project in visual studio with IE browser
- see the debugging in action

## Some notes on the working of Source maps

- Source maps works better if the granularity is set at token level (punctuation included)
- If a coarse granularity is used, the highlight selection will span over, reaching the start of the next map
- A granularity at character level is possible too, e.g. mapping every single character, 
  this seem to work and to be handled ok by VS and the browsers
- If there are more map points than necessary, both browsers and VS will "intersect" them with
  legal points in the javascript code
- VS uses maps to make debugging at istruction level, while browser work at "start of line" level
- If there are two istructions on the same line, VS will handle it, browsers instead will consider just one breakpoint for the line
- The names specified in the maps are used only for callstack tracing, so only names for function calls should be specified
- Variable names are useless, Visual Studio doesn't recognize them (hovering the mouse over the variable name does not produce the tooltip)






