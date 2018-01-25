# SCORM Resource Validator
A new version of the SCORM Package Resource Validator. 
The application takes a SCORM PIF (`.zip`) and generates log files based on the following criteria:
* Files in the PIF
* Files referenced in the manifest (`imsmanifest.xml`)
* Files referenced in the manifest but not found in the PIF
* Files found in the PIF but not referenced in the manifest

Authors of distributed learning products for the U.S. Army use the application as one step in the overall process for DL validation.

## Installation
Clone this repo and open the Visual Studio solution:

`https://github.com/nicksorrell/SCORMResourceValidator`

## Usage
1. Build the solution to generate and run the executable.
2. Select the **Browse** button and choose a SCORM PIF to validate.
3. Select the **Validate** button.

Four log files will be saved in a timestamped folder in the `logs` folder located in the same directory as the application.

Example output files:

`logs\RV_170208_025854_SCORM2004_CP\manifest_files_found.html
logs\RV_170208_025854_SCORM2004_CP\manifest_files_missing.html
logs\RV_170208_025854_SCORM2004_CP\packaged_files_found.html
logs\RV_170208_025854_SCORM2004_CP\packaged_files_missing.html`

The logs may then be used in other steps of the validation process.

## Credits and Ownership
Produced in collaboration with [Ken Thomann](https://twitter.com/KenThomann).

This application is property of [JANUS Research Group, Inc.](http://janusresearch.com/), © 2017
