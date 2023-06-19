#!/bin/bash

7z x -y node.7z node
rm node.7z

7z x -y node_modules.7z
rm node_modules.7z

rm 7z.exe
rm 7z.dll
rm prepare.cmd
rm prepare.sh
