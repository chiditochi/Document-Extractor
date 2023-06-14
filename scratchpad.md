

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
    - run ef migrations to create db and tables 
    - write seeding method to create needed entities 
    - configure upload folders 
    - create upload view and test 
    - create confirmation view and test 


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