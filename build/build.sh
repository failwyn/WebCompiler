#!/bin/bash

#:: IMPORTANT!! npm 3.x is required to avoid long path exceptions

#// TODO: GH: replace this with linux version
#if exist "../src/WebCompiler/node/node_modules.7z" goto:EOF

mkdir ../src/WebCompiler/node

pushd ../src/WebCompiler/node

#        node-sass \ // TODO: GH: is this really needed? it fails to install on OS X
echo Installing packages...
npm install --quiet \
        babel@5.8.34 \
        iced-coffee-script \
        less \
        less-plugin-autoprefix \
        less-plugin-csscomb \
        sass \
        postcss@latest \
        postcss-cli \
        autoprefixer \
        stylus \
        handlebars \
        > /dev/null
npm install --quiet > /dev/null

#if not exist "node_modules/node-sass/vendor/win32-ia32-48" (
#    echo Copying node binding...
#    md "node_modules/node-sass/vendor/win32-ia32-48"
#    copy binding.node "node_modules/node-sass/vendor/win32-ia32-48"
#)

echo Deleting unneeded files and folders...
rm -rf *.html > /dev/null
rm -rf *.markdown > /dev/null
rm -rf *.md > /dev/null
rm -rf *.npmignore > /dev/null
rm -rf *.patch > /dev/null
rm -rf *.txt > /dev/null
rm -rf *.yml > /dev/null
rm -rf .editorconfig > /dev/null
rm -rf .eslintrc > /dev/null
rm -rf .gitattributes > /dev/null
rm -rf .jscsrc > /dev/null
rm -rf .jshintrc > /dev/null
rm -rf CHANGELOG > /dev/null
rm -rf CNAME > /dev/null
rm -rf example.js > /dev/null
rm -rf generate-* > /dev/null
rm -rf gruntfile.js > /dev/null
rm -rf gulpfile.* > /dev/null
rm -rf makefile.* > /dev/null
rm -rf README > /dev/null

# TODO: limux version of the below
#for /d /r . %%d in (benchmark)  do @if exist "%%d" rd /s /q "%%d" > /dev/null
#for /d /r . %%d in (bench)      do @if exist "%%d" rd /s /q "%%d" > /dev/null
#for /d /r . %%d in (doc)        do @if exist "%%d" rd /s /q "%%d" > /dev/null
#for /d /r . %%d in (docs)       do @if exist "%%d" rd /s /q "%%d" > /dev/null
#for /d /r . %%d in (example)    do @if exist "%%d" rd /s /q "%%d" > /dev/null
#for /d /r . %%d in (examples)   do @if exist "%%d" rd /s /q "%%d" > /dev/null
#for /d /r . %%d in (images)     do @if exist "%%d" rd /s /q "%%d" > /dev/null
#for /d /r . %%d in (man)        do @if exist "%%d" rd /s /q "%%d" > /dev/null
#for /d /r . %%d in (media)      do @if exist "%%d" rd /s /q "%%d" > /dev/null
#for /d /r . %%d in (scripts)    do @if exist "%%d" rd /s /q "%%d" > /dev/null
#for /d /r . %%d in (test)       do @if exist "%%d" rd /s /q "%%d" > /dev/null
#for /d /r . %%d in (tests)      do @if exist "%%d" rd /s /q "%%d" > /dev/null
#for /d /r . %%d in (testing)    do @if exist "%%d" rd /s /q "%%d" > /dev/null
#for /d /r . %%d in (tst)        do @if exist "%%d" rd /s /q "%%d" > /dev/null

echo Compressing artifacts and cleans up...
rm node_modules.7z
7z a -r -mx9 node_modules.7z node_modules > /dev/null
rm -rf node_modules > /dev/null
rm package.json > /dev/null

#:done
echo Done
popd