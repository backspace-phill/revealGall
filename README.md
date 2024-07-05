# revealGall

This is a Gallery for reveal.js presentations

## What does this do?

First of all it has a Gallery of the presentations in the current directory

Also it does some HTML rewriting to make all of Presentations standalone.
This means you can you open a presentation with this gallery and save it through the webbrowser to have
something you can easily share.

## Technical Stuff

- Currently fully .NET 8 AOT compatible
- Sucks up a lot of system resouces (maybe nothing for your toaster)
- Wanted to write this in F# but sadly its currently not compatible with AOT-Compilation
- It replaces linked stylesheets, sourced script files, and images with the same but inlined or base64 encoded

# LICENSE

See LICENSE