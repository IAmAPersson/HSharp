#H# README

##Current Version: 1.3

##Using the .exe to run H# programs:

There are two ways you can use HSCi.exe. The first is an an interactive console. Simply open the file and it will act as the interactive console. Type in your program line-by-line and the console will execute it. It's also a great way to test quick code ideas before you implement them. Note that the interactive console does not require (or even recognize) semicolon line breakers.

In the interactive console, any remaining value of a line is directed onto the console screen. For example, inputting "2+2" directly will evaluate to 4. However, since you don't do anything with it, it is directed onto the console screen as output. By using the interactive console this way, you can treat H# as a nice calculator. However, if you do "let x=2+2" the value "2+2" is assigned to "x". A "let" statement has no return value, so you get no output.

Alternatively, if you use a program editor like Notepad++ and save your program as a file (typically .hsh) and click "Open file with"->"HSCi.exe" the interpreter will run your multiline program. Note that semicolons are required as line breakers, and newlines in your program are ignored (even for ending a comment, hence why the NPP highlighting has // for a "begin comment" and ; for the "end comment").

##Using the npphighlighting.xml:

The .xml is the syntax highlighting document for NPP, if you decide to use that as your main editor. If you do not have a "userDefineLang.xml" inside your NPP file folder, then simply move "npphighlighting.xml" to the root of your NPP files and rename it to "userDefineLang". If you already have a "userDefineLang", then you'll copy everything inside the <UserLang> tag in the "npphighlighting" and paste it inside the <NotepadPlus> tag in the "userDefineLang". If you have problems, there are detailed tutorials on the internet explaining how to use NPP's .xml files.

##Source code

SOURCE.cs is the full source code for H#. You may edit and distribute the code according to the terms of the LICENSE. If you ever need to fix a bug, add on to H#, or just want to take a peek, SOURCE.cs is always there. All of the source code is contained inside that one file. You can recompile into a single .exe file, or you can break it up into different files as shown by the comments I put in there.

##Tutorial

HSharp Tutorial.txt is a work-in-progress full tutorial for H#. While it is not 100% complete, it covers most of what you'll need to get started. Later on, I'll reformat it and upload it to the website at http://iamapersson.github.io/hsharp/
