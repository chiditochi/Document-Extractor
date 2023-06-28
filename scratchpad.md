### 
dotnet tool install --global dotnet-ef

- dotnet-ef migrations add Initial
- dotnet-ef database update


###  libman
dotnet tool install -g Microsoft.Web.LibraryManager.Cli
libman --version
libman init

libman install <LIBRARY> [-d|--destination] [--files] [-p|--provider] [--verbosity]
    e.g libman install jquery@3.2.1 --provider cdnjs --destination wwwroot/scripts/jquery --files jquery.min.js
    e.g libman install toastr.js@2.1.4 --provider cdnjs --destination wwwroot/lib/toastr --files toastr.min.js --files toastr.min.css
        libman install C:\temp\contosoCalendar\ --provider filesystem --files calendar.js --files calendar.css

libman clean
libman restore
libman uninstall <LIBRARY> [--verbosity]

-- cdnjs repository

## ef migrations

- dotnet ef migrations add InitialCreate
- dotnet ef database update
  - dotnet ef database update AddNewTables (updates your database to a given migration)
- dotnet ef migrations remove
- dotnet ef migrations list
- dotnet ef migrations script
  - dotnet ef migrations script AddNewTables
  - dotnet ef migrations script AddNewTables AddAuditTable

### Todos 
    - Table 
        - create PatientTemp Table 
        - from Patient model, remove 
            - IsUploadComfirmed
            - Status
    - create a permanent tempfolder 
        - wwwroot/tempFolder
        - aid, add this to the folders automatically profiled during startup 
    - User Action Trigger
        - on Confirmation 
                - copy record to Patient table from PatientTemp table 
                - copy associated files PatientTemp.FileName & PatientTemp.TxtFileName to the configured folders 
        - on Rejection
                - do Cleanup
        - Cleanup
            - remove wwwroot/tempFolder files , aid is the target record 
            - remove db record for that upload 
    - Upload form validation 
        - add file validation
            - accept .pdf and .docx 
        - check 
    
        if (ext == ".docx") result = new WordExtractor();
        if (ext == ".pdf") result = new PdfExtractor();


    - cleanup cancelled uploads 
        - from db 
        - form uploads


## db init 
    - create UserTypes 
        - OpTeam 
        - Patient 
    - create Roles 
        - Admin
        - User
    - create two users 
        - admin user | admin@yahoo.com
        - user | user@yahoo.com
    - create 3 Teams 
        - Code: CodeDescription 
            tm247	AMH City Carers Support Service 
            tm314   AMH Bipolar Service 
            tm361   AMH LMHT Mansfield 
    - create AppConstant 
        - required props | Name, DOB, NHS Number
            RequiredProps:PatientNHS
            RequiredProps:PatientDOB
            RequiredProps:PatientFirstname
            RequiredProps:PatientSurname
        - upload exist prop | DateTime, NHS No
            UploadExistProps:DateTime
            UploadExistProps:PatientNHS


### 
PATIENT FORM CONVERTER (WORD/PDF -> TXT)

SUMMARY
This application allows users to upload a completed patient form (Word or PDF format) into the system. The system reads the file, checks for some mandatory required fields and converts the uploaded file into a txt file.


STEPS/FLOW

1. User Login to the application. Upon successful login, the upload page is displayed

2. On the upload page, user performs the following action:
	i. Click on the file upload field to select a word/pdf document 
	ii. Select the Team/Department (from a dropdown list) they will send the file to
	iii. Click on the upload/submit button

3. Upon submission the system checks to ensure the file has not been previously uploaded. (This is to prevent uploading the same file twice)
  
4. The system reads the uploaded file (word/pdf) and validate to ensure some required fields are there e.g. (Name, DOB, NHS Number) etc.

5. Upon successful validation, the system will display the uploaded patient demographic (patient details in the word/pdf document) information on the screen for the user to confirm

6. Once everything is ok, the users acknowledges and submits.

7.  Upon submission, the system CONVERTS the uploaded file to a TEXT file. The files will be kept in 2 different path which can be configured.
Config paths (app.config)
	PATH-1 will have the word + txt file (rename timestamp with extension)
	PATH-2 will just have the txt file in it.	

NOTE
We need to check if a user has previously loaded a file so they dont load the exact same one twice. in that case, we store the following information 
	i. DateTime (from the txt)
	ii. The NHS Number
That should do, so if any of the same document gets uploaded it will throw an error

8. Finally, the contents from the txt file needs to be saved in the DB.


### 
  -- truncate table dbo.Patients;
  -- DBCC CHECKIDENT('Patients', RESEED, 1);

---------------------------------------------

DateTime
OperatorFirstname
OperatorMiddle 
OperatorSurname 
PatientNHS
PatientTitle
PatientFirstname
PatientMiddle
PatientSurname
PatientDOB
PatientSex
PatientSexCode
PatientHousename
PatientAddress1
PatientAddress2 
PatientAddress3
PatientAddress4 
PatientPostcode
PatientPhoneno
PatientReligion
PatientEthnicity 
PatientPractice 
PatientPracticeAddress 
PatientPracticeCode
PatientGPTitle 
PatientGPFirstName
PatientGPSurname
PatientGPCode


           function getDummyData(){
                return {
                    dateTime: "15/09/2022 16:13",
                    operatorFirstname: "Ahmed", 
                    operatorMiddle:  "",
                    operatorSurname: "Yousafzai", 
                    patientNHS: "999 006 9034", 
                    patientTitle: "Mrs", 
                    patientFirstname: "Ebs-Donotuse", 
                    patientMiddle:  "",
                    patientSurname: "Xxtestpatientaado",
                    patientDOB: "01 May 1944", 
                    patientSex: "Female",
                    patientSexCode: "F",
                    patientHousename: "Co Npfit Test Data Manager",
                    patientAddress1: "Princes Exchange", 
                    patientAddress2:  "",
                    patientAddress3: "Princes Square", 
                    patientAddress4:  "",
                    patientPostcode: "LS1 4HY", 
                    patientPhoneno: "07706 257457", 
                    patientReligion: "Religious affiliation", 
                    patientEthnicity: "White:Eng/Welsh/Scot/NI/Brit - England and Wales 2011 census", 
                    patientPractice: "The Gamston Medical Centre", 
                    patientPracticeAddress: "The Gamston Medical Centre, Gamston District Centre, Gamston, Nottingham NG2 6PS", 
                    patientPracticeCode: "C84703",  
                    patientGPTitle: "Dr", 
                    patientGPFirstName: "Linda", 
                    patientGPSurname: "Kandola", 
                    patientGPCode: "G9313091"
                }
            }

            async function getDummyData2(){
                let result = null;
                var options = appJS.makeApiCallDefaultOption();

                await appJS.setSpinnerPromise(true);
                var res = await appJS.makeApiCall(`/Patient/1`, options);

                appJS.setSpinnerPromise(false, 5000);
               //console.log({ result: res.data });
                if(res.data.status){

                    result = res.data.data[0];
                    console.log({ id: result.patientId });
                    $('.confirm').data("id", result.patientId);
                    $('.reject').data("id", result.patientId); 

                }
                else appJS.displayError({ message: res.data.message, title: messageLabel });

                return result; 
            }