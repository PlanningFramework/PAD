
## Project PAD.InputData

### 1. License

Project PAD.InputData (a part of PAD Framework) 
Copyright (C) 2020 Tomas Hurt
  
This program is free software: you can redistribute it and/or modify  
it under the terms of the GNU General Public License as published by  
the Free Software Foundation, either version 3 of the License, or  
(at your option) any later version.  
  
This program is distributed in the hope that it will be useful,  
but WITHOUT ANY WARRANTY; without even the implied warranty of  
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the  
GNU General Public License for more details.  
  
You should have received a copy of the GNU General Public License  
along with this program.  If not, see <http://www.gnu.org/licenses/>.

### 2. Description

* This project is a part of PAD Framework and provides loading and validation tools for PDDL 3.1 a SAS+ input formalisms.
* The result of the loading is PDDLInputData or SASInputData objects.
* To specifically utilize the input loader, use PDDLInputDataLoader or SASInputLoader class.
* To specifically utilize the validator, use PDDLInputDataValidator or SASInputDataValidator class.

### 3. Examples

Loading the data is very simple:

    var inputDataPDDL = new PAD.InputData.PDDLInputData("domain.pddl", "problem.pddl");
    var inputDataSAS = new PAD.InputData.SASInputData("problem.sas");

If you want to omit the validation, use the extra parameter in constructor:

    var inputDataWithoutValidation = new PAD.InputData.SASInputData("problem.sas", false);
